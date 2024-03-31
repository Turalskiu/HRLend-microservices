using AuthorizationApi.Models;
using Helpers.Except;
using Microsoft.Extensions.Options;
using AuthorizationApi.Utils;
using AuthorizationApi.Settings;
using AuthorizationApi.Repository;
using AuthorizationApi.Models.DTO.Request;
using AuthorizationApi.Models.DTO.Response;
using AuthorizationApi.Models.DTO.Session;

namespace AuthorizationApi.Services
{

    public interface IUserService
    {
        User Registration(RegistrationSession user);
        User Registration(RegistrationByTokenSession user);
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        void RevokeToken(string token, string ipAddress);
    }



    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        private IAdminRepository _adminRepository;
        private ICabinetRepository _cabinetRepository;
        private IJwtUtils _jwtUtils;
        private readonly AppSetting _appSettings;

        public UserService(
            IUserRepository userRepository,
            IAdminRepository adminRepository,
            ICabinetRepository cabinetRepository,
            IJwtUtils jwtUtils,
            IOptions<AppSetting> appSettings
            )
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _cabinetRepository = cabinetRepository;
            _jwtUtils = jwtUtils;
            _appSettings = appSettings.Value;
        }


        public User Registration(RegistrationSession user)
        {
            /* Запрос бд: 2 */

            var newCabinet = new Cabinet
            {
                Title = user.CabinetTitle,
                StatusId = (int)CABINET_STATUS.ACTIVATED,
                DateCreate = DateTime.Now
            };

            int cabinetId = _cabinetRepository.InsertCabinet(newCabinet);

            var newUser = new User
            {
                CabinetId = cabinetId,
                Username = user.Username,
                Email = user.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                StatusId = (int)USER_STATUS.ACTIVATED,
                DateCreate = user.DateCreate,
                DateActivation = DateTime.Now,
                Roles = new List<Role>
                {
                    //new Role {Id = (int)ROLE.ADMIN },
                    new Role {Id = (int)ROLE.USER },
                    new Role {Id = (int)ROLE.CABINET_ADMIN }
                }
            };

            int userId = _userRepository.InsertUser(newUser);
            newUser.Id = userId;

            return newUser;
        }


        public User Registration(RegistrationByTokenSession user)
        {
            /* Запрос бд: 1 */

            var newUser = new User
            {
                CabinetId = user.CabinetId,
                Username = user.Username,
                Email = user.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                StatusId = (int)USER_STATUS.ACTIVATED,
                DateCreate = user.DateCreate,
                DateActivation = DateTime.Now,
                Roles = new List<Role>
                {
                    new Role {Id = (int)ROLE.USER },
                    new Role {Id = user.CabinetRoleId}
                }
            };

            int userId = _userRepository.InsertUser(newUser);
            newUser.Id = userId;

            return newUser;
        }


        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            /* Запрос бд: 3 */

            var user = _userRepository.GetUserAndRefreshToken(model.Username);

            if (user == null) 
                throw new AppException("Username or password is incorrect");

            if (user.StatusId == (int)USER_STATUS.DELETED) return null;

            if (IsBlock(user))
                return new AuthenticateResponse()
                {
                    IsBlocked = true,
                    DateBlocked = user.DateBlocked,
                    DateUnblocked = user.DateUnblocked,
                    ReasonBlocked = user.ReasonBlocked
                };

            // validate
            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                throw new AppException("Username or password is incorrect");

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = _jwtUtils.GenerateJwtToken(user);
            RefreshToken refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            refreshToken.UserId = user.Id;

            //добавляем refreshToken в бд
            if (!AddRefreshToken(refreshToken)) return null;

            // remove old refresh tokens from user
            removeOldRefreshTokens(user);

            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            /* Запрос бд: 4 */

            var user = getUserByRefreshToken(token);

            if(user.StatusId == (int)USER_STATUS.DELETED) return null;

            if(IsBlock(user)) 
                return new AuthenticateResponse() 
                { 
                    IsBlocked = true,
                    DateBlocked = user.DateBlocked,
                    DateUnblocked = user.DateUnblocked,
                    ReasonBlocked = user.ReasonBlocked
                };

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            //if (refreshToken.IsRevoked)
            //{
            //    // revoke all descendant tokens in case this token has been compromised
            //    List<int> ids = revokeDescendantRefreshTokens(refreshToken, user);
            //    RefreshToken recallToken = new RefreshToken 
            //    { 
            //        RevokedByIp = ipAddress, 
            //        ReplacedByToken = null,
            //        Revoked = DateTime.Now,
            //        ReasonRevoked = $"Attempted reuse of revoked ancestor token: {token}"
            //    };
            //    _userRepository.UpdateRefreshToken(ids, refreshToken);
            //}

            if (!refreshToken.IsActive)
                throw new AppException("Invalid token");

            // replace old refresh token with a new one (rotate token)
            var newRefreshToken = rotateRefreshToken(refreshToken, ipAddress);
            newRefreshToken.UserId = user.Id;

            //добавляем refreshToken в бд
            if (!AddRefreshToken(newRefreshToken)) return null;

            // remove old refresh tokens from user
            removeOldRefreshTokens(user);

            // generate new jwt
            var jwtToken = _jwtUtils.GenerateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
        }

        public void RevokeToken(string token, string ipAddress)
        {
            /* Запрос бд: 2 */

            var user = getUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
                throw new AppException("Invalid token");

            // revoke token and save
            revokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
            _userRepository.UpdateRefreshToken(refreshToken);
        }



        // helper methods
        private User getUserByRefreshToken(string token)
        {
            var user = _userRepository.GetUserByRefreshToken(token);

            if (user == null)
                throw new AppException("Invalid token");

            return user;
        }

        private RefreshToken rotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            revokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            _userRepository.UpdateRefreshToken(refreshToken);
            return newRefreshToken;
        }

        private void removeOldRefreshTokens(User user)
        {
            // remove old inactive refresh tokens from user based on TTL in app settings
            var refreshTokens = user.RefreshTokens.Where(x =>
                !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.Now);

            if(refreshTokens.Count() != 0) _userRepository.DeleteRefreshToken(refreshTokens.Select(rt => rt.Id).ToArray());
        }


        //получить список id токенов которых нужно отозвать
        private List<int> revokeDescendantRefreshTokens(RefreshToken refreshToken, User user)
        {

            List<int> listId = new List<int>();

            RefreshToken current = refreshToken;
            while (!string.IsNullOrEmpty(current.ReplacedByToken))
            {
                current = user.RefreshTokens.SingleOrDefault(x => x.Token == current.ReplacedByToken);
                if (current.IsActive)
                    listId.Add(current.Id);
            }

            return listId;
        }

        private void revokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
        {
            token.Revoked = DateTime.Now;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }


        private bool IsBlock(User user)
        {
            if (user.StatusId != (int)USER_STATUS.BLOCKED) return false;
            else
            {
                if (user.DateUnblocked <= DateTime.Now)
                {
                    _adminRepository.UnblockedUser(user.Id);
                    user.StatusId = (int)USER_STATUS.ACTIVATED;
                    return false;
                }
                else return true;
            }
        }

        //попытка создать уникальный refreshToken
        private bool AddRefreshToken(RefreshToken refreshToken)
        {
            bool isGeneretedRefreshToken = _userRepository.InsertRefreshToken(refreshToken);

            int i = 0;
            RefreshToken newRefreshToken;
            while (!isGeneretedRefreshToken && i < 3)
            {
                i++;
                newRefreshToken = _jwtUtils.GenerateRefreshToken(refreshToken.CreatedByIp);
                newRefreshToken.UserId = refreshToken.UserId;
                isGeneretedRefreshToken = _userRepository.InsertRefreshToken(newRefreshToken);
            };
            return isGeneretedRefreshToken;
        }
    }
}

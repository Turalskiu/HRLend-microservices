using AuthorizationApi.Models;
using AuthorizationApi.Models.DTO.Session;
using AuthorizationApi.Repository;
using AuthorizationApi.Services;
using AuthorizationApi.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthorizationApi.Utils
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(User user);
        public UserSession? ValidateJwtToken(string token);
        public RefreshToken GenerateRefreshToken(string ipAddress);
    }

    public class JwtUtils : IJwtUtils
    {
        private IUserRepository _userRepository;
        private readonly AppSetting _appSettings;

        public JwtUtils(
            IUserRepository userRepository,
            IOptions<AppSetting> appSettings)
        {
            _userRepository = userRepository;
            _appSettings = appSettings.Value;
        }

        public string GenerateJwtToken(User user)
        {
            // generate token that is valid for 15 minutes
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim("user_id", user.Id.ToString()),
                new Claim("cabinet_id", user.CabinetId.ToString())
            };

            //foreach (var role in user.Roles)
            //{
            //    Console.WriteLine(role.Id);
            //}

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Id.ToString()));
            }

            //Console.WriteLine("claims count: " + claims.Count());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public UserSession? ValidateJwtToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "user_id").Value);
                var cabinetId = int.Parse(jwtToken.Claims.First(x => x.Type == "cabinet_id").Value);
                var roles = jwtToken.Claims.Where(x => x.Type == "role").Select(c => int.Parse(c.Value));

                // return user id from JWT token if validation successful
                return new UserSession
                {
                    Id = userId,
                    CabinetId = cabinetId,
                    Roles = roles.ToList()
                };
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }

        public RefreshToken GenerateRefreshToken(string ipAddress)
        {
            var refreshToken = new RefreshToken
            {
                Token = getUniqueToken(),
                // token is valid for 7 days
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            return refreshToken;

            string getUniqueToken()
            {
                // token is a cryptographically strong random sequence of values
                var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                return token;
            }
        }
    }
}

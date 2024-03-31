using KnowledgeBaseApi.Domain.Auth;
using KnowledgeBaseApi.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace KnowledgeBaseApi.Utils
{
    public interface IJwtUtils
    {
        public User? ValidateJwtToken(string token);
    }

    public class JwtUtils : IJwtUtils
    {
        private readonly AppSetting _appSettings;

        public JwtUtils(
            IOptions<AppSetting> appSettings)
        {
            _appSettings = appSettings.Value;
        }


        public User? ValidateJwtToken(string token)
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
                return new User
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

    }
}

using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SCMS_back_end.Repositories.Services
{
    public class JwtTokenService
    {
        public static TokenValidationParameters ValidateToken(IConfiguration configuration)
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSecurityKey(configuration),
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateAudience = false            
            };

        }

        public static SecurityKey GetSecurityKey(IConfiguration configuration)
        {
            var secretKey = configuration["JWT:SecretKey"];
            if (secretKey == null)
            {
                throw new InvalidOperationException("Jwt Secret key is not exsist");
            }

            var secretBytes = Encoding.UTF8.GetBytes(secretKey);

            return new SymmetricSecurityKey(secretBytes);
        }
    }
}

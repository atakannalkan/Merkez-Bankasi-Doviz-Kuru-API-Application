using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using dovizapp.shared.Entities;
using dovizapp.shared.Utilities.Security.Abstract;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace dovizapp.shared.Utilities.Security.Concrete
{
    public class JwtTokenGenerator : ITokenGenerator
    {
        private readonly JwtSettingsModel _jwtSettingsModel;
        public JwtTokenGenerator(IOptionsSnapshot<JwtSettingsModel> jwtSettingsModel)
        {
            _jwtSettingsModel = jwtSettingsModel.Value;
        }

        public string CreateToken(string username, string role)
        {
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_jwtSettingsModel.SecretKey));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Role, role)
            };

            JwtSecurityToken token = new(
                issuer: _jwtSettingsModel.Issuer,
                audience: _jwtSettingsModel.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(_jwtSettingsModel.ExpirationTimeByHour),
                signingCredentials: credentials
            );

            JwtSecurityTokenHandler handler = new();
            return handler.WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_jwtSettingsModel.SecretKey));

            try
            {
                JwtSecurityTokenHandler handler = new();
                handler.ValidateToken(token, new TokenValidationParameters() {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = _jwtSettingsModel.Issuer,
                    ValidAudience = _jwtSettingsModel.Audience,
                    IssuerSigningKey = securityKey
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
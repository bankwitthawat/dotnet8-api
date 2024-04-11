﻿using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DataModel.ViewModels.Auth.LogIn;
using DataModel.ViewModels.Auth.Token;
using System.Text.Json;

namespace BusinessLogic.Utilities
{
    public class JwtManager
    {
        public readonly IConfiguration _configuration;
        public JwtManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(LogInResponse user)
        {
            Int32.TryParse(_configuration.GetSection("JwtSetting:ExpireMins").Value, out int tokenExpire);
            var appauthorize = JsonSerializer.Serialize(user.AppModule);

            List<Claim> claims = new List<Claim> {
                new Claim (ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim (ClaimTypes.Name, user.Username),
                new Claim ("refreshtoken", user.RefreshToken),
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSetting:AccessSecret").Value)
            );

            SigningCredentials creds = new SigningCredentials(
               key, SecurityAlgorithms.HmacSha512Signature
           );

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(tokenExpire),
                SigningCredentials = creds,
                Issuer = _configuration.GetSection("JwtSetting:Issuer").Value,
                Audience = _configuration.GetSection("JwtSetting:Audience").Value,
                NotBefore = DateTime.UtcNow,
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(string ipAddress, string userAgent, string machineName)
        {
            Int32.TryParse(_configuration.GetSection("JwtSetting:ExpireMins").Value, out int tokenExpire);
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    ExpiresOn = DateTime.Now.AddMinutes(tokenExpire),
                    IssuedOn = DateTime.Now,
                    RequestIp = ipAddress,
                    UserAgent = userAgent,
                    MachineName = machineName
                };
            }
        }

        public int? ValidateJwtToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtSetting:AccessSecret").Value);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration.GetSection("JwtSetting:Issuer").Value,
                    ValidAudience = _configuration.GetSection("JwtSetting:Audience").Value,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

                // return user id from JWT token if validation successful
                return userId;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }
    }
}


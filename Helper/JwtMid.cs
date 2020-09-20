using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using scoreapp.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scoreapp.Helper
{
    public class JwtMid
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ScoreDbContext _db = new ScoreDbContext();
        public JwtMid(IConfiguration configuration, RequestDelegate next)
        {
            _next = next; _configuration = configuration;
        }
        public async Task Invoke(HttpContext context, ITokkenService tokkenService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                attachUserToContext(context, tokkenService, token);

            await _next(context);
        }
        private void attachUserToContext(HttpContext context, ITokkenService tokkenService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var xusername = jwtToken.Claims.First(x => x.Type == "username").Value;
                // attach user to context on successful jwt validation
                context.Items["User"] = tokkenService.GetValidUser(xusername);
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}

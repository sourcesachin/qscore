using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using scoreapp.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace scoreapp.Helper
{
    public interface ITokkenService
    {
        string GetFakeTokkenAsync(String username);
        string GetValidUser(String username);
    }

    public class TokkenService : ITokkenService
    {
        private readonly ScoreDbContext _db = new ScoreDbContext();
        private readonly IConfiguration _configuration;
        public TokkenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetFakeTokkenAsync(String username)
        {
            var old = _db.Scores.FirstOrDefaultAsync<ScoreModel>(o => o.UserName == username);
            if (old == null)
            {
                _db.Scores.Add(new ScoreModel() { UserName = username, Points = 0 });
                 _db.SaveChangesAsync();
            }
            return generateJwtToken(username);
        }

        public string GetValidUser(string username)
        {
            return _db.Scores.Count(x => x.UserName == username)>0?username:null;
        }

        private string generateJwtToken(String username)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            //configuration.GetValue<string>("Jwt:Secret"); 
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("username", username) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
 
    }
}

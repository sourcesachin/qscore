using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using scoreapp.Data;
using scoreapp.Helper;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace scoreapp.Controllers
{
    [ApiController, ScoreAuthorizeAttribute]
    public class ScoreApiController : ControllerBase
    {
        private readonly ScoreDbContext _db = new ScoreDbContext();

        [Route("api/getallscore"), HttpGet]
        public async Task<ScoreModelContainer> GetScore() {
            return new ScoreModelContainer
            {
                Scores = await Queryable.OrderByDescending<ScoreModel, int>(_db.Scores, o => o.Points).ToArrayAsync()
            };
        }

        [Route("api/getscore/{username}"), HttpGet]
        public Task<ScoreModel> GetScore(string username)
        {
            return _db.Scores.FirstOrDefaultAsync<ScoreModel>(o => o.UserName == username);
        }

        [Route("api/addscore"), HttpPost]
        [HttpPost]
        public async Task<IActionResult> AddScore([FromBody]ScoreModel model)
        {
            if (!ModelState.IsValid) {
                return BadRequest("Bad Username!");
            }
            //Check for new or update
            var old = await _db.Scores.FirstOrDefaultAsync<ScoreModel>(o => o.UserName == model.UserName);
            if (old == null)
            {
                //New
                _db.Scores.Add(model);
                await _db.SaveChangesAsync();
                return Ok(model);
            }
            else
            {
                //UPDATE
                old.Points = model.Points;
                await _db.SaveChangesAsync();
                return Ok(model);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using scoreapp.Data;
using scoreapp.Helper;

namespace scoreapp.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private ITokkenService _tokkenService;

        public UserController(ITokkenService tokkenService)
        {
            _tokkenService = tokkenService;
        }

        [Route("api"), HttpGet]
        public String index()
        {
            return "Welcome to WebApi";
        }

        [Route("api/gettokken/{username}"), HttpGet]
        public String GetFakeTokkenAsync(String username)
        {
            return _tokkenService.GetFakeTokkenAsync(username);
        }

        
    }
}

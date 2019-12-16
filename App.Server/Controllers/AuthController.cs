using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Shared;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase, IAuthService
    {
        [HttpGet]
        public Task<string[]> GetUserPermissions()
        {
            return Task.FromResult(new string[]
            {
                "AAAA",
                "BBBB"
            });
        }
    }
}
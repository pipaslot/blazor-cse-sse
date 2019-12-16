using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{
    [Route("api/value")]
    [ApiController]
    public class ValueController : ControllerBase
    {
        [HttpGet]
        public string[] GetAll()
        {
            return new string[]
            {
                "AAAA",
                "BBBB"
            };
        }
    }
}
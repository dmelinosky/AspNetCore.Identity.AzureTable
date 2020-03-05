using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web.Models;

namespace web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityKeyController : ControllerBase
    {
        [HttpPost]
        [Route("RegisterCallback")]
        public IActionResult RegisterCallback([FromBody] CredentialsModel model)
        {
            return Ok();
        }
    }
}
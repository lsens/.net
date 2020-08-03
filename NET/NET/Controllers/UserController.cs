using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NET.Controllers
{
    //[Route("api/[controller]/[action]")]
    //[ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public IActionResult Login()
        {
            return Ok("2");
        }

        [HttpPost]
        public IActionResult GetLine([FromBody]Statistical.User.UserP userP)
        {
            var r = Bo.GetData.GetLine(userP);
            return Ok(r);
        }

        [HttpPost]
        public IActionResult GetData([FromBody]Statistical.PR.P p)
        {
            if (p.status == 0)
            {
                var r = Bo.GetCount.GetR(p);
                return Ok(r);
            }
            else if (p.status > 100 && p.status < 200)
            {
                var r = Bo.GetCount.GetMonthR(p);
                return Ok(r);
            }
            else if (p.status > 200 && p.status < 300)
            {
                var r = Bo.GetCount.GetMinuteR(p);
                return Ok(r);
            }
            else if (p.status > 90)
            {
                var r = Bo.GetCount.GetDayR(p);
                return Ok(r);
            }
            else
            {
                var r = Bo.GetCount.GetWarnR(p);
                return Ok(r);
            }
        }
    }
}






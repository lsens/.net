using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Statistical.PR;
using Tools;

namespace NET.Controllers
{
    //[Route("api/[controller]/[action]")]
    //[ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public IActionResult GetData([FromBody]TestP p)
        {
            if (p.status == 0)
            {
                var r = Bo.GetCount.GetR(p);
                return Ok(r);
            }
            else if (p.status > 100 && p.status < 200)
            {
                var r = Bo.GetCount.GetDayR(p);
                return Ok(r);
            }
            else if (p.status > 200 && p.status < 300)
            {
                var r = Bo.GetCount.GetMonthR(p);
                return Ok(r);
            }
            else if (p.status > 300 && p.status < 400)
            {
                var r = Bo.GetCount.GetYearR(p);
                return Ok(r);
            }
            else if (p.status > 90)
            {
                var r = Bo.GetCount.GetWeekDayR(p);
                return Ok(r);
            }
            else
            {
                return Ok("出错了");
            }
        }

        [HttpPost]
        public IActionResult Test([FromBody]P p)
        {
            //ModuleTools mt = new ModuleTools();

            //List<string> jsonDatas = new List<string>
            //{
            //    p.data
            //};

            //List<DateTime> times = new List<DateTime>
            //{
            //    p.startTime,
            //    p.endTime
            //};

            //var r = mt.ReturnModule(jsonDatas, times, 1);

            return Ok(p.startTime);
        }

        [HttpGet]
        public IActionResult Test2([FromBody]TestP p) 
        {
            var r = Bo.GetCount.GetDeepSleepR(p);
            return Ok(r);
        }

        [HttpGet]
        public IActionResult Demo([FromBody]TestP p)
        {
            if (p.status == 0)
            {
                var r = Bo.GetCount.GetDeepSleepR(p);
                return Ok(r);
            }
            else
            {
                var r = Bo.GetCount.GetDeepSleepR(p);
                return Ok(r);
            }
        }

    }
}






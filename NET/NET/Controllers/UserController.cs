using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        public IActionResult Test([FromBody] Rootobject p)
        {
            ModuleTools mt = new ModuleTools();

            string jsonData = "[";
            List<string> jsonDatas = new List<string>();

            for (int i = 0; i < p.data.Length; i++)
            {
                string dd = JsonConvert.SerializeObject(p.data[i]);
                jsonData += dd;
                jsonData += ",";
            }

            jsonData += "]";

            jsonDatas.Add(jsonData);

            List<DateTime> times = new List<DateTime>
            {
                Convert.ToDateTime(p.startTime),
                Convert.ToDateTime(p.endTime)
            };

            var r = mt.ReturnModule(jsonDatas, times, 1, 1);

            //var r = jsonDatas;

            return Ok(r);

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







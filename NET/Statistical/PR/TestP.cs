﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Statistical.PR
{
    public class TestP
    {
        public string data { get; set; }
        public int status { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }

    }

    public class Rootobject
    {
        public string startTime { get; set; }
        public string endTime { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public float hrAvg { get; set; }
        public float hrMin { get; set; }
        public float hrMax { get; set; }
        public int hrTimes { get; set; }
        public int status { get; set; }
        public string[] time { get; set; }
    }

    public class TB_CHARGING
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 充电设备名称
        /// </summary>
        public string NAME { get; set; }

        /// <summary>
        /// 充电设备描述
        /// </summary>
        public string DES { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CREATETIME { get; set; }
    }



    public class Rootobject1
    {
        public string message { get; set; }
        public string nu { get; set; }
        public string ischeck { get; set; }
        public string condition { get; set; }
        public string com { get; set; }
        public string status { get; set; }
        public string state { get; set; }
        public Datum1[] data { get; set; }
    }

    public class Datum1
    {
        public string time { get; set; }
        public string ftime { get; set; }
        public string context { get; set; }
        public string location { get; set; }
    }

}

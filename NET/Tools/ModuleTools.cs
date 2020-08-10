﻿using System;
using Data;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Statistical.PR;
using Newtonsoft.Json;

namespace Tools
{
    public class ModuleTools
    {
        public R ReturnModule(List<string> jsonDatas, List<DateTime> times, int timeSpan = 1, int dataStatus = 0) 
        {
            R r = new R();

            DataTools ts = new DataTools();
            DataCount dc = new DataCount();
            CalculateData cd = new CalculateData();

            int[] windows = ts.GetWindow(timeSpan);
            List<double> cDatas = new List<double>();
            List<double> aDatas = new List<double>();
            List<DateTime> dTimes = new List<DateTime>();
            List<DateTime> cTimes = new List<DateTime>();

            // 1 日 2 月 3 年
            if (timeSpan == 1)
            {
                TimeCounts timeCounts = new TimeCounts();
                List<JsonData> jsonData = JsonConvert.DeserializeObject<List<JsonData>>(jsonDatas[0]);
                timeCounts = dc.GetDayCount(jsonData, times[0], times[1], dataStatus);
                cDatas = timeCounts.Counts;
                dTimes = timeCounts.Times;
            }
            if (timeSpan == 2)
            {
                // 把每天的数据分成24小时  取2小时小窗口
                cDatas = dc.GetMonthCount(jsonDatas, dataStatus);
            }
            if (timeSpan == 3)
            {
                // 取两周的数据为大窗口  取2天小窗口
                cDatas = dc.GetJsonDataCount(jsonDatas, dataStatus);
                dTimes = times;
            }

            if (timeSpan == 2)
            {
                cTimes = times;
                aDatas = dc.GetJsonDataCount(jsonDatas, dataStatus);
            }
            else
            {
                for (int i = 0; i < dTimes.Count / windows[1]; i++)
                {
                    aDatas.Add(cDatas.Skip(i * windows[1]).Take(windows[1]).Sum());
                    cTimes.Add(dTimes[i * windows[1]]);
                }
            }

            CutCalculateList bDatas = cd.CutCalculateData(cDatas, windows[0], windows[1]);
            List<double> ttList = bDatas.TrendCList.Select(x => x.Value).ToList();

            double[] arr = cd.DataPercentileInplace(aDatas);
            double[] normalData = { (int)Math.Round(arr[1]), (int)Math.Round(arr[2]) };
            double[] abnormalData = { (int)Math.Round(arr[0]), (int)Math.Round(arr[3]) };

            List<double> abPointData = new List<double>();
            List<DateTime> abPointTime = new List<DateTime>();

            for (int j = 0; j < aDatas.Count; j++)
            {
                if (aDatas[j] > abnormalData[1])
                {
                    abPointData.Add((int)aDatas[j]);
                    abPointTime.Add(cTimes[j]);
                }
            }

            r = new R
            {
                LineData = ttList.ToArray(),
                LineTime = cTimes.Select(x => x.ToString("yyyy-MM-dd HH:mm")).ToArray(),
                PointData = aDatas.ToArray(),
                AbPointData = abPointData.ToArray(),
                AbPointTime = abPointTime.Select(x => x.ToString("yyyy-MM-dd HH:mm")).ToArray(),
                NormalData = normalData,
                AbnormalData = abnormalData.ToArray()
            };
        
         
            return r;
        }
    }
}



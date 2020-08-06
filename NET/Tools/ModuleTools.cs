using System;
using Data;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Statistical.PR;

namespace Tools
{
    public class ModuleTools
    {
        //  封装算法模型
        //  封装个人规律输出模块 
        public MinuteR BasicModule(List<double> datas, List<DateTime> times, int cWindow = 2, int aWindow = 14)
        {
            DataTools tools = new DataTools();
            DataCount dc = new DataCount();
            CalculateData cd = new CalculateData();

            CutCalculateList tList = cd.CutCalculateData(datas, cWindow, aWindow);

            //  tt  取得趋势线值
            List<double> ttList = tList.TrendCList.Select(x => x.Value).ToList();

            //  ttC 取得正常范围
            double[] ttC = cd.PercentileData(ttList);

            //  ta  取得变换趋势
            List<double?> taList = tList.AmplitudeCList;

            //  taC 取得变换趋势异常点 95分箱值有正负
            List<double?> taCList = tools.GetAmpC95(taList);
            double? taC95 = taCList[0];
            double? ntaC95 = taCList[1];

            List<DateTime> cTimes = new List<DateTime>();
            List<double> cDatas = new List<double>();
            for (int i = 0; i < times.Count / aWindow; i++)
            {
                cDatas.Add(datas.Skip(i * aWindow).Take(aWindow).Sum());
                cTimes.Add(times[i * aWindow]);
            }



            List<double> PointData = new List<double>();
            List<string> LineTime = new List<string>();
            List<string> PointTime = new List<string>();

            int[] normalData = { (int)Math.Round(ttC[1]), (int)Math.Round(ttC[2]) };
            MinuteR r = new MinuteR
            {
                LineData = ttList.ToArray(),
                LineTime = cTimes.Select(x => x.ToString("HH:mm")).ToArray(),
                PointData = cDatas.ToArray(),
                NormalData = normalData,
            };
            
            return r;

        }

        public R ReturnModule(List<string> jsonDatas, List<DateTime> times, int cWindow = 2, int aWindow = 14, int dataStatus = 0) 
        {

            DataTools ts = new DataTools();
            ModuleTools mt = new ModuleTools();
            DataCount dc = new DataCount();
            CalculateData cd = new CalculateData();

            List<double> cDatas = dc.GetNewDayCount(jsonDatas);
            CutCalculateList bDatas = cd.CutCalculateData(cDatas, cWindow, aWindow);
            //  咳嗽特殊情况
            if (dataStatus == 3)
            {
                cDatas = dc.GetNewDayCount(jsonDatas, 1);
                bDatas = cd.CutCalculateData(cDatas, cWindow, aWindow);
            }

            List<double> aDatas = dc.GetJsonDataCount(jsonDatas);

            List<double?> monthCAClist = bDatas.AmplitudeCList;

            List<double> ttList = bDatas.TrendCList.Select(x => x.Value).ToList();

            double[] arr = cd.DataPercentileInplace(aDatas);
            double[] normalData = { (int)Math.Round(arr[1]), (int)Math.Round(arr[2]) };
            double[] abnormalData = { (int)Math.Round(arr[0]), (int)Math.Round(arr[3]) };

            List<double> abPointData = new List<double>();
            List<int> abPointTime = new List<int>();

            for (int j = 0; j < aDatas.Count; j++)
            {
                if (aDatas[j] > abnormalData[1])
                {
                    abPointData.Add((int)aDatas[j]);
                    abPointTime.Add(times[j].Day);
                }
            }

            R r = new R();
            r = new R
            {
                LineData = ttList.ToArray(),
                LineTime = times.Select(x => x.ToString()).ToArray(),
                PointData = aDatas.ToArray(),
                AbPointData = abPointData.ToArray(),
                AbPointTime = abPointTime.Select(x => x.ToString()).ToArray(),
                NormalData = normalData,
                AbnormalData = abnormalData.ToArray()
            };
            return r;
        }
    }
}

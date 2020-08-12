using Data;
using Tools;
using Newtonsoft.Json;
using Statistical.PR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NPOI.SS.Formula.Functions;

namespace Bo
{
    public class GetCount
    {
        public static TestR GetR(TestP p)
        {
            ReadExcel rd = new ReadExcel();
            List<ExcelData> excelDatas = rd.ImportExcel(p.data);

            List<DateTime?> startSleepData;
            List<DateTime?> endSleepData;

            startSleepData = excelDatas
            .Select(x => x.StartSleepTime)
            .ToList();

            endSleepData = excelDatas
                .Select(x => x.EndSleepTime)
                .ToList();

            DataCount dc = new DataCount();
            CalculateData calculateData = new CalculateData();

            List<TimeAndListClass> timeAndListClass = dc.GetTimeAndList(startSleepData);

            //  入睡时间模型
            SleepTimeLineAndPoint sleepTimeLineAndPoint = calculateData.GetSleepTimeLineAndPoint(timeAndListClass, endSleepData, 2, 14);

            var r = new TestR
            {
                LineData = sleepTimeLineAndPoint.LineData.ToArray(),
                LineTime = sleepTimeLineAndPoint.LineTime.ToArray(),
                PointData = sleepTimeLineAndPoint.PointData.ToArray(),
                PointTime = sleepTimeLineAndPoint.PointTime.ToArray(),
                NormalData = sleepTimeLineAndPoint.NormalData.ToArray(),
                AbnormalData = sleepTimeLineAndPoint.AbnormalData.ToArray()
            };
            return r;
        }

        public static R GetWarnR(TestP p)
        {
            ReadExcel rd = new ReadExcel();
            List<ExcelData> excelDatas = rd.ImportExcel(p.data);

            List<DateTime?> endSleepData;

            endSleepData = excelDatas
                .Select(x => x.EndSleepTime)
                .ToList();

            DataTools tools = new DataTools();
            DataCount dc = new DataCount();
            CalculateData calculateData = new CalculateData();

            List<double> datas = dc.GetJsonDataCount(tools.GetDataType(p.data, p.status));

            CutCalculateList cutCalculateList = calculateData.CutCalculateData(datas, 2, 14);

            List<StartEndTime> timeData = calculateData.CutTimes(endSleepData, 14);

            List<double> PointData = new List<double>();
            List<string> LineTime = new List<string>();
            List<string> PointTime = new List<string>();

            List<double?> ampC = cutCalculateList.AmplitudeCList;

            List<double?> ampCTwo = tools.GetAmpC95(ampC);

            double? ampC95 = ampCTwo[0];
            double? nampC95 = ampCTwo[1];

            for (int i = 0; i < timeData.Count; i++)
            {
                string time = string.Format("{0},{1}", timeData[i].StartTime.ToShortDateString(), timeData[i].EndTime.ToShortDateString());
                LineTime.Add(time);
                if ((ampC95 != null && ampC[i] >= ampC95) || (nampC95 != null && ampC[i] <= nampC95))
                {
                    PointData.Add(ampC[i].Value);
                    PointTime.Add(time);  
                }
            }
            
            List<double> ttList = cutCalculateList.TrendCList.Select(x => x.Value).ToList();
            double [] ttC = calculateData.PercentileData(ttList);
            double[] normalData = { ttC[1], ttC[2] };
            var r = new R
            {
                LineData = ttList.ToArray(),
                LineTime = LineTime.ToArray(),
                AbPointData = PointData.ToArray(),
                AbPointTime = PointTime.ToArray(),
                NormalData = normalData
            };
            return r;
        }

        public static DayR GetWeekDayR(TestP p)
        {
            ReadExcel rd = new ReadExcel();
            List<ExcelData> excelDatas = rd.ImportExcel(p.data);
            List<DateTime?> startSleepData;
            List<DateTime?> endSleepData;

            startSleepData = excelDatas
            .Select(x => x.StartSleepTime)
            .ToList();

            endSleepData = excelDatas
                .Select(x => x.EndSleepTime)
                .ToList();

            DataTools tools = new DataTools();
            DataCount dc = new DataCount();
            CalculateData calculateData = new CalculateData();

            List<string> holidayList = new List<string>
                {
                    "2020/6/25",
                    "2020/5/1",
                    "2020/5/2",
                    "2020/5/3",
                    "2020/5/4",
                    "2020/5/5",
                    "2020/4/4",
                    "2020/4/5",
                    "2020/4/6",
                    "2020/1/29",
                    "2020/1/28",
                    "2020/1/27",
                    "2020/1/26",
                    "2020/1/25",
                    "2020/1/24",
                    "2020/1/1",
                    "2020/1/30",
                    "2020/1/31",
                    "2020/2/1",
                    "2020/2/2",
                };

            List<DayCount> dayCount = new List<DayCount>();

            int dataStatus = p.status - 90;

            dayCount = dc.GetDayCount(tools.GetDataType(p.data, dataStatus), endSleepData);

            List<DayCount> ortherWeek = new List<DayCount>();
            List<DayCount> monday = new List<DayCount>();
            List<DayCount> friday = new List<DayCount>();
            List<DayCount> weekend = new List<DayCount>();
            List<DayCount> holiday = new List<DayCount>();

            if (excelDatas.Count > 14)
            {
                //  心率 异常 实验  
                for (int i = 0; i < dayCount.Count; i++)
                {
                    string week = DateTime.Parse(dayCount[i].DayTime).AddDays(-1).DayOfWeek.ToString();

                    if (week == "Tuesday" || week == "Wednesday" || week == "Thursday")
                    {
                        ortherWeek.Add(dayCount[i]);
                    }
                    else if (week == "Saturday" || week == "Sunday")
                    {
                        weekend.Add(dayCount[i]);
                    }
                    else if (week == "Monday")
                    {
                        monday.Add(dayCount[i]);
                    }
                    else if (week == "Friday")
                    {
                        friday.Add(dayCount[i]);
                    }
                    if (holidayList.Exists(x => x == dayCount[i].DayTime))
                    {
                        holiday.Add(dayCount[i]);
                    }
                }
            }

            var r = new DayR
            {
                weekCount = new int[] { monday.Count, ortherWeek.Count, friday.Count, weekend.Count, holiday.Count }
            };
            return r;
        }


        public static R GetDayR(TestP p) 
        {
            ReadExcel rd = new ReadExcel();
            List<ExcelData> excelDatas = rd.ImportExcel(p.data);

            DataCount dc = new DataCount();
            ModuleTools mt = new ModuleTools();
            CalculateData cd = new CalculateData();

            //   时间段模型  也是 每天 的模型  
            ExcelData excelData = excelDatas.Where(x => x.EndSleepTime.Value.Month == p.month && x.EndSleepTime.Value.Day == p.day).First();

            int dataStatus = p.status - 100;

            List<string> jsonDatas = new List<string>();
            List<DateTime> times = new List<DateTime>();

            string jsonData = "";

            if (dataStatus == 1)
            {
                jsonData = excelData.HeartWarnData;
            }
            else if (dataStatus == 2)
            {
               jsonData = excelData.BreathWarnsData;
            }
            else if (dataStatus == 3)
            {
                jsonData = excelData.CoughJsonData;
            }

            jsonDatas.Add(jsonData);
            times.Add(excelData.StartSleepTime.Value);
            times.Add(excelData.EndSleepTime.Value);

            R r = new R();

            r = mt.ReturnModule(jsonDatas, times, 1, dataStatus);
           
            return r;

        }

        public static R GetMonthR(TestP p)
        {
            ReadExcel rd = new ReadExcel();
            List<ExcelData> excelDatas = rd.ImportExcel(p.data);

            List<DateTime?> endSleepData;

            endSleepData = excelDatas
                .Select(x => x.EndSleepTime)
                .ToList();

            DataTools tools = new DataTools();
            ModuleTools mt = new ModuleTools();
            DataCount dc = new DataCount();

            int dataStatus = p.status - 200;

            List<MonthCount> monthCount = dc.GetMonthTimeDataCount(endSleepData, tools.GetDataType(p.data, dataStatus));

            R r = new R();

            for (int i = 0; i < monthCount.Count; i++)
            {
                if (monthCount[i].Month == p.month)
                {
                    List<string> monthData = monthCount[i].MonthData;
                    List<DateTime> monthTime = monthCount[i].MonthTime;

                    r = mt.ReturnModule(monthData, monthTime, 2, dataStatus);
                }
            }
            
            return r;

        }



        //  跨度过大 数值累加  之间分箱 取异常 效果并不理想  考虑使用一开始的办法  
        public static R GetYearR(TestP p) 
        {
            ReadExcel rd = new ReadExcel();
            List<ExcelData> excelDatas = rd.ImportExcel(p.data);

            List<DateTime?> endSleepData;

            endSleepData = excelDatas
                .Select(x => x.EndSleepTime)
                .ToList();

            DataTools tools = new DataTools();
            ModuleTools mt = new ModuleTools();
            DataCount dc = new DataCount();

            int dataStatus = p.status - 300;

            List<string> datas = tools.GetDataType(p.data, dataStatus);

            R r = new R();

            r = mt.ReturnModule(datas, endSleepData.Select(x => x.Value).ToList(), 3, dataStatus);

            return r;
        }

    }
}



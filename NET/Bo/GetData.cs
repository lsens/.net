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

            DataCount dc = new DataCount();
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

        public static DayR GetWeekDayR(TestP p)
        {
            DataCount dc = new DataCount();
            DataTools ts = new DataTools();
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

            //  读取静态的假期日期txt文件  后期可以直接对该文件进行新的日期添加
            string[] holidayArr = File.ReadAllLines("holiday.txt");
            List<string> holidayList = new List<string>();
            holidayList.AddRange(holidayArr);

            List<DayCount> dayCount = new List<DayCount>();
            int dataStatus = p.status - 90;

            dayCount = dc.GetDayCount(ts.GetDataType(p.data, dataStatus), endSleepData);

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
            DataTools ts = new DataTools();
            ReadExcel rd = new ReadExcel();
            ModuleTools mt = new ModuleTools();
            List<ExcelData> excelDatas = rd.ImportExcel(p.data);

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
            DataTools ts = new DataTools();
            ReadExcel rd = new ReadExcel();
            DataCount dc = new DataCount();
            ModuleTools mt = new ModuleTools();

            List<ExcelData> excelDatas = rd.ImportExcel(p.data);

            List<DateTime?> endSleepData;

            endSleepData = excelDatas
                .Select(x => x.EndSleepTime)
                .ToList();

            int dataStatus = p.status - 200;

            List<MonthCount> monthCount = dc.GetMonthTimeDataCount(endSleepData, ts.GetDataType(p.data, dataStatus));

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

        public static R GetYearR(TestP p)
        {

            DataTools ts = new DataTools();
            ReadExcel rd = new ReadExcel();
            ModuleTools mt = new ModuleTools();

            List<ExcelData> excelDatas = rd.ImportExcel(p.data);

            List<DateTime?> endSleepData;

            endSleepData = excelDatas
                .Select(x => x.EndSleepTime)
                .ToList();

            int dataStatus = p.status - 300;
            List<string> datas = ts.GetDataType(p.data, dataStatus);

            R r = new R();
            r = mt.ReturnModule(datas, endSleepData.Select(x => x.Value).ToList(), 3, dataStatus);
            return r;

        }

        public static ClassDemo.R Test(TestP p) 
        {

            DataTools ts = new DataTools();
            ReadExcel rd = new ReadExcel();
            DataCount dc = new DataCount();
            ClassDemo.ReturnDemo cr = new ClassDemo.ReturnDemo();

            List<ExcelData> excelDatas = rd.ImportExcel(p.data);

            List<DateTime?> endSleepData;

            endSleepData = excelDatas
                .Select(x => x.EndSleepTime)
                .ToList();

            int dataStatus = p.status - 200;

            List<MonthCount> monthCount = dc.GetMonthTimeDataCount(endSleepData, ts.GetDataType(p.data, dataStatus));

            ClassDemo.R r = new ClassDemo.R();

            for (int i = 0; i < monthCount.Count; i++)
            {
                if (monthCount[i].Month == p.month)
                {
                    List<string> monthData = monthCount[i].MonthData;
                    List<DateTime> monthTime = monthCount[i].MonthTime;

                    r = cr.ReturnModule(monthData, monthTime, 2, dataStatus);
                }
            }

            return r;
        }

        public static R GetDeepSleepR(TestP p) 
        {
            DataTools ts = new DataTools();
            ReadExcel rd = new ReadExcel();
            DataCount dc = new DataCount();
            ModuleTools mt = new ModuleTools();

            List<ExcelData> excelDatas = rd.ImportExcel(p.data);

            List<string> periodData;
            periodData = excelDatas
              .Select(x => x.PeriodData)
              .ToList();

            //  深睡时间模型
            List<double> DeepSleepTimeData = new List<double>();
            DeepSleepTimeData = dc.GetDeepSleepTimeData(periodData);

            List<DateTime?> endSleepData;
            endSleepData = excelDatas
            .Select(x => x.EndSleepTime)
            .ToList();

            R r = new R
            {
                LineData = DeepSleepTimeData.ToArray(),
                LineTime = endSleepData.Select(x => x.Value.ToString("MM-dd")).ToArray()
            };

            return r;
        }

    }
}



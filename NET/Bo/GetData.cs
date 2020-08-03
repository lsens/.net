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
    public class GetData
    {
        public static Statistical.User.UserR GetLine(Statistical.User.UserP userP)
        {
            var r = new Statistical.User.UserR();
            if (userP.num2 == 0)
            {
                r.sum1 = 0;
                r.message = "除数不能为0";
            }
            else
            {
                r.sum1 = userP.num1 / userP.num2;
                r.message = "成功";
            }
            return r;
        }
    }

    public class GetCount
    {
        public static R GetR(P p)
        {
            ReadExcel rd = new ReadExcel();
            string filepath = Path.Combine("wwwroot", string.Format("{0}年后正常数据.xlsx", p.data));
            List<ExcelData> excelDatas = rd.ImportExcel(filepath);

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

            var r = new R
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

        public static WarnR GetWarnR(P p)
        {
            ReadExcel rd = new ReadExcel();
            string filepath = Path.Combine("wwwroot", string.Format("{0}年后正常数据.xlsx", p.data));
            List<ExcelData> excelDatas = rd.ImportExcel(filepath);

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
            int[] normalData = { (int)Math.Round(ttC[1]), (int)Math.Round(ttC[2]) };
            var r = new WarnR
            {
                LineData = ttList.ToArray(),
                LineTime = LineTime.ToArray(),
                PointData = PointData.ToArray(),
                PointTime = PointTime.ToArray(),
                NormalData = normalData
            };
            return r;
        }

        public static DayR GetDayR(P p)
        {
            ReadExcel rd = new ReadExcel();
            string filepath = Path.Combine("wwwroot", string.Format("{0}年后正常数据.xlsx", p.data));
            List<ExcelData> excelDatas = rd.ImportExcel(filepath);
            List<string> heartWarnData;
            List<string> breathWarnsData;
            List<string> coughJsonData;
            List<DateTime?> startSleepData;
            List<DateTime?> endSleepData;

            heartWarnData = excelDatas
                .Select(x => x.HeartWarnData)
                .ToList();
            breathWarnsData = excelDatas
                .Select(x => x.BreathWarnsData)
                .ToList();
            coughJsonData = excelDatas
                .Select(x => x.CoughJsonData)
                .ToList();
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

            if (p.status == 91)
            {
                dayCount = dc.GetDayCount(heartWarnData, endSleepData);
            }
            if (p.status == 92)
            {
                dayCount = dc.GetDayCount(breathWarnsData, endSleepData);
            }
            if (p.status == 93)
            {
                dayCount = dc.GetDayCount(coughJsonData, endSleepData, 1);
            }

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

            var r = new Statistical.PR.DayR
            {
                weekCount = new int[] { monday.Count, ortherWeek.Count, friday.Count, weekend.Count, holiday.Count }
            };
            return r;
        }

        public static MonthWarnR GetMonthR(P p)
        {
            ReadExcel rd = new ReadExcel();
            string filepath = Path.Combine("wwwroot", string.Format("{0}年后正常数据.xlsx", p.data));
            List<ExcelData> excelDatas = rd.ImportExcel(filepath);

            List<DateTime?> startSleepData;
            List<DateTime?> endSleepData;

            startSleepData = excelDatas
            .Select(x => x.StartSleepTime)
            .ToList();
            endSleepData = excelDatas
                .Select(x => x.EndSleepTime)
                .ToList();

            DataTools tools = new DataTools();
            ModuleTools mt = new ModuleTools();
            DataCount dc = new DataCount();
            CalculateData cd = new CalculateData();

            int dataStatus = p.status - 100;
            List<MonthCount> monthCount = dc.GetMonthTimeDataCount(endSleepData, tools.GetDataType(p.data, dataStatus));

            MonthWarnR r = new MonthWarnR();

            for (int i = 0; i < monthCount.Count; i++)
            {
                if (monthCount[i].Month == p.month)
                {
                    List<string> monthData = monthCount[i].MonthData;
                    List<int> monthTime = monthCount[i].MonthTime;
                    List<double> monthlist = dc.GetNewDayCount(monthData);

                    CutCalculateList monthClist = cd.CutCalculateData(monthlist, 4, 24);
                    List<double> LineDatas = dc.GetJsonDataCount(monthData);


                    if (dataStatus == 3)
                    {
                        monthlist = dc.GetNewDayCount(monthData, 1);
                        monthClist = cd.CutCalculateData(monthlist, 4, 24);
                    }
                    
                    List<double?> monthCAClist = monthClist.AmplitudeCList;

                    List<double> ttList = monthClist.TrendCList.Select(x => x.Value).ToList();

                    double[] arr = cd.DataPercentileInplace(LineDatas);
                    int[] normalData = { (int)Math.Round(arr[1]), (int)Math.Round(arr[2]) };
                    int[] abnormalData = { (int)Math.Round(arr[0]), (int)Math.Round(arr[3]) };

                    List<int> abPointData = new List<int>();
                    List<int> abPointTime = new List<int>();

                    for (int j = 0; j < LineDatas.Count; j++)
                    {
                        if (LineDatas[j] > abnormalData[1])
                        {
                            abPointData.Add((int)LineDatas[j]);
                            abPointTime.Add((int)monthTime[j]);
                        }
                    }

                    // MinuteR a = mt.BasicModule(monthlist, monthTime, 4, 24);

                    r = new MonthWarnR
                    {
                        LineData = ttList.ToArray(),
                        LineTime = monthTime.ToArray(),
                        PointData = LineDatas.ToArray(),
                        AbPointData = abPointData.ToArray(),
                        AbPointTime = abPointTime.ToArray(),
                        NormalData = normalData,
                        AbnormalData = abnormalData.ToArray()
                    };
                }
            }

            return r;

        }

        public static MinuteR GetMinuteR(P p) 
        {
            ReadExcel rd = new ReadExcel();
            string filepath = Path.Combine("wwwroot", string.Format("{0}年后正常数据.xlsx", p.data));
            List<ExcelData> excelDatas = rd.ImportExcel(filepath);

            DataCount dc = new DataCount();
            ModuleTools mt = new ModuleTools();
            CalculateData cd = new CalculateData();

            //   时间段模型  也是 每天 的模型  
            ExcelData excelData = excelDatas.Where(x => x.EndSleepTime.Value.Month == p.month && x.EndSleepTime.Value.Day == p.day).First();

            int dataStatus = p.status - 200;

            List<JsonData> jsonDatas1 = new List<JsonData>();

            if (dataStatus == 1)
            {
                jsonDatas1 = JsonConvert.DeserializeObject<List<JsonData>>(excelData.HeartWarnData);
            }
            else if (dataStatus == 2)
            {
               jsonDatas1 = JsonConvert.DeserializeObject<List<JsonData>>(excelData.BreathWarnsData);
            }
            else if (dataStatus == 3)
            {
                jsonDatas1 = JsonConvert.DeserializeObject<List<JsonData>>(excelData.CoughJsonData);
            }

            //  反序列化
            TimeCounts timeCounts = new TimeCounts();
            if (dataStatus == 3)
            {
                timeCounts = dc.GetMinuteTimeDataCount(jsonDatas1, excelData.StartSleepTime.Value, excelData.EndSleepTime.Value, 1);
            }
            else
            {
                timeCounts = dc.GetMinuteTimeDataCount(jsonDatas1, excelData.StartSleepTime.Value, excelData.EndSleepTime.Value);
            }

            List<double> minuteCount = timeCounts.Counts;
            List<DateTime> minuteTime = timeCounts.Times;

            // 调用模块 输入数据和时间  获取点线异常 等指标
            MinuteR r = mt.BasicModule(minuteCount, minuteTime, 5, 30);

            return r;

        }

    }
}


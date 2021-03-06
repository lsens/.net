﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    public class DataCount
    {

        //  接收 json格式  输出 次数  默认不传参status取0  咳嗽取 1
        public List<double> GetJsonDataCount(List<string> jsonData, int status = 0)
        {

            List<double> countList = new List<double>();

            for (int i = 0; i < jsonData.Count; i++)
            {
                //  反序列化
                List<JsonData> jsonDatas = JsonConvert.DeserializeObject<List<JsonData>>(jsonData[i]);
                double count = 0;

                if (status == 1)
                {
                    //  循环 咳嗽数据 取出 等于3 的状态  
                    if (jsonDatas.Count != 0)
                    {
                        for (int j = 0; j < jsonDatas.Count; j++)
                        {
                            if (jsonDatas[j].Status == 3)
                            {
                                count++;
                            }
                        }
                    }
                }
                else
                {
                    count = jsonDatas.Count;
                }
                countList.Add(count);
            }

            return countList;
        }

        public List<DayCount> GetDayCount(List<string> jsonData, List<DateTime?> DayTimes, int status = 0)
        {
            CalculateData calculateData = new CalculateData();
            List<DayWarnCount> countList = new List<DayWarnCount>();

            for (int i = 0; i < jsonData.Count; i++)
            {
                //  反序列化
                List<JsonData> jsonDatas = JsonConvert.DeserializeObject<List<JsonData>>(jsonData[i]);

                if (jsonDatas.Count != 0)
                {
                    string day = DayTimes[i].Value.ToShortDateString();
                    DayWarnCount dayWarnCount = new DayWarnCount(day, new double[24]);
                    int flag = 0;

                    for (int j = 0; j < jsonDatas.Count; j++)
                    {
                        //  两周   对应两天trend 后求 各个指标

                        //  构建一个 存储 每天  24 小时 的 异常 

                        //   7个  trend  求 a  w  p  

                        //  24小时 对应两时trend 后求 各个指标

                        //   6个  trend  求 a  w  p

                        //  存储   每个小时 异常出现的次数 

                        if (status == 1)
                        {
                            if (jsonDatas[j].Status == 3)
                            {
                                int hour = Convert.ToDateTime(jsonDatas[j].Time[0]).Hour;

                                dayWarnCount.TimeCount[hour]++;

                                flag = 1;
                            }
                        }
                        else
                        {
                            int hour = Convert.ToDateTime(jsonDatas[j].Time[0]).Hour;

                            dayWarnCount.TimeCount[hour]++;

                            flag = 1;
                        }
                    }
                    if (flag == 1)
                    {
                        countList.Add(dayWarnCount);
                    }
                }
            }

            List<DayCount> dayCounts = new List<DayCount>();

            List<double> hourList = new List<double>();
            for (int i = 0; i < countList.Count; i++)
            {
                hourList.AddRange(countList[i].TimeCount);
            }

            List<double?> hourCList = calculateData.CutCalculateData(hourList, 2, 24).AmplitudeCList;

            double[] hourCPList = calculateData.PercentileData(hourCList.Select(x => x.Value).ToList());

            for (int i = 0; i < hourCList.Count; i++)
            {
                if (hourCList[i] < hourCPList[0])
                {
                    DayCount dayCount = new DayCount
                    {
                        TimeCount = countList[i].TimeCount.Sum(),
                        DayTime = countList[i].DayTime
                    };
                    dayCounts.Add(dayCount);
                }
            }

            return dayCounts;
        }
        //   尝试用自定义类存储  时刻和对应时刻列表 入睡时间 
        public List<TimeAndListClass> GetTimeAndList(List<DateTime?> timeData)
        {
            List<double> timeList = new List<double>();
            List<TimeAndListClass> goSleepList = new List<TimeAndListClass>();

            int lenth = timeData.Count;

            //  判断异常  获取时间列表  后面队时间列表进行判断
            //  入睡情况下  00：00：00   是异常值
            for (int i = 0; i < lenth; i++)
            {
                if (Convert.ToDateTime(timeData[i]).ToLongTimeString() != "0:00:00")
                {
                    timeList.Add((Convert.ToDateTime(timeData[i]).Hour));
                }
                else
                {
                    timeList.Add(-1);
                }
            }

            string ttt = string.Join(",", timeList.ToArray());

            double? maxHour = timeList.Where(x => x >= 14).Min();
            double? minHout = timeList.Where(x => x < 14).Max();

            if (minHout != null)
            {
                for (int i = 23; i >= maxHour; i--)
                {
                    List<double> t = new List<double>();
                    TimeAndListClass timeClass = new TimeAndListClass(i, t);
                    goSleepList.Add(timeClass);
                }
            }

            if (maxHour != null)
            {
                for (int i = 0; i <= minHout; i++)
                {
                    List<double> t = new List<double>();
                    TimeAndListClass timeClass = new TimeAndListClass(i, t);
                    goSleepList.Add(timeClass);
                }
            }

            for (int j = 0; j < lenth; j++)
            {
                if (timeList[j] != -1)
                {
                    var goSleep = goSleepList.Where(x => x.TimeHour == timeList[j]).FirstOrDefault();
                    goSleep.TimeList.Add(1);
                    List<TimeAndListClass> otherList = goSleepList.Where(x => x.TimeHour != timeList[j]).ToList();
                    for (int k = 0; k < otherList.Count; k++)
                    {
                        otherList[k].TimeList.Add(0);
                    }
                }
                else
                {
                    for (int k = 0; k < goSleepList.Count; k++)
                    {
                        goSleepList[k].TimeList.Add(-1);
                    }
                }
            }

            TimeAndListClass allTimeList = new TimeAndListClass(24, timeList);
            goSleepList.Add(allTimeList);

            return goSleepList;
        }
        //   每月 按天 函数处理 
        public List<double> GetNewDayCount(List<string> jsonData, int status = 0)
        {

            List<double> hourList = new List<double>();

            for (int i = 0; i < jsonData.Count; i++)
            {
                //  反序列化
                List<JsonData> jsonDatas = JsonConvert.DeserializeObject<List<JsonData>>(jsonData[i]);

                double[] dayWarnCount = new double[24];

                if (jsonDatas.Count != 0)
                {
                    for (int j = 0; j < jsonDatas.Count; j++)
                    {
                        if (status == 1)
                        {
                            if (jsonDatas[j].Status == 3)
                            {
                                int hour = Convert.ToDateTime(jsonDatas[j].Time[0]).Hour;

                                dayWarnCount[hour]++;
                            }
                        }
                        else
                        {
                            int hour = Convert.ToDateTime(jsonDatas[j].Time[0]).Hour;
                            dayWarnCount[hour]++;
                        }
                    }
                }
                hourList.AddRange(dayWarnCount);

            }
            
            return hourList;

        }
        //  获取时间和数据  进行切割 按月份切割
        public List<MonthCount> GetMonthTimeDataCount(List<DateTime?> times, List<string> datas)
        {
            List<MonthCount> monthCounts = new List<MonthCount>();
            int length = times.Count();

            MonthCount monthCount = new MonthCount
            {
                Year = times[0].Value.Year,
                Month = times[0].Value.Month,
                MonthData = new List<string>(),
                MonthTime = new List<int>(),
            };

            for (int i = 0; i < length; i++)
            {
                if (times[i].Value.Year == monthCount.Year && times[i].Value.Month == monthCount.Month)
                {
                    monthCount.MonthTime.Add(times[i].Value.Day);
                    monthCount.MonthData.Add(datas[i]);
                }
                else
                {
                    monthCounts.Add(monthCount);
                    monthCount = new MonthCount
                    {
                        Year = times[i].Value.Year,
                        Month = times[i].Value.Month,
                        MonthData = new List<string>(),
                        MonthTime = new List<int>(),
                    };
                    monthCount.MonthTime.Add(times[i].Value.Day);
                    monthCount.MonthData.Add(datas[i]);
                }
            }
            monthCounts.Add(monthCount);
            return monthCounts;
        }
        //  获取时间和数据  进行切割 按分钟切割
        public TimeCounts GetMinuteTimeDataCount(List<JsonData> datas, DateTime startTime, DateTime endTime, int statu = 0)
        {
            int i = 0;
            List<double> counts = new List<double>();
            List<DateTime> minutes = new List<DateTime>();

            List<JsonData> cData = new List<JsonData>();

            if (statu == 1)
            {
                cData = datas.Where(x => x.Status == 1).ToList();
            }
            else
            {
                cData = datas;
            }

            while (startTime < endTime)
            {
                minutes.Add(startTime);
                counts.Add(0);
                startTime = startTime.AddMinutes(1);

                while (i < cData.Count)
                {
                    string time = cData[i].Time[0];
                    DateTime dateTime = Convert.ToDateTime(time);
                    if (dateTime.TimeOfDay < startTime.TimeOfDay)
                    {
                        counts[^1]++;
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            TimeCounts timeCount = new TimeCounts
            {
                Counts = counts,
                Times = minutes
            };
            return timeCount;
        }
        //  获取睡眠分期数据  计算每天的深睡时长
        public List<double> GetDeepSleepTimeData(List<string> jsonData)
        {
            List<double> timeList = new List<double>();

            for (int i = 0; i < jsonData.Count(); i++)
            {
                //  反序列化
                List<OneJsonData> jsonDatas = JsonConvert.DeserializeObject<List<OneJsonData>>(jsonData[i]);
                if (jsonData.Count != 0 && jsonDatas.FirstOrDefault(x => x.Status == 0) != null)
                {
                    double deepTime = TimeSpan.Parse(jsonDatas.FirstOrDefault(x => x.Status == 0).Time).TotalMinutes;
                    timeList.Add(deepTime);
                }
                else
                {
                    timeList.Add(0);
                }
            }
            return timeList;
        }
           
    }
}


using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MathNet.Numerics.Statistics;

namespace Data
{
    public class CalculateData
    {
        //  底层处理数据模型
        public CutCalculateList CutCalculateData(List<double> datas, int cWindow = 2, int aWindow = 14, int statu = 0)
        {
            WaveCalculator waveCalculator = new WaveCalculator();

            CutCalculateList cutCalculateList = new CutCalculateList();

            List<double> trendList = new List<double>();
            //  空值 处理  后面再写  
            List<double?> amplitudeCList = new List<double?>();
            List<double?> trendCList = new List<double?>();
            List<double?> amplitudeAList = new List<double?>();

            double[] arr;
            int a = 0;
            int b = 0;
            List<double> cutData = new List<double>();

            while ((datas.Count / cWindow) > b)
            {
                a++;
                arr = datas.GetRange(b * cWindow, cWindow).ToArray();
                if (statu == 1)
                {
                    //  入睡和起床时间  -1  是异常 不处理
                    arr = arr.Where(x => x >= 0).ToArray();
                }
                else if (statu == 2)
                {
                    //  睡眠时长   睡眠时间 太短 直接忽略 
                    arr = arr.Where(x => x > 2).ToArray();

                }
                else if (statu == 3)
                {
                    //  入睡时长  0是 异常数据 不处理
                    arr = arr.Where(x => x > 0).ToArray();

                }

                cutData.AddRange(arr.ToList());


                double? trend = waveCalculator.GetTrend(arr);

                if (trend != null) { trendList.Add(trend.Value); }

                if (a == aWindow / cWindow)
                {
                    double? amplitudeC = waveCalculator.GetAmplitude(trendList.ToArray());
                    double? trendC = waveCalculator.GetTrend(trendList.ToArray());
                    amplitudeCList.Add(amplitudeC);
                    trendCList.Add(trendC);

                    double? amplitudeA = waveCalculator.GetAmplitude(cutData.ToArray());
                    amplitudeAList.Add(amplitudeA);

                    cutData.Clear();

                    trendList.Clear();

                    a = 0;
                }
                
                b++;
            }

            cutCalculateList.AmplitudeCList = amplitudeCList;
            cutCalculateList.TrendCList = trendCList;
            cutCalculateList.AmplitudeAList = amplitudeAList;

            return cutCalculateList;

        }

        //  使用Math.net 获取数据的 离群点  适用于常量 短程数据分析   
        public double[] DataPercentileInplace(List<double> datas)
        {
            double[] newDatas = datas.ToArray();

            double q1 = ArrayStatistics.LowerQuartileInplace(newDatas);
            double q3 = ArrayStatistics.UpperQuartileInplace(newDatas);

            double iqr = q3 - q1;
            if (iqr < 3)
            {
                iqr = 3;
            }

            double arrLower = q1 - 1.5 * (iqr);
            double arrUpper = q3 + 1.5 * (iqr);

            double[] arr = {
                arrLower,
                q1,
                q3,
                arrUpper,
            };
            
            return arr;
        }

        //  正负分开进行判断 
        public double[] PercentileDataTwo(List<double?> adatas)
        {
            List<double> apercentileDatas = new List<double>();
            List<double> ndatas = new List<double>();
            List<double> datas = new List<double>();

            for (int i = 0; i < adatas.Count; i++)
            {
                if (adatas[i] < 0)
                {
                    ndatas.Add(adatas[i].Value);
                }
                else
                {
                    datas.Add(adatas[i].Value);
                }
            }

            if (datas.Count > 0)
            {
                double[] percentileDatas = PercentileData(datas);
                apercentileDatas.AddRange(percentileDatas);
            }
            if (ndatas.Count > 0)
            {
                double[] percentileDatas = PercentileData(ndatas);
                apercentileDatas.AddRange(percentileDatas);
            }

            double[] pDatas = apercentileDatas.ToArray();
            return pDatas;
        }

        //  数据分箱 取 5 25 75 95 4个位置的值
        public double[] PercentileData(List<double> datas)
        {
            double[] newDatas = datas.ToArray();

            double percentile5Data = ArrayStatistics.PercentileInplace(newDatas, 5);
            double percentile25Data = ArrayStatistics.PercentileInplace(newDatas, 25);
            double percentile75Data = ArrayStatistics.PercentileInplace(newDatas, 75);
            double percentile95Data = ArrayStatistics.PercentileInplace(newDatas, 95);

            double[] percentileDatas = new double[]
            {
                percentile5Data,
                percentile25Data,
                percentile75Data,
                percentile95Data
            };

            return percentileDatas;
        }

        //   输入 时间窗口  输出切割后的时间  数据进来就进行切割  具体 空值 处理 后面再写 
        public List<StartEndTime> CutTimes(List<DateTime?> times, int aWindow = 14)
        {
            List<StartEndTime> startEndTimes = new List<StartEndTime>();
            int a = 0;
            while ((times.Count / aWindow) > a)
            {
                StartEndTime startEndTime = new StartEndTime
                {
                    StartTime = times[a * aWindow].Value,
                    EndTime = times[(a + 1) * aWindow - 1].Value
                };
                startEndTimes.Add(startEndTime);
                a++;
            }
            return startEndTimes;
        }

        /// <summary>
        /// 大类的封装  输入时间和数据直接获取点线值 
        /// </summary>
        /// <param name="goSleepTimeList"></param>
        /// <param name="endSleepData"></param>
        /// <param name="cWindow"></param>
        /// <param name="aWindow"></param>
        /// <returns></returns>

        //   入睡时间模型  ps：起床时间模型    ok  ok  ok  异常问题
        //   传入入睡时间  和切割后的时间    输出趋势线的点以及对应的时间段  以及异常点和异常时间  
        public SleepTimeLineAndPoint GetSleepTimeLineAndPoint(List<TimeAndListClass> goSleepTimeList, List<DateTime?> endSleepData, int cWindow = 2, int aWindow = 14)
        {

            WaveCalculator waveCalculator = new WaveCalculator();

            CalculateData calculateData = new CalculateData();

            SleepTimeLineAndPoint sleepTimeLineAndPoint = new SleepTimeLineAndPoint();

            List<Point> gSCCList = new List<Point>();

            List<List<double?>> gSCTLL = new List<List<double?>>();

            for (int i = 0; i < goSleepTimeList.Count - 1; i++)
            {
                List<double> gSL = new List<double>();
                gSL = goSleepTimeList[i].TimeList;

                //  切割 取 trend 值
                List<double?> gSCL = calculateData.CutCalculateData(gSL, cWindow, aWindow, 1).TrendCList;
                double? gSCCX = waveCalculator.GetTrend(gSCL.Select(x => x.Value).ToArray());
                Point gSCC = new Point(goSleepTimeList[i].TimeHour, gSCCX.Value);
                gSCCList.Add(gSCC);

                // 循环 取时刻列表
                for (int j = 0; j < gSCL.Count; j++)
                {
                    if (i == 0)
                    {
                        List<double?> gSCTL = new List<double?> { gSCL[j] };
                        gSCTLL.Add(gSCTL);
                    }
                    else
                    {
                        gSCTLL[j].Add(gSCL[j]);
                    }
                }
            }

            //  3次trand后的分箱结果;
            double[] gSCCPList = calculateData.PercentileData(gSCCList.Select(x => x.Y).ToList());

            List<double> normalData = new List<double>();
            List<int> pointData = new List<int>();
            List<string> pointTime = new List<string>();

            // 取正常值范围和异常点的值 
            List<double> abnormalData = new List<double>();
            for (int i = 0; i < gSCCList.Count; i++)
            {
                if ((gSCCList[i].Y == gSCCPList[3]) || (gSCCList[i].Y == gSCCPList[2]))
                {
                    normalData.Add(gSCCList[i].X);
                }
                if ((gSCCPList[0] != 0 && gSCCList[i].Y == gSCCPList[0]) || (gSCCPList[1] != 0 && gSCCList[i].Y == gSCCPList[1]))
                {
                    abnormalData.Add(gSCCList[i].X);
                }
            }

            // 如果 取不到 5 % 和 % 25  就 取 最小的 数值

            if (abnormalData.Count == 0)
            {
                double minHour = gSCCList.OrderBy(x => x.Y).Where(x => x.Y != 0).FirstOrDefault().Y;
                abnormalData.Add(minHour);
            }

            List<double> allTimeList = goSleepTimeList.Last().TimeList;

            pointData.Add((int)abnormalData[0]);

            for (int i = 0; i < allTimeList.Count(); i++)
            {
                if (abnormalData[0] == allTimeList[i])
                {
                    pointTime.Add(endSleepData[i].Value.ToShortDateString());
                }
                if (pointTime.Count == 5)
                {
                    break;
                }
            }

            sleepTimeLineAndPoint.NormalData = normalData;
            sleepTimeLineAndPoint.AbnormalData = abnormalData;
            sleepTimeLineAndPoint.PointData = pointData;
            sleepTimeLineAndPoint.PointTime = pointTime;

            //  求 趋势线的 点 和周期
            List<StartEndTime> timeData = calculateData.CutTimes(endSleepData, aWindow);
            List<int> lineData = new List<int>();
            List<string> lineTime = new List<string>();
            for (int i = 0; i < gSCTLL.Count; i++)
            {
                double? maxData = gSCTLL[i].Max();

                int maxIndex = gSCTLL[i].FindIndex(x => x == maxData);

                int Data = goSleepTimeList[maxIndex].TimeHour;

                string time = string.Format("{0},{1}", timeData[i].StartTime.ToShortDateString(), timeData[i].EndTime.ToShortDateString());

                lineData.Add(Data);
                lineTime.Add(time);
            }

            sleepTimeLineAndPoint.LineData = lineData;
            sleepTimeLineAndPoint.LineTime = lineTime;

            return sleepTimeLineAndPoint;

        }

        //  短时间入睡分析判断  
        public void GetGoSleepStatus(List<DateTime> times)
        {
            double length = times.Count;

            List<HourCount> hourCounts = new List<HourCount>();

            for (int i = 0; i < length; i++)
            {
                if (hourCounts.Exists(x => x.Time == times[i].Hour))
                {
                    HourCount hourCount = hourCounts.FirstOrDefault(x => x.Time == times[i].Hour);
                    hourCount.Count++;
                }
                else
                {
                    HourCount hourCount = new HourCount(times[i].Hour, 1);
                    hourCounts.Add(hourCount);
                }
            }

            int? oneTime = null;
            List<int> twoTime = new List<int>();

            for (int i = 0; i < hourCounts.Count; i++)
            {
                if (hourCounts[i].Count / length > 0.5)
                {
                    oneTime = hourCounts[i].Time;
                }
                else if (hourCounts[i].Count / length > 0.27)
                {
                    twoTime.Add(hourCounts[i].Time);
                }
            }
            
            //  24 点和 第二天


            List<DateTime> AbnormalTime = new List<DateTime>();
            List<int> NormalData = new List<int>();

            if (oneTime != null)
            {
                for (int i = 0; i < length; i++)
                {
                    int poor = Math.Abs(times[i].Hour - oneTime.Value);
                    if (poor >= 2 && poor < 12)
                    {
                        AbnormalTime.Add(times[i]);
                    }
                    else if (poor <= 22 && poor >= 12)
                    {
                        AbnormalTime.Add(times[i]);
                    }
                }
                NormalData.Add(oneTime.Value);
            }
            else if (twoTime.Count == 2)
            {
                twoTime.Sort();
                int poor = twoTime[1] - twoTime[0];
                if (poor == 1)
                {
                    double t1 = twoTime[0] + 0.5;
                    for (int i = 0; i < length; i++)
                    {
                        double t2 = times[i].Hour + times[i].Minute / 60;
                        double poor2 = t2 - t1;
                        if (poor2 >= 2 && poor2 < 12)
                        {
                            AbnormalTime.Add(times[i]);
                        }
                        else if (poor2 <= 22 && poor2 >= 12)
                        {
                            AbnormalTime.Add(times[i]);
                        }
                    }

                    NormalData.AddRange(twoTime);
                }
                else if (poor == 23)
                {
                    double t1 = 23.5;
                    for (int i = 0; i < length; i++)
                    {
                        double t2 = times[i].Hour + times[i].Minute / 60;
                        double poor2 = t2 - t1;
                        if (poor2 >= 2 && poor2 < 12)
                        {
                            AbnormalTime.Add(times[i]);
                        }
                        else if (poor2 <= 22 && poor2 >= 12)
                        {
                            AbnormalTime.Add(times[i]);
                        }
                    }

                    NormalData.Add(twoTime[1]);
                    NormalData.Add(twoTime[0]);
                }

            }
            
            List<int> LineData = new List<int>();
            List<string> LineTime = new List<string>();

            LineData = times.Select(x => x.Hour).ToList();
            LineTime = times.Select(x => x.ToShortDateString()).ToList();

        }
        
    }
}



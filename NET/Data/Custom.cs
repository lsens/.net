using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    //  构建映射ex表格的数据 对应的 类 
    public class ExcelData
    {
        public DateTime? StartSleepTime { get; set; }
        public DateTime? EndSleepTime { get; set; }
        public double? SleepTime { get; set; }
        public string BreathWarnsData { get; set; }
        public string HeartWarnData { get; set; }
        public string CoughJsonData { get; set; }
        public string PeriodJsonData { get; set; }
        public string PeriodData { get; set; }
    }
    
    //  多时间json数据
    public class JsonData
    {
        public int Status { get; set; }
        public string[] Time { get; set; }

    }

    //  单时间json数据
    public class OneJsonData
    {
        public int Status { get; set; }
        public string Time { get; set; }

    }

    //   构建  开始时间和结束时间类
    public class StartEndTime
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    //  构建  存储分箱数据和对应时间段的 列表
    public class PercentileDataTime
    {
        public double[] PercentileDatas { get; set; }
        public string[] PercentileTimes { get; set; }
    }

    // 构建   一个 存储时间  和 时间列表的  类
    public class TimeAndListClass
    {
        public int TimeHour { get; set; }
        public List<double> TimeList { get; set; }

        public TimeAndListClass(int timeHour, List<double> timeList)
        {
            TimeHour = timeHour;
            TimeList = timeList;
        }
    }

    //  构建   存储 切割后  各个函数计算结果的  列表 的类
    public class CutCalculateList
    {
        public List<double?> AmplitudeCList { get; set; }
        public List<double?> TrendCList { get; set; }
        public List<double?> AmplitudeAList { get; set; }
    }

    //   构建  存储 入睡时间模型的  类  4个 字段
    public class SleepTimeLineAndPoint
    {
        public List<int> LineData { get; set; }
        public List<string> LineTime { get; set; }
        public List<int> PointData { get; set; }
        public List<string> PointTime { get; set; }
        public List<double> NormalData { get; set; }
        public List<double> AbnormalData { get; set; }

    }

    //  构建  存储  每天各个时间段 出现异常 的次数 的    类
    public class DayWarnCount
    {
        public string DayTime { get; set; }
        public double[] TimeCount { get; set; }
        public DayWarnCount(string dayTime, double[] timeCount)
        {
            DayTime = dayTime;
            TimeCount = timeCount;
        }
    }

    //  构建  每天出现异常 的时间点  和 对应的 次数 类 
    public class DayCount
    {
        public string DayTime { get; set; }
        public double TimeCount { get; set; }
    }
    //  X Y 结构的 类似point 的类
    public class Point
    {
        public int X { get; set; }
        public double Y { get; set; }
        public Point(int x, double y)
        {
            X = x;
            Y = y;
        }
    }
    //   构建 存储每个月的异常数据 
    public class MonthCount
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<DateTime> MonthTime { get; set; }
        public List<string> MonthData { get; set; }

    }
    //  构建  出现异常 的时间点 和 对应的  次数 的数组 类
    public class TimeCounts
    {
        public List<DateTime> Times { get; set; }
        public List<double> Counts { get; set; }
    }

}

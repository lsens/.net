using System;
using System.Collections.Generic;
using System.Text;

namespace Statistical.PR
{
    // 通用返回模型
    public class R
    {
        // 趋势线点和线
        public double[] LineData { get; set; }
        public string[] LineTime { get; set; }
        // 点的数据
        public double[] PointData { get; set; }
        // 正常指标范围
        public double[] NormalData { get; set; }
        // 异常指标上下边界
        public double[] AbnormalData { get; set; }
        // 异常点和时间
        public double[] AbPointData { get; set; }
        public string[] AbPointTime { get; set; }
    }
}

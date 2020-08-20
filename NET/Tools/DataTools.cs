using Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace Tools
{
    public class DataTools
    {
        // 根据请求的数据类型 进行判断 
        public List<string> GetDataType(string name, int dataStatus)
        {
            ReadExcel rd = new ReadExcel();
            List<ExcelData> excelDatas = rd.ImportExcel(name);

            if (dataStatus == 1)
            {
                List<string> heartWarnData;
                heartWarnData = excelDatas
                    .Select(x => x.HeartWarnData)
                    .ToList();
                return heartWarnData;
            }
            if (dataStatus == 2)
            {
                List<string> breathWarnsData;
                breathWarnsData = excelDatas
                    .Select(x => x.BreathWarnsData)
                    .ToList();
                return breathWarnsData;
            }
            if (dataStatus == 3)
            {

                List<string> coughJsonData;
                coughJsonData = excelDatas
                    .Select(x => x.CoughJsonData)
                    .ToList();
                return coughJsonData;
            }
            else
            {
                return null;
            }
        }

        // 输入ampC 数据 进行分箱  求出正值的95点和负值的95 分箱值 
        public List<double?> GetAmpC95(List<double?> ampC)
        {
            CalculateData calculateData = new CalculateData();
            double[] ampCAll = calculateData.PercentileDataTwo(ampC);
            double? ampC95 = null;
            double? nampC95 = null;
            if (ampCAll.Length > 4)
            {
                ampC95 = ampCAll[3];
                nampC95 = ampCAll[4];
            }
            else
            {
                if (ampC[0] >= 0)
                {
                    ampC95 = ampCAll[3];
                }
                else
                {
                    nampC95 = ampCAll[0];
                }
            }

            List<double?> ampCTwo = new List<double?>
            {
                ampC95,
                nampC95
            };

            return ampCTwo;
        }

        // 输入时间跨度 获取大小窗口  
        // 1  天 包括多小时  最少测量2小时 5分钟小窗口  30分钟大窗口  窗口仅为辅助计算工具 
        // 2  月 包括多天    最少测量7天   2小时小窗口  24小时大窗口
        // 3  年 包括多月    最少测量3月   2天为小窗口  14天为大窗口
        public int[] GetWindow(int timeSpan) 
        {
            int[] timeWindows = new int[2];
            if (timeSpan == 1)
            {
                timeWindows[0] = 5;
                timeWindows[1] = 30;
            }
            if (timeSpan == 2)
            {
                timeWindows[0] = 2;
                timeWindows[1] = 24;
            }
            if (timeSpan == 3)
            {
                timeWindows[0] = 2;
                timeWindows[1] = 14;
            }

            return timeWindows;
        }
    }
}

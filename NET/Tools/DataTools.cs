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
            string filepath = Path.Combine("wwwroot", string.Format("{0}年后正常数据.xlsx", name));
            List<ExcelData> excelDatas = rd.ImportExcel(filepath);

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
            double[] ampCAll = calculateData.PercentileData8(ampC);
            double? ampC95 = null;
            double? nampC95 = null;
            if (ampCAll.Length > 4)
            {
                ampC95 = ampCAll[3];
                nampC95 = ampCAll[7];
            }
            else
            {
                if (ampC[0] >= 0)
                {
                    ampC95 = ampCAll[3];
                }
                else
                {
                    nampC95 = ampCAll[3];
                }
            }

            List<double?> ampCTwo = new List<double?>
            {
                ampC95,
                nampC95
            };

            return ampCTwo;
        }

    }
}

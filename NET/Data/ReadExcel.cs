using System;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;


namespace Data
{
    public class ReadExcel
    {
        //  对读取的 ex  数据进行出来 生成 double 类型
        private double GetDoubleValue(ICell cell)
        {
            if (cell == null)
            {
                return 0;
            }

            double errorValue = 0;
            switch (cell.CellType)
            {
                case CellType.Numeric:
                    return cell.NumericCellValue;

                case CellType.String:
                    double d;
                    if (double.TryParse(cell.StringCellValue, out d))
                    {
                        return d;
                    }
                    return errorValue;

                case CellType.Boolean:
                case CellType.Formula:
                case CellType.Blank:
                case CellType.Error:
                case CellType.Unknown:
                default:
                    return errorValue;
            }
        }

        //  读取数据  
        public List<ExcelData> ImportExcel(string name)
        {
            string filePath = Path.Combine("wwwroot", string.Format("{0}年后正常数据.xlsx", name));

            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            IWorkbook workbook;
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                string extension = Path.GetExtension(filePath).ToLower();
                switch (extension)
                {
                    case ".xls":
                        workbook = new HSSFWorkbook(fileStream);
                        break;
                    case ".xlsx":
                        workbook = new XSSFWorkbook(fileStream);
                        break;
                    default:
                        return null;
                }
            }
            
            if (workbook == null || workbook.NumberOfSheets == 0)
            {
                return null;
            }

            ISheet sheet = workbook.GetSheetAt(0);
            if (sheet == null)
            {
                return null;
            }

            int lastRowNum = sheet.LastRowNum;

            List<ExcelData> excelDatas = new List<ExcelData>();

            ExcelData excelData = new ExcelData
            {
                StartSleepTime = null,
                EndSleepTime = null,
                SleepTime = 0,
                BreathWarnsData = "[]",
                HeartWarnData = "[]",
                CoughJsonData = "[]",
                PeriodJsonData = "[]",
                PeriodData = "[]",
            };

            for (int i = 1; i < lastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);

                if (row == null)
                {
                    continue;
                }

                if (excelData.EndSleepTime != null)
                {
                    IRow prevRow = sheet.GetRow(i - 1);

                    DateTime start = Convert.ToDateTime(prevRow.GetCell(2).StringCellValue).Date;
                    DateTime end = Convert.ToDateTime(row.GetCell(2).StringCellValue).Date;

                    TimeSpan sp = end.Subtract(start);
                    //  对时间进行判断  取第二天时间 同一天则取两天睡眠时长的最大值 跳出循环 隔天则继续 
                    //  相差多少天则补多少天的空值  最后一天则直接继续 
                    //  取空类  
                    //  取值类
                    
                    if (sp.Days == 0)
                    {
                        double? prevSleepTime = GetDoubleValue(prevRow.GetCell(3));
                        double? sleepTime = GetDoubleValue(row.GetCell(3));

                        if (prevSleepTime > sleepTime)
                        {
                            row = prevRow;
                        }

                        excelData = new ExcelData
                        {
                            StartSleepTime = Convert.ToDateTime(row.GetCell(1).StringCellValue),
                            EndSleepTime = Convert.ToDateTime(row.GetCell(2).StringCellValue),
                            SleepTime = GetDoubleValue(row.GetCell(3)),
                            BreathWarnsData = row.GetCell(9).StringCellValue,
                            HeartWarnData = row.GetCell(12).StringCellValue,
                            CoughJsonData = row.GetCell(10).StringCellValue,
                            PeriodJsonData = row.GetCell(15).StringCellValue,
                            PeriodData = row.GetCell(14).StringCellValue,
                        };
                        continue;
                    }

                    else if (sp.Days > 1)
                    {

                        excelDatas.Add(excelData);

                        for (int j = 1; j < sp.Days; j++)
                        {
                            string eDate = Convert.ToDateTime(excelData.EndSleepTime).AddDays(j).ToString("yyyy-MM-dd");
                            string sDate = Convert.ToDateTime(excelData.StartSleepTime).AddDays(j).ToString("yyyy-MM-dd");

                            ExcelData ed = new ExcelData
                            {
                                StartSleepTime = null,
                                EndSleepTime = null,
                                SleepTime = 0,
                                BreathWarnsData = "[]",
                                HeartWarnData = "[]",
                                CoughJsonData = "[]",
                                PeriodJsonData = "[]",
                                PeriodData = "[]",
                            };

                            ed.EndSleepTime = Convert.ToDateTime(eDate += " 00:00:01");
                            ed.StartSleepTime = Convert.ToDateTime(sDate += " 00:00:00");
                            excelDatas.Add(ed);
                        }
                    }

                    else if (sp.Days == 1)
                    {
                        excelDatas.Add(excelData);
                    }
                    excelData = new ExcelData
                    {
                        StartSleepTime = Convert.ToDateTime(row.GetCell(1).StringCellValue),
                        EndSleepTime = Convert.ToDateTime(row.GetCell(2).StringCellValue),
                        SleepTime = GetDoubleValue(row.GetCell(3)),
                        BreathWarnsData = row.GetCell(9).StringCellValue,
                        HeartWarnData = row.GetCell(12).StringCellValue,
                        CoughJsonData = row.GetCell(10).StringCellValue,
                        PeriodJsonData = row.GetCell(15).StringCellValue,
                        PeriodData = row.GetCell(14).StringCellValue,
                    };
                }

                else
                {
                    excelData = new ExcelData
                    {
                        StartSleepTime = Convert.ToDateTime(row.GetCell(1).StringCellValue),
                        EndSleepTime = Convert.ToDateTime(row.GetCell(2).StringCellValue),
                        SleepTime = GetDoubleValue(row.GetCell(3)),
                        BreathWarnsData = row.GetCell(9).StringCellValue,
                        HeartWarnData = row.GetCell(12).StringCellValue,
                        CoughJsonData = row.GetCell(10).StringCellValue,
                        PeriodJsonData = row.GetCell(15).StringCellValue,
                        PeriodData = row.GetCell(14).StringCellValue,
                    };
                }
            }

            return excelDatas;

        }

    }
}

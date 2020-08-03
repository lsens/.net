using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Data
{
    class WaveCalculator
    {
        public double? GetTrend(double[] arr)
        {
            double? result;
            if (arr.Length > 0)
            {
                result = arr.Sum() / arr.Length;
            }
            else
            {
                result = null;
            }
            return result;
        }

        public double? GetAmplitude(double[] arr)
        {
            int length = arr.Length;
            double arrMax = arr.Max();
            double arrMin = arr.Min();
            List<int> maxList = new List<int>();
            List<int> minList = new List<int>();

            for (int i = 0; i < length; i++)
            {
                if (arrMax == arr[i])
                {
                    maxList.Add(i);
                }
                if (arrMin == arr[i])
                {
                    minList.Add(i);
                }
            }

            double? result;
            if (maxList.Count > 1 && minList.Count > 1)
            {
                result = arr[length - 1] - arr[0];
            }
            else
            {
                int maxId = Math.Min(maxList[0], length - 1 - maxList[^1]);
                if (maxId != maxList[0])
                {
                    maxId = maxList[^1];
                }
                int minId = Math.Min(minList[0], length - 1 - minList[^1]);
                if (minId != minList[0])
                {
                    minId = minList[^1];
                }

                if (maxId > minId)
                {
                    result = arrMax - arrMin;
                }
                else
                {
                    result = arrMin - arrMax;
                }
            }
            return result;
        }
    }
}

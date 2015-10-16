using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RasterizeCsharp.AppUtils;
using RasterizeCsharp.ZonalStatistics;

namespace RasterizeCsharp.ZonalIO
{
    class StatisticsExport
    {
        private static string _csvFileName;
        public StatisticsExport(string csvFileName)
        {
            _csvFileName = csvFileName;
        }

        public void ExportZonalStatistics(ref Dictionary<int, List<double>> zonalValues, double cellSize)
        {
            Console.WriteLine("Exporting zonal statistics in csv...");
            using (var w = new StreamWriter(_csvFileName))
            {
                string csvHeader = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", "VALUE", "COUNT", "AREA", "MIN", "MAX", "RANGE", "MEAN", "STDEV", "SUM", "VARIETY", "MAJORITY", "MINORITY", "MEDIAN");
                w.WriteLine(csvHeader);
                w.Flush();

                foreach (KeyValuePair<int, List<double>> zoneValue in zonalValues)
                {
                    try
                    {
                        //zonal variables
                        double value = zoneValue.Key;
                        var count = zoneValue.Value.Count;

                        var area = zoneValue.Value.Count * cellSize * cellSize;

                        var min = zoneValue.Value.Min(v => Convert.ToDouble(v));
                        var max = zoneValue.Value.Max(v => Convert.ToDouble(v));
                        var range = max - min;
                        var mean = zoneValue.Value.Average(avg => Convert.ToDouble(avg));
                        var stdev = Math.Sqrt(zoneValue.Value.Average(z => z * z) - Math.Pow(zoneValue.Value.Average(), 2));
                        var sum = zoneValue.Value.Sum(v => Convert.ToDouble(v));
                        var variety = zoneValue.Value.Distinct().Count();

                        IEnumerable<double> sortedValueOccurance = zoneValue.Value.GroupBy(i => i).OrderByDescending(g => g.Count()).Select(g => g.Key);
                        List<double> valueOccurance = sortedValueOccurance.ToList();
                        var majority = valueOccurance.First();
                        var minority = valueOccurance.Last();

                        var median = GetMedian(valueOccurance);

                        var csvLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", value, count, area, min, max, range, mean, stdev, sum, variety, majority, minority, median);

                        w.WriteLine(csvLine);
                    }
                    catch (Exception ex)
                    {
                        new CustomExceptionHandler("Failed to export zonal statistics 'ExportZonalStatistics'", ex);
                    }
                    finally
                    {
                        w.Flush();
                    }

                }
            }
        }

        private static double GetMedian(List<double> sortedArray)
        {
            int count = sortedArray.Count();
            int itemIndex = count / 2;
            if (count % 2 == 0) // Even number of items. 
                return (sortedArray.ElementAt(itemIndex) +
                        sortedArray.ElementAt(itemIndex - 1)) / 2;

            // Odd number of items. 
            return sortedArray.ElementAt(itemIndex);
        }

        public void ExportZonalStatistics2(ref Dictionary<int, StatisticsInfo>[] zonalValues, double cellSize)
        {
            Console.WriteLine("Exporting zonal statistics in csv...");
            using (var w = new StreamWriter(_csvFileName))
            {
                string csvHeader = string.Format("{0},{1},{2},{3},{4},{5}", "BAND", "VALUE", "COUNT", "AREA", "MEAN", "SUM");
                w.WriteLine(csvHeader);
                w.Flush();

                for (int bandCount = 0; bandCount < zonalValues.Length; bandCount++)
                {
                    foreach (KeyValuePair<int, StatisticsInfo> zonevalue in zonalValues[bandCount])
                    {
                        try
                        {
                            int value = zonevalue.Key;
                            double count = zonevalue.Value.Count;
                            double sum = zonevalue.Value.Sum;
                            double mean = sum / count;
                            int bandInfo = bandCount + 1;
                            double area = count * cellSize* cellSize;
                            var csvLine = string.Format("{0},{1},{2},{3},{4},{5}", bandInfo, value, count, area, mean, sum);

                            w.WriteLine(csvLine);
                        }
                        catch (Exception ex)
                        {
                            new CustomExceptionHandler("Failed to export zonal statistics 'ExportZonalStatistics'", ex);
                        }
                        finally
                        {
                            w.Flush();
                        }
                    }
                }
            }
        }


    }
}

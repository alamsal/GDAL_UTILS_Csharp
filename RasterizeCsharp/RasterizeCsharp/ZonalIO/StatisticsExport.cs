using System;
using System.Collections.Generic;
using System.IO;
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

        public void ExportZonalStatistics(ref Dictionary<int, StatisticsInfo>[] zonalValues, double cellSize)
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

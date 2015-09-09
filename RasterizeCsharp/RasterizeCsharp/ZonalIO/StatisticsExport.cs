using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RasterizeCsharp.ZonalIO
{
    class StatisticsExport
    {
        private static string _csvFileName;
        public StatisticsExport(string csvFileName)
        {
            _csvFileName = csvFileName;
        }

        public void WriteZonalStatistics(ref Dictionary<int, List<double>> zonalValues)
        {
            Console.WriteLine("Exporting zonal statistics in csv...");
            using(var w = new StreamWriter(_csvFileName))
            {
                string csvHeader = string.Format("{0},{1},{2},{3},{4},{5},{6}","VALUE","COUNT","MIN","MAX","MEAN","SUM","VARIETY");
                w.WriteLine(csvHeader);

                foreach (KeyValuePair<int, List<double>> zoneValue in zonalValues)
                {

                    string csvLine = string.Format("{0},{1},{2},{3},{4},{5},{6}", zoneValue.Key, zoneValue.Value.Count, zoneValue.Value.Min(min => Convert.ToDouble(min)), zoneValue.Value.Max(max => Convert.ToDouble(max)),
                        zoneValue.Value.Average(avg => Convert.ToDouble(avg)), zoneValue.Value.Sum(sum => Convert.ToDouble(sum)),zoneValue.Value.Distinct().Count());

                    w.WriteLine(csvLine);
                    w.Flush();

                    //Console.WriteLine(zoneValue.Key + " " + zoneValue.Value.Count + " " + zoneValue.Value.Sum(sum => Convert.ToDouble(sum)) + " "+ zoneValue.Value.Min(min => Convert.ToDouble(min))  );

                }
            }
        }
    }
}

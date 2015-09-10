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
                string csvHeader = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", "VALUE", "COUNT","AREA","MIN", "MAX", "RANGE", "MEAN", "STDEV", "SUM", "VARIETY", "MAJORITY","MINORITY","MEDIAN");
                w.WriteLine(csvHeader);

                foreach (KeyValuePair<int, List<double>> zoneValue in zonalValues)
                {
                    //zonal variables
                    var value = zoneValue.Key;
                    var count = zoneValue.Value.Count;
                    
                    var cellsize = 0;
                    var area = zoneValue.Value.Count*cellsize*cellsize;
                    
                    var min = zoneValue.Value.Min(v => Convert.ToDouble(v));
                    var max = zoneValue.Value.Max(v => Convert.ToDouble(v));
                    var range = max - min;
                    var mean = zoneValue.Value.Average(avg => Convert.ToDouble(avg));
                    var stdev = Math.Sqrt(zoneValue.Value.Average(z => z * z) - Math.Pow(zoneValue.Value.Average(), 2));
                    var sum = zoneValue.Value.Sum(v => Convert.ToDouble(v));
                    var variety = zoneValue.Value.Distinct().Count();

                    IEnumerable<double> sortedValueOccurance = zoneValue.Value.GroupBy(i => i).OrderByDescending(g => g.Count()).Select(g => g.Key);
                    var majority = sortedValueOccurance.First();
                    var minority = sortedValueOccurance.Last();

                    var median = GetMedian(zoneValue.Value);

                    var csvLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", value, count, area, min, max, range, mean, stdev, sum, variety, majority, minority, median);

                    w.WriteLine(csvLine);
                    w.Flush();




                    //Console.WriteLine(zoneValue.Key + " " + zoneValue.Value.Count + " " + zoneValue.Value.Sum(sum => Convert.ToDouble(sum)) + " "+ zoneValue.Value.Min(min => Convert.ToDouble(min))  );

                }
            }
        }

        private double GetMedian(IEnumerable<double> source)
        {
            var sortedList = from number in source
                             orderby number
                             select number;

            int count = sortedList.Count();
            int itemIndex = count / 2;
            if (count % 2 == 0) // Even number of items. 
                return (sortedList.ElementAt(itemIndex) +
                        sortedList.ElementAt(itemIndex - 1)) / 2;

            // Odd number of items. 
            return sortedList.ElementAt(itemIndex);
        }
    }
}

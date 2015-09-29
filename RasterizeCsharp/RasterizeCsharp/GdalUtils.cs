using System;
using RasterizeCsharp.ZonalStatistics;

namespace RasterizeCsharp
{
    class RsacGdalUtils
    {
        static void Main(string[] args)
        {
            
            string inputShapeFile = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\ns_lev05.shp";
            string infieldName = "Id";
            int outRasterCellSize =30;
            
            string inValueRaster = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\Whetstone_20080229eDOQQMos.tif";
            string outZonalCsvGDAL = "gdalZonalStat.csv";
            string outZonalCsvESRI = "esriZonalStat.csv";

            
            
            DateTime gdalStart = DateTime.Now;
            string invalRast = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\Whetstone_20080229SoilEnh.img";
            ComputeStatistics.ComputeZonalStatistics(invalRast, inputShapeFile, infieldName, outRasterCellSize, outZonalCsvGDAL);

            DateTime gdalEnd = DateTime.Now;

            TimeSpan gdalTimeSpan = gdalEnd - gdalStart;
            Console.WriteLine("Total time GDAL: {0}",gdalTimeSpan);
            
            DateTime esriStart = DateTime.Now;
            
            string folder =@"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik";
            string fileName = "Whetstone_20080229SoilEnh.img";
            

            ZonalStatisticsEsri.OpenFileRasterDataset(folder, fileName, inputShapeFile, infieldName, outRasterCellSize, outZonalCsvESRI);
          
            DateTime esriEnd = DateTime.Now;

            TimeSpan esriTimeSpan = esriEnd - esriStart;
            Console.WriteLine("Total time ESRI: {0}", esriTimeSpan);
            

              
            
            
            Console.WriteLine("Done");
            Console.ReadLine();

        }

    }
}

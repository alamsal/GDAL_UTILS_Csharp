using System;
using RasterizeCsharp.ZonalStatistics;

namespace RasterizeCsharp
{
    class RsacGdalUtils
    {
        static void Main(string[] args)
        {
            
            //string inputShapeFile = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\ns_lev05.shp";
            string inputShapeFile = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik2\smallish dataset\polygons.shp";
            //string infieldName = "Id";
            string infieldName = "OBJECTID";
            int outRasterCellSize =30;
            
            
            string outZonalCsvGDAL = "gdalZonalStat.csv";
            string outZonalCsvESRI = "esriZonalStat.csv";

            string inValueRaster = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik2\smallish dataset\teuitest_NED_30m_30_R1C1.img";
            //string inValueRaster = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\Whetstone_20080229SoilEnh.img";
            //string inValueRaster = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\Whetstone_20080229eDOQQMos.tif";
            
            DateTime gdalStart = DateTime.Now;
            ComputeStatistics.ComputeZonalStatistics(inValueRaster, inputShapeFile, infieldName, outRasterCellSize, outZonalCsvGDAL);

            DateTime gdalEnd = DateTime.Now;

            TimeSpan gdalTimeSpan = gdalEnd - gdalStart;
            Console.WriteLine("Total time GDAL: {0}",gdalTimeSpan);
            
            DateTime esriStart = DateTime.Now;
            
            //string folder =@"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik";
            string folder = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik2\smallish dataset";

            string fileName = "teuitest_NED_30m_30_R1C1.img";
            //string fileName = "Whetstone_20080229SoilEnh.img";

            //string fileName = "Whetstone_20080229eDOQQMos.tif";

            ZonalStatisticsEsri.OpenFileRasterDataset(folder, fileName, inputShapeFile, infieldName, outRasterCellSize, outZonalCsvESRI);
          
            DateTime esriEnd = DateTime.Now;

            TimeSpan esriTimeSpan = esriEnd - esriStart;
            Console.WriteLine("Total time ESRI: {0}", esriTimeSpan);
            Console.WriteLine("\n \n");
            Console.WriteLine(" Input shp: {0}", inputShapeFile);
            Console.WriteLine(" Input raster: {0}", inValueRaster);
            Console.WriteLine(" Output Cell: {0}", outRasterCellSize);

              
            
            
            Console.WriteLine("Done");
            Console.ReadLine();

        }

    }
}

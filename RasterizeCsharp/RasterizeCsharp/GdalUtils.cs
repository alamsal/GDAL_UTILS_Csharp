using System;
using RasterizeCsharp.ZonalStatistics;

namespace RasterizeCsharp
{
    class RsacGdalUtils
    {
        static void Main(string[] args)
        {
            
             
            //string inputShapeFile = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\ns_lev05.shp";
            //string inputShapeFile = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik2\smallish dataset\polygons.shp";
            //string inputShapeFile = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik2\TEUI_5__Test_data\state_boundaries_us_100k.shp";
            string inputShapeFile = @"D:\Ashis_Work\GDAL Utilities\sample-data\UtahGrid\Counties.shp";

            string infieldName = "FIPS";
            //string infieldName = "STATE_FIPS";
            //string infieldName = "Id";
            //string infieldName = "OBJECTID";
            //int outRasterCellSize =30;

            int outRasterCellSize = 90;
            
            
            string outZonalCsvGDAL = "gdalZonalStat.csv";
            string outZonalCsvESRI = "esriZonalStat.csv";

            //string inValueRaster = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik2\smallish dataset\teuitest_NED_30m_30_R1C1.img";
            //string inValueRaster = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\Whetstone_20080229SoilEnh.img";
            //string inValueRaster = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\Whetstone_20080229eDOQQMos.tif";

            //string inValueRaster = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik2\TEUI_5__Test_data\slope_90m";
            string inValueRaster = @"D:\Ashis_Work\GDAL Utilities\sample-data\UtahGrid\dem90_utm83";
            Console.WriteLine("GDAL working...");
            DateTime gdalStart = DateTime.Now;
            
            //ComputeStatistics.ComputeZonalStatistics(inValueRaster, inputShapeFile, infieldName, outRasterCellSize, outZonalCsvGDAL);

            string gdbPath = @"D:\Ashis_Work\GDAL Utilities\sample-data\Geodatabase\EsriZonalOutputs.gdb";

            ComputeStatistics.ComputeZonalStatisticsUsingFeatureGdb(gdbPath, "Counties", inValueRaster, infieldName,outRasterCellSize, outZonalCsvGDAL);
            
            
            DateTime gdalEnd = DateTime.Now;

            TimeSpan gdalTimeSpan = gdalEnd - gdalStart;
            Console.WriteLine("Total time GDAL: {0}",gdalTimeSpan);
            /*
            Console.WriteLine("Esri working ...");
            DateTime esriStart = DateTime.Now;
            
            //string folder =@"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik";
            //string folder = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik2\smallish dataset";
            string folder = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik2\TEUI_5__Test_data";
            //string folder = @"D:\Ashis_Work\GDAL Utilities\sample-data\UtahGrid";

            //string fileName = "teuitest_NED_30m_30_R1C1.img";
            //string fileName = "Whetstone_20080229SoilEnh.img";
            string fileName = "slope_90m";
            //string fileName = "dem90_utm83";
            //string fileName = "Whetstone_20080229eDOQQMos.tif";

            //ZonalStatisticsEsri.OpenFileRasterDataset(folder, fileName, inputShapeFile, infieldName, outRasterCellSize, outZonalCsvESRI);
          
            DateTime esriEnd = DateTime.Now;

            TimeSpan esriTimeSpan = esriEnd - esriStart;
            Console.WriteLine("Total time ESRI: {0}", esriTimeSpan);
            Console.WriteLine("\n \n");
            Console.WriteLine(" Input shp: {0}", inputShapeFile);
            Console.WriteLine(" Input raster: {0}", inValueRaster);
            Console.WriteLine(" Output Cell: {0}", outRasterCellSize);

            
            */



            Console.WriteLine("Done");
            Environment.Exit(0);
            

        }

    }
}

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
            int outRasterCellSize =1;
            
            string inValueRaster = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\Whetstone_20080229eDOQQMos.tif";
            string outZonalCsv = "new_zonalresults.csv";
            string outZonalTable = @"D:\Ashis_Work\GDAL Utilities\sample-data\esri_zonalTable.dbf";

            string outZonalTable2 = @"D:\Ashis_Work\GDAL Utilities\sample-data\esri_Table.dbf";
            
            /*
            DateTime gdalStart = DateTime.Now;
            ComputeStatistics.ComputeZonalStatistics(inValueRaster, inputShapeFile, infieldName, outRasterCellSize, outZonalCsv);
            DateTime gdalEnd = DateTime.Now;

            TimeSpan gdalTimeSpan = gdalEnd - gdalStart;
            Console.WriteLine("Total time GDAL: {0}",gdalTimeSpan);

            DateTime esriStart = DateTime.Now;
            ZonalStatisticsEsri.ComputeZonalStatisticsFromEsri(inputShapeFile,infieldName,inValueRaster,outZonalTable);
            DateTime esriEnd = DateTime.Now;

            TimeSpan esriTimeSpan = esriEnd - esriStart;
            Console.WriteLine("Total time ESRI: {0}", esriTimeSpan);
            */

            string folder =@"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik";
            string fileName = "Whetstone_20080229SoilEnh.img";
            ZonalStatisticsEsri.OpenFileRasterDataset(folder, fileName, inputShapeFile, infieldName, outZonalTable2);




            Console.ReadLine();

        }

    }
}

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
            string outZonalCsv = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\zonalresults.csv";

            ComputeStatistics.ComputeZonalStatistics(inValueRaster, inputShapeFile, infieldName, outRasterCellSize, outZonalCsv);

        }

    }
}

using System;
using RasterizeCsharp.RasterizeLayer;
using RasterizeCsharp.ZonalStatistics;

namespace RasterizeCsharp
{
    class VectorToRaster
    {
        static void Main(string[] args)
        {
            string inputShapeFile = @"D:\Ashis_Work\GDAL Utilities\sample-data\UtahBoundary.shp";
            string fieldName = "Shape_Area";
            string outRasterNameEsri = @"D:\Ashis_Work\GDAL Utilities\sample-data\Utah_ESRI_30m.tif";
            string outRasterNameGdal = @"D:\Ashis_Work\GDAL Utilities\sample-data\Utah_gdal_30m.tif";
            int rasterCellSize = 30;

            //SharpRasterizeLayer.VectorToRasterFromEsri(inputShapeFile, outRasterNameEsri, fieldName, rasterCellSize);
            //SharpRasterizeLayer.VectorToRasterFromGdal(inputShapeFile, outRasterNameGdal, fieldName, rasterCellSize);

            string goldStandardRaster =@"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\Whetstone_20080229eDOQQMos.tif";
            string goldStandardVector = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\ns_lev05.shp";
            string goldStandarOutputraster = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\ns_lev05.tif";

            //SharpRasterizeLayer.VectorToRasterFromGdal(goldStandardVector,goldStandarOutputraster,"Id",3);
            ComputeStatistics.ReadValueAndZoneRasters(goldStandardRaster, goldStandardRaster);
            

            Console.ReadLine();

        }

    }
}

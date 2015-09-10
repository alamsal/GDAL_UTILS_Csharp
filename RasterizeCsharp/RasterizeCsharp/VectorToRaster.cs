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
            int rasterCellSize = 1000;

            //SharpRasterizeLayer.VectorToRasterFromEsri(inputShapeFile, outRasterNameEsri, fieldName, rasterCellSize);
            //SharpRasterizeLayer.VectorToRasterFromGdal(inputShapeFile, outRasterNameGdal, fieldName, rasterCellSize);

            string goldStandardRaster =@"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\Whetstone_20080229eDOQQMos.tif";
            string croppedGoldStandardRaster = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\maskedout__1tif.tif";
            string goldStandardVector = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\ns_lev05.shp";
            string goldStandarZoneraster = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\ns_lev05.tif";


            // Gold standard
            /*
            fieldName = "Id";
            rasterCellSize = 3;

            SharpRasterizeLayer.VectorToRasterFromGdal(goldStandardVector, goldStandarZoneraster, fieldName, rasterCellSize);
            //ComputeStatistics.ReadValueAndZoneRasters(outRasterNameGdal, outRasterNameGdal);

             */

            ComputeStatistics.ReadValueAndZoneRasters(croppedGoldStandardRaster, goldStandarZoneraster);

            /*


            SharpRasterizeLayer.VectorToRasterFromGdal(inputShapeFile, outRasterNameGdal, fieldName, rasterCellSize);
            SharpRasterizeLayer.VectorToRasterFromGdal(inputShapeFile, outRasterNameEsri, fieldName, rasterCellSize);

            ComputeStatistics.ReadValueAndZoneRasters(outRasterNameEsri, outRasterNameGdal);
            */


          //  Console.ReadLine();

        }

    }
}

using System;
using RasterizeCsharp.RasterizeLayer;
using RasterizeCsharp.ZonalStatistics;
using RasterizeCsharp.MaskRaster;

namespace RasterizeCsharp
{
    class RsacGdalUtils
    {
        static void Main(string[] args)
        {
            string inputShapeFile = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\ns_lev03.shp";
            string fieldName = "Id";
            string outRasterNameEsri = @"D:\Ashis_Work\GDAL Utilities\sample-data\Utah_ESRI_30m.tif";
            string outRasterNameGdal = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\ns_lev03_gdal.tif";
            int rasterCellSize = 30;

            //SharpRasterizeLayer.VectorToRasterFromEsri(inputShapeFile, outRasterNameEsri, fieldName, rasterCellSize);
            //SharpRasterizeLayer.VectorToRasterFromGdal(inputShapeFile, outRasterNameGdal, fieldName, rasterCellSize);

            string goldStandardRaster =@"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\Whetstone_20080229eDOQQMos.tif";
            //string croppedGoldStandardRaster = @"D:\Ashis_Work\GDAL Utilities\sample-data\FromErik\maskedout__1tif.tif";
            string croppedGoldStandardRaster =@"D:\Ashis_Work\GDAL Utilities\sample-data\myraster_reproject.tif";

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

            
           // MaskRasterBoundary.CorrectRaster(inputShapeFile,goldStandardRaster);


          //  Console.ReadLine();

        }

    }
}

using System;
using RasterizeCsharp.RasterizeLayer;

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
            SharpRasterizeLayer.VectorToRasterFromGdal(inputShapeFile, outRasterNameGdal, fieldName, rasterCellSize);

            Console.ReadLine();

        }

    }
}

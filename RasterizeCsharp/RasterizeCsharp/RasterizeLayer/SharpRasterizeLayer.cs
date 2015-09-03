namespace RasterizeCsharp.RasterizeLayer
{
    class SharpRasterizeLayer
    {
        public static void VectorToRasterFromGdal(string inputFeature, string outRaster, string fieldName, int cellSize)
        {
            RasterizeGdal.Rasterize(inputFeature, outRaster, fieldName, cellSize);
        }

        public static void VectorToRasterFromEsri(string inputFeature, string outRaster, string fieldName, int cellSize)
        {
            RasterizeEsri.Rasterize(inputFeature, outRaster, fieldName, cellSize);
        }
    }
}

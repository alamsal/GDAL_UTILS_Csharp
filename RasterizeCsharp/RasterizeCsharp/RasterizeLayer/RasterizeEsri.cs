using System;
using ESRI.ArcGIS.ConversionTools;
using ESRI.ArcGIS.Geoprocessor;

namespace RasterizeCsharp.RasterizeLayer
{
    class RasterizeEsri
    {
        public static void Rasterize(string inputFeature, string outRaster, string fieldName, int cellSize)
        {
            try
            {
                //Runtime manager to find the ESRI product installed in the system
                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);

                Geoprocessor geoprocessor = new Geoprocessor();
                geoprocessor.OverwriteOutput = true;

                FeatureToRaster featureToRaster = new FeatureToRaster();
                featureToRaster.cell_size = cellSize;
                featureToRaster.in_features = inputFeature;
                featureToRaster.out_raster = outRaster;
                featureToRaster.field = fieldName;

                geoprocessor.Execute(featureToRaster, null);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}

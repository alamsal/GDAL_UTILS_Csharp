using OSGeo.GDAL;
using OSGeo.OGR;
using RasterizeCsharp.AppUtils;

namespace RasterizeCsharp.RasterizeLayer
{
    class RasterizeLayerGdal
    {
        public static void RasterizeFeature(string inputFeature, out Dataset outputDataset, string fieldName, double rasterCellSize)
        {
           DriverUtils.RegisterGdalOgrDrivers();
           ReadFeature readFeature = new ReadFeature(inputFeature);
           Layer layer = readFeature.GetFeatureLayer();
           
           ConversionGdal.ConvertFeatureToRaster(layer,out outputDataset,rasterCellSize,fieldName);
        }

        public static void RasterizeGdbFeature(string gdbPath,string inputeFeatureLayer,out Dataset outputDataset, string fieldName, double rasterCellSize)
        {
            DriverUtils.RegisterGdalOgrDrivers();
            ReadFeature readFeature = new ReadFeature(gdbPath,inputeFeatureLayer);
            Layer layer = readFeature.GetFeatureLayer();

            ConversionGdal.ConvertFeatureToRaster(layer, out outputDataset,rasterCellSize,fieldName);
        }

        
    }
}

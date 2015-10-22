using OSGeo.OGR;

namespace RasterizeCsharp.AppUtils
{
    class ReadFeature
    {
        private readonly Layer _layer;

        public ReadFeature(string inputFeaturePath)
        {
            DriverUtils.RegisterOgrDriver();
            //Reading the vector data
            DataSource dataSource = Ogr.Open(inputFeaturePath, 0);
            Layer layer = dataSource.GetLayerByIndex(0);

            _layer = layer;
        }

        public ReadFeature(string gdbPath, string featureLayerName)
        {
            DriverUtils.RegisterOgrDriver();

            //Reading the vector data from GDB
            DataSource dataSource = Ogr.Open(gdbPath, 0);
            Layer layer = dataSource.GetLayerByName(featureLayerName);

            _layer = layer;
        }

        public Layer GetFeatureLayer()
        {
            return _layer;
        }
    }
}

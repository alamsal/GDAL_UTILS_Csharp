using OSGeo.OGR;
using OSGeo.GDAL;

namespace RasterizeCsharp.AppUtils
{
    class DriverUtils
    {
        public static void RegisterGdalOgrDrivers()
        {
            RegisterOgrDriver();
            RegisterGdalDriver();
        }
        public static void RegisterOgrDriver()
        {
            Ogr.RegisterAll();
        }
        public static void RegisterGdalDriver()
        {
            Gdal.AllRegister();
        }
    }
}

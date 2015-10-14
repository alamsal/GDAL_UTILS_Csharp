using System;
using OSGeo.OGR;
namespace RasterizeCsharp.GdbOperations
{
    class GdbUtils
    {
        public static  void ReadEsriGdb(string gdbPath, string featureLayerName)
        {
            //Register the vector drivers
            Ogr.RegisterAll();

            //Reading the vector data
            DataSource dataSource = Ogr.Open(gdbPath, 0);
            Layer layer = dataSource.GetLayerByName(featureLayerName);
           
            Console.WriteLine(layer.GetName() + dataSource.GetLayerCount().ToString() );   
        }
    }
}

using System;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.SpatialAnalystTools;
using ESRI.ArcGIS.esriSystem;


namespace RasterizeCsharp.ZonalStatistics
{
    class ZonalStatisticsEsri
    {
        public static void ComputeZonalStatisticsFromEsri(string feature, string zoneField, string valueRaster, string outputTable)
        {

            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);
            ESRI.ArcGIS.RuntimeManager.BindLicense(ESRI.ArcGIS.ProductCode.Desktop);

            UID pUid = new UIDClass();
            pUid.Value = "esriSpatialAnalystUI.SAExtension";

            // Add Spatial Analyst extension to the license manager.
            object v = null;
            IExtensionManagerAdmin extensionManagerAdmin = new ExtensionManagerClass();
            extensionManagerAdmin.AddExtension(pUid, ref v);

            // Enable the license.
            IExtensionManager extensionManager = (IExtensionManager)extensionManagerAdmin;
            IExtension extension = extensionManager.FindExtension(pUid);
            IExtensionConfig extensionConfig = (IExtensionConfig)extension;



            if (extensionConfig.State != esriExtensionState.esriESUnavailable)
            {
                extensionConfig.State = esriExtensionState.esriESEnabled;

                Geoprocessor geoprocessor = new Geoprocessor();
                geoprocessor.OverwriteOutput = true;

                var zonalStatistics = new ZonalStatisticsAsTable
                                                                {
                                                                    in_value_raster = valueRaster,
                                                                    zone_field = zoneField,
                                                                    in_zone_data = feature,
                                                                    out_table = outputTable
                                                                };

                try
                {
                    geoprocessor.Execute(zonalStatistics, null);
                }
                catch (Exception ex)
                {
                    object level = 0;
                    Console.WriteLine(geoprocessor.GetMessages(ref level));
                    Console.WriteLine(ex.Message);
                }




            }
            else
            {
                Console.WriteLine("No Spatial Analyst License available");
            }









        }
    }
}

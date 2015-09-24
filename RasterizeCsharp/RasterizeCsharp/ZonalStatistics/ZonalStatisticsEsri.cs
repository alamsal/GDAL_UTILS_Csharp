using System;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
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

        private static void EnableEsriLiscences()
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
            }
            else
            {
                Console.WriteLine("No Spatial Analyst License available");
            }

        }

        public static void OpenFileRasterDataset(string folderName, string datasetName,string featureName,string fieldName, string outTableName)
        {
            EnableEsriLiscences();

            //Open raster file workspace
            IWorkspaceFactory workspaceFactory = new RasterWorkspaceFactoryClass();
            IRasterWorkspace rasterWorkspace = (IRasterWorkspace) workspaceFactory.OpenFromFile(folderName, 0);

            //Open file raster dataset 
            IRasterDataset rasterDataset = rasterWorkspace.OpenRasterDataset(datasetName);

            IRasterBandCollection rasBandCol = rasterDataset as IRasterBandCollection;
            

            //create raster cursor to read block by block

            IRaster2 raster2 = rasterDataset.CreateDefaultRaster() as IRaster2;

            IRasterCursor rasterCursor = raster2.CreateCursorEx(null);

            IRasterEdit rasterEdit = raster2 as IRasterEdit;
            System.Array pixels;
            object value;


            IPixelBlock3 pixelBlock3 = null;
            int blockWidth = 0;
            int blockHeight = 0;
            
            do
            {
                pixelBlock3 = rasterCursor.PixelBlock as IPixelBlock3;
                blockWidth = pixelBlock3.Width;
                blockHeight = pixelBlock3.Height;

                Console.WriteLine(blockHeight);
                Console.WriteLine(blockWidth);

                try
                {
                    for (int k = 0; k < rasBandCol.Count; k++)
                    {
                        Console.WriteLine(k);
                        //Get pixel array
                        pixels = (System.Array)pixelBlock3.get_PixelData(k);

                        for (int i = 0; i < blockWidth; i++)
                        {
                            for (int j = 0; j < blockHeight; j++)
                            {
                                //Get pixel value
                                value = pixels.GetValue(i, j);

                                //Console.WriteLine(value);
                            }
                        }

                        pixelBlock3.set_PixelData(k, pixels);
                    }
                
                }
                catch(Exception ex)
               {
                   Console.WriteLine(ex.Message);
               }
                

               


            } while (rasterCursor.Next() == true);

            Console.WriteLine("done");

        }


    }
}

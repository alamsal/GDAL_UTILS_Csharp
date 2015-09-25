using System;
using System.Collections.Generic;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.SpatialAnalystTools;
using ESRI.ArcGIS.esriSystem;

using RasterizeCsharp.RasterizeLayer;

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

        public static void OpenFileRasterDataset(string inFolderName, string inRasterDatasetName, string inFeatureName, string inFieldName, int outCellSize)
        {
            EnableEsriLiscences();

            //Get feature raster from feature shp
            string outTempRasterName = "tempRasterFromESRI.tif";
            string outZoneRater = inFolderName + "\\" + outTempRasterName;
            RasterizeEsri.Rasterize(inFeatureName, outZoneRater, inFieldName, outCellSize);
            
            // Value containers
            Dictionary<int,List<double>> rasterBandValues = new Dictionary<int, List<double>>();
            Dictionary<int,uint> rasterCellCounts = new Dictionary<int, uint>();
            

            //Open raster file workspace
            IWorkspaceFactory workspaceFactory = new RasterWorkspaceFactoryClass();
            IRasterWorkspace rasterWorkspace = (IRasterWorkspace) workspaceFactory.OpenFromFile(inFolderName, 0);
            
            
            //Open zone raster dataset
            IRasterDataset zoneRasterDataset = rasterWorkspace.OpenRasterDataset(outTempRasterName);


            //Open value raster dataset 
            IRasterDataset valueRasterDataset = rasterWorkspace.OpenRasterDataset(inRasterDatasetName);

            //Extract bands from the raster
            IRasterBandCollection zoneRasterBandCol = zoneRasterDataset as IRasterBandCollection;
            IRasterBandCollection valueRasterBandCol = valueRasterDataset as IRasterBandCollection;
            

            //create raster cursor to read block by block
            IPnt pnt = new PntClass();
            pnt.SetCoords(1024,2014);

            IRaster2 zoneRaster2 = zoneRasterDataset.CreateDefaultRaster() as IRaster2;
            IRasterCursor zoneRasterCursor = zoneRaster2.CreateCursorEx(pnt);

            IRaster2 valueRaster2 = valueRasterDataset.CreateDefaultRaster() as IRaster2;
            IRasterCursor valueRasterCursor = valueRaster2.CreateCursorEx(pnt);

            
            System.Array valueRasterPixels;
            System.Array zoneRasterPixels;
            object pixelValueFromValue; 
            object pixelValueFromZone;


            IPixelBlock3 valueRasterPixelBlock3 = null;
            IPixelBlock3 zoneRasterPixelBlock3 = null;
            int blockWidth = 0;
            int blockHeight = 0;
            int zoneRasterBandId = 0;


            //Raster value holder
            if (valueRasterBandCol != null)
            {
                Dictionary<int,StatisticsInfo> [] rasInfoDict = new Dictionary<int, StatisticsInfo>[valueRasterBandCol.Count];
                
            
                do
                {


                    valueRasterPixelBlock3 = valueRasterCursor.PixelBlock as IPixelBlock3;
                    blockWidth = valueRasterPixelBlock3.Width;
                    blockHeight = valueRasterPixelBlock3.Height;

                    zoneRasterPixelBlock3 = zoneRasterCursor.PixelBlock as IPixelBlock3;

                    Console.WriteLine(zoneRasterPixelBlock3.Width);
                    Console.WriteLine(blockWidth);

                    try
                    {

                        zoneRasterPixels = (System.Array)zoneRasterPixelBlock3.get_PixelData(zoneRasterBandId);
                        for (int b = 0; b < valueRasterBandCol.Count; b++)
                        {
                            Console.WriteLine(b);
                            //Get pixel array
                            valueRasterPixels = (System.Array)valueRasterPixelBlock3.get_PixelData(b);
                            

                            rasInfoDict[b] = new Dictionary<int, StatisticsInfo>();

                            for (int i = 0; i < blockWidth; i++)
                            {
                                for (int j = 0; j < blockHeight; j++)
                                {
                                    //Get pixel value
                                    pixelValueFromValue = valueRasterPixels.GetValue(i, j);
                                    pixelValueFromZone = zoneRasterPixels.GetValue(i, j);
                                
                                    //process each pixel value
                                    
                                    if (rasInfoDict[b].ContainsKey(Convert.ToInt32(pixelValueFromZone)))
                                    {
                                        StatisticsInfo rastStatistics = rasInfoDict[b][ Convert.ToInt32(pixelValueFromZone)];
                                        rastStatistics.Count++;
                                        rastStatistics.Sum = rastStatistics.Sum + Convert.ToDouble(pixelValueFromValue);

                                        rasInfoDict[b][ Convert.ToInt32(pixelValueFromZone)] = rastStatistics;
                                    }
                                 
                                    else
                                    {
                                        rasInfoDict[b][Convert.ToInt32(pixelValueFromZone)] = new StatisticsInfo() {Count = 1,Sum = Convert.ToDouble(pixelValueFromValue)};
                                    }
                                        

                                    Console.WriteLine( pixelValueFromZone +" <--> "+ pixelValueFromValue);
                                }
                            }
                            valueRasterPixelBlock3.set_PixelData(b, valueRasterPixels);
                            zoneRasterPixelBlock3.set_PixelData(zoneRasterBandId, zoneRasterPixels);
                        }
                       


                
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }


                } while (valueRasterCursor.Next() == true);

                Console.WriteLine(rasInfoDict);
            }
        }


    }
}

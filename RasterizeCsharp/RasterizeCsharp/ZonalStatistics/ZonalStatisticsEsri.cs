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
            int rasterBlockSize = 1024;
            RasterizeEsri.Rasterize(inFeatureName, outZoneRater, inFieldName, outCellSize);

            //Open raster file workspace
            IWorkspaceFactory workspaceFactory = new RasterWorkspaceFactoryClass();
            IRasterWorkspace rasterWorkspace = (IRasterWorkspace)workspaceFactory.OpenFromFile(inFolderName, 0);


            //Open zone raster dataset
            IRasterDataset zoneRasterDataset = rasterWorkspace.OpenRasterDataset(outTempRasterName);
            IRasterDataset2 zoneRasterDataset2 = zoneRasterDataset as IRasterDataset2;
            IRaster2 zoneRs2 = zoneRasterDataset2.CreateFullRaster() as IRaster2;


            //Open value raster dataset 
            IRasterDataset valueRasterDataset = rasterWorkspace.OpenRasterDataset(inRasterDatasetName);
            IRasterDataset2 valueRasterDataset2 = valueRasterDataset as IRasterDataset2;
            IRaster2 valueRs2 = valueRasterDataset2.CreateFullRaster() as IRaster2;

            //Extract bands from the raster
            IRasterBandCollection valueRasterPlanes = valueRasterDataset as IRasterBandCollection;


            //create raster cursor to read block by block
            IPnt blockSize = new PntClass();
            blockSize.SetCoords(rasterBlockSize, rasterBlockSize);

            IRasterCursor valueRasterCursor = valueRs2.CreateCursorEx(blockSize);
            IRasterCursor zoneRasterCursor = zoneRs2.CreateCursorEx(blockSize);

           
            if (valueRasterPlanes != null)
            {
                Dictionary<int, StatisticsInfo>[] rasInfoDict = new Dictionary<int, StatisticsInfo>[valueRasterPlanes.Count];
                int zoneRasterBandId = 0;

                do
                {
                    IPixelBlock3 valueRasterPixelBlock3 = valueRasterCursor.PixelBlock as IPixelBlock3;
                    
                    IPixelBlock3 zoneRasterPixelBlock3 = zoneRasterCursor.PixelBlock as IPixelBlock3;
                    int blockWidth = zoneRasterPixelBlock3.Width;
                    int blockHeight = zoneRasterPixelBlock3.Height;

                    //Console.WriteLine(zoneRasterPixelBlock3.Width);
                    //Console.WriteLine(blockWidth);

                    try
                    {
                        System.Array zoneRasterPixels = (System.Array)zoneRasterPixelBlock3.get_PixelData(zoneRasterBandId);
                        for (int b = 0; b < valueRasterPlanes.Count; b++)
                        {
                            Console.WriteLine(b);
                            //Get pixel array
                            System.Array valueRasterPixels = (System.Array)valueRasterPixelBlock3.get_PixelData(b);

                            rasInfoDict[b] = new Dictionary<int, StatisticsInfo>();

                            for (int i = 0; i < blockWidth; i++)
                            {
                                for (int j = 0; j < blockHeight; j++)
                                {
                                    //Get pixel value
                                    object pixelValueFromValue = valueRasterPixels.GetValue(i, j);
                                    object pixelValueFromZone = zoneRasterPixels.GetValue(i, j);
                                    
                                    //process each pixel value
                                    if (rasInfoDict[b].ContainsKey(Convert.ToInt32(pixelValueFromZone)))
                                    {
                                        StatisticsInfo rastStatistics = rasInfoDict[b][Convert.ToInt32(pixelValueFromZone)];
                                        rastStatistics.Count++;
                                        rastStatistics.Sum = rastStatistics.Sum + Convert.ToDouble(pixelValueFromValue);

                                        rasInfoDict[b][Convert.ToInt32(pixelValueFromZone)] = rastStatistics;
                                    }
                                    else
                                    {
                                        rasInfoDict[b][Convert.ToInt32(pixelValueFromZone)] = new StatisticsInfo() { Count = 1, Sum = Convert.ToDouble(pixelValueFromValue) };
                                    }
                                }
                            }
                         }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                } while (zoneRasterCursor.Next() == true);

                Console.WriteLine(rasInfoDict);
            }
            else
            {
                Console.WriteLine("No band available in the Value Raster");
            }
        }


    }
}

using System;
using System.Collections.Generic;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

using RasterizeCsharp.RasterizeLayer;
using RasterizeCsharp.ZonalIO;
using RasterizeCsharp.MaskRaster;

namespace RasterizeCsharp.ZonalStatistics
{
    class ZonalStatisticsEsri
    {
        public static void OpenFileRasterDataset(string inFolderName, string inRasterDatasetName, string inFeatureName, string inFieldName, double outCellSize, string outSummaryFile)
        {
            EnableEsriLiscences();

            //Get feature raster from feature shp
            string outTempRasterName = "tempZoneRasterFromESRI.tif";
            string outZoneRater = inFolderName + "\\" + outTempRasterName;
            int rasterBlockSize = 1024;
            RasterizeEsri.Rasterize(inFeatureName, outZoneRater, inFieldName, outCellSize);

            //Open raster file workspace
            IWorkspaceFactory workspaceFactory = new RasterWorkspaceFactoryClass();
            IRasterWorkspace rasterWorkspace = (IRasterWorkspace)workspaceFactory.OpenFromFile(inFolderName, 0);

            //Align raster
            string inValueRaster = inFolderName + "\\" + inRasterDatasetName;
            string inClipFeature = inFeatureName;
            string outClippedRasterName = "tempValueRasterFromESRI.tif";
            string outClippedValueRaster = inFolderName + "\\" + outClippedRasterName;

            ClipRasterBoundaryEsri.ClipRaster(inValueRaster, inClipFeature, outClippedValueRaster);

            //Open zone raster dataset
            IRasterDataset zoneRasterDataset = rasterWorkspace.OpenRasterDataset(outTempRasterName);
            IRasterDataset2 zoneRasterDataset2 = zoneRasterDataset as IRasterDataset2;
            IRaster2 zoneRs2 = zoneRasterDataset2.CreateFullRaster() as IRaster2;


            //Open value raster dataset 
            IRasterDataset valueRasterDataset = rasterWorkspace.OpenRasterDataset(outClippedRasterName);
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

                for (int b = 0; b < valueRasterPlanes.Count; b++)
                {
                    rasInfoDict[b] = new Dictionary<int, StatisticsInfo>();
                }

                do
                {
                    IPixelBlock3 valueRasterPixelBlock3 = valueRasterCursor.PixelBlock as IPixelBlock3;
                    IPixelBlock3 zoneRasterPixelBlock3 = zoneRasterCursor.PixelBlock as IPixelBlock3;

                    //No Idea how esri cursor fills the raster gap if zone is greater than value, so quick and fix using smallest extent

                    int blockWidth = valueRasterPixelBlock3.Width < zoneRasterPixelBlock3.Width ? valueRasterPixelBlock3.Width : zoneRasterPixelBlock3.Width;
                    int blockHeight = valueRasterPixelBlock3.Height < zoneRasterPixelBlock3.Height ? valueRasterPixelBlock3.Height : zoneRasterPixelBlock3.Height;


                    //Console.WriteLine(zoneRasterPixelBlock3.Width);
                    //Console.WriteLine(blockWidth);

                    try
                    {
                        System.Array zoneRasterPixels = (System.Array)zoneRasterPixelBlock3.get_PixelData(zoneRasterBandId);
                        for (int b = 0; b < valueRasterPlanes.Count; b++)
                        {
                            //Console.WriteLine(b);
                            //Get pixel array
                            System.Array valueRasterPixels = (System.Array)valueRasterPixelBlock3.get_PixelData(b);

                            for (int i = 0; i < blockWidth; i++)
                            {
                                for (int j = 0; j < blockHeight; j++)
                                {
                                    //Get pixel value
                                    object pixelValueFromValue = null;
                                    object pixelValueFromZone = null;
                                    try
                                    {
                                        pixelValueFromValue = valueRasterPixels.GetValue(i, j);
                                        pixelValueFromZone = zoneRasterPixels.GetValue(i, j);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }


                                    //process each pixel value
                                    try
                                    {
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
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }

                                    //Console.WriteLine(i +"-"+j);
                                    //Console.WriteLine(pixelValueFromValue + "-" + pixelValueFromZone);



                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                } while (zoneRasterCursor.Next() == true);

                //Export results
                StatisticsExport writer = new StatisticsExport(outSummaryFile);
                writer.ExportZonalStatistics(ref rasInfoDict, outCellSize);

            }
            else
            {
                Console.WriteLine("No band available in the Value Raster");
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


    }
}

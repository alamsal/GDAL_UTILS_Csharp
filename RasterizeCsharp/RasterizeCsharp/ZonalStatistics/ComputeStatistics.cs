using System;
using System.Collections.Generic;
using OSGeo.GDAL;
using RasterizeCsharp.AppUtils;
using RasterizeCsharp.ZonalIO;
using RasterizeCsharp.RasterizeLayer;
using RasterizeCsharp.MaskRaster;

namespace RasterizeCsharp.ZonalStatistics
{
    class ComputeStatistics
    {
        private static Dictionary<int, StatisticsInfo>[] _rasInfoDict;

        private static string _zoneFile;
        private static double _cellSize;

        public static void ComputeZonalStatistics(string valueRasterName, string featureName, string featureFieldName, double cellSize, string zoneOutputFile)
        {
            Dataset alignedValueRaster;
            Dataset zoneRaster;

            //Step 1: Convert feature to raster
            RasterizeLayerGdal.RasterizeFeature(featureName, out zoneRaster, featureFieldName, cellSize);

            //Step 2: Align/mask value raster with step1's output raster
            MaskRasterBoundary.ClipRasterUsingFeature(featureName, valueRasterName, cellSize, out alignedValueRaster);
            _zoneFile = zoneOutputFile;
            _cellSize = cellSize;

            //Setp 3: Feed both raster into an algorithm
            ReadRasterBlocks(ref alignedValueRaster, ref zoneRaster);
        }

        public static void ComputeZonalStatisticsUsingFeatureGdb(string gdbPath, string featureLayerName, string valueRasterName, string featureFieldName, double cellSize, string zoneOutputFile)
        {
            Dataset alignedValueRaster;
            Dataset zoneRaster;

            //Step 1: Convert feature to raster
            RasterizeLayerGdal.RasterizeGdbFeature(gdbPath, featureLayerName, out zoneRaster, featureFieldName, cellSize);

            //Step 2: Align/mask value raster with step1's output raster
            MaskRasterBoundary.ClipRasterUsingGdbFeature(gdbPath, featureLayerName, valueRasterName, cellSize, out alignedValueRaster);
            _zoneFile = zoneOutputFile;
            _cellSize = cellSize;

            //Setp 3: Feed both raster into an algorithm
            ReadRasterBlocks(ref alignedValueRaster, ref zoneRaster);
        }


        private static void ProcessEachRasterBlock(double[] valueRasterValues, double[] zoneRasterValues, int band, ref Dictionary<int, StatisticsInfo>[] rasInfoDict)
        {
            for (int index = 0; index < valueRasterValues.Length; index++)
            {
                //Skip no data values as in ESRI
                if ((Math.Round(zoneRasterValues[index], 3) != GdalUtilConstants.NoDataValue) && (Math.Round(valueRasterValues[index], 3) != GdalUtilConstants.NoDataValue))
                {
                    int pixelValueFromZone = Convert.ToInt32(zoneRasterValues[index]);
                    double pixelValueFromValue = valueRasterValues[index];

                    //process each pixel value
                    if (rasInfoDict[band].ContainsKey(Convert.ToInt32(pixelValueFromZone)))
                    {
                        StatisticsInfo rastStatistics = rasInfoDict[band][Convert.ToInt32(pixelValueFromZone)];
                        rastStatistics.Count++;
                        rastStatistics.Sum = rastStatistics.Sum + pixelValueFromValue;

                        rasInfoDict[band][Convert.ToInt32(pixelValueFromZone)] = rastStatistics;
                    }
                    else
                    {
                        rasInfoDict[band][Convert.ToInt32(pixelValueFromZone)] = new StatisticsInfo() { Count = 1, Sum = pixelValueFromValue };
                    }
                }
            }
        }

        private static void ReadRasterBlocks(ref Dataset valueRaster, ref Dataset zoneRaster)
        {
            _rasInfoDict = new Dictionary<int, StatisticsInfo>[valueRaster.RasterCount];

            int valueRasterBandCount = valueRaster.RasterCount;

            int rasterRows = zoneRaster.RasterYSize;
            int rasterCols = zoneRaster.RasterXSize;

            const int blockSize = AppUtils.GdalUtilConstants.RasterBlockSize;

            for (int rasBand = 0; rasBand < valueRasterBandCount; rasBand++)
            {
                Band bandValueRaster = valueRaster.GetRasterBand(rasBand + 1);
                Band bandZoneRaster = zoneRaster.GetRasterBand(1);

                _rasInfoDict[rasBand] = new Dictionary<int, StatisticsInfo>();

                for (int row = 0; row < rasterRows; row += blockSize)
                {
                    int rowProcess;
                    if (row + blockSize < rasterRows)
                    {
                        rowProcess = blockSize;
                    }
                    else
                    {
                        rowProcess = rasterRows - row;
                    }

                    for (int col = 0; col < rasterCols; col += blockSize)
                    {
                        int colProcess;
                        if (col + blockSize < rasterCols)
                        {
                            colProcess = blockSize;
                        }
                        else
                        {
                            colProcess = rasterCols - col;
                        }

                        double[] valueRasterValues = new double[rowProcess * colProcess];
                        double[] zoneRasterValues = new double[rowProcess * colProcess];

                        bandValueRaster.ReadRaster(col, row, colProcess, rowProcess, valueRasterValues, colProcess, rowProcess, 0, 0);
                        bandZoneRaster.ReadRaster(col, row, colProcess, rowProcess, zoneRasterValues, colProcess, rowProcess, 0, 0);

                        ProcessEachRasterBlock(valueRasterValues, zoneRasterValues, rasBand, ref _rasInfoDict);
                    }
                }
            }

            //flush rasters cache
            valueRaster.FlushCache();
            zoneRaster.FlushCache();

            valueRaster.Dispose();
            zoneRaster.Dispose();

            StatisticsExport writer = new StatisticsExport(_zoneFile);
            writer.ExportZonalStatistics2(ref _rasInfoDict, _cellSize);
        }
    }
}

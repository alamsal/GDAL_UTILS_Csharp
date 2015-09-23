using System;
using System.Collections.Generic;
using OSGeo.GDAL;
using RasterizeCsharp.AppUtils;
using RasterizeCsharp.ZonalIO;
using RasterizeCsharp.RasterizeLayer;
using RasterizeCsharp.MaskRaster;
using RasterizeCsharp.ZonalStatistics;

namespace RasterizeCsharp.ZonalStatistics
{
    class ComputeStatistics
    {
        
        private static Dictionary<int,StatisticsInfo> _zonalValues = new Dictionary<int, StatisticsInfo>();

        private static string _zoneFile;
        private static double _cellSize;

        public static void ComputeZonalStatistics(string valueRasterName, string featureName, string featureFieldName, double cellSize, string zoneOutputFile)
        {
            Dataset alignedValueRaster;
            Dataset zoneRaster;

            //Step 1: Convert feature to raster
            RasterizeGdal.Rasterize(featureName, out zoneRaster, featureFieldName, cellSize);

            //Step 2: Align/mask value raster with step1's output raster
            MaskRasterBoundary.ClipRaster(featureName, valueRasterName, cellSize, out alignedValueRaster);
            _zoneFile = zoneOutputFile;
            _cellSize = cellSize;

            //Setp 3: Feed both raster into an algorithm
            ReadRasterBlocks(ref alignedValueRaster, ref zoneRaster);
        }

        private static void ProcessEachRasterBlock(double[] valueRasterValues, double[] zoneRasterValues)
        {
            for(int index=0;index<valueRasterValues.Length;index++)
            {
                //Skip no data values as in ESRI
                if((Math.Round(zoneRasterValues[index],3) != GdalUtilConstants.NoDataValue) && (Math.Round(valueRasterValues[index],3) != GdalUtilConstants.NoDataValue))
                {
                    int zoneRasterPixelValue = Convert.ToInt32(zoneRasterValues[index]);

                    double valueRasterPixelValue = valueRasterValues[index];

                    if (_zonalValues.ContainsKey(zoneRasterPixelValue))
                    {
                        StatisticsInfo statisticsInfo = _zonalValues[zoneRasterPixelValue];

                        statisticsInfo.Count++;
                        statisticsInfo.Sum = statisticsInfo.Sum + valueRasterPixelValue;

                        _zonalValues[zoneRasterPixelValue] = statisticsInfo;

                    }
                    else
                    {
                        _zonalValues[zoneRasterPixelValue] = new StatisticsInfo() { Count = 1, Sum = valueRasterPixelValue };
                    }
                    
                }

            }
        }

        private static void ReadRasterBlocks(ref Dataset valueRaster, ref Dataset zoneRaster)
        {
            Band bandValueRaster = valueRaster.GetRasterBand(1);
            Band bandZoneRaster = zoneRaster.GetRasterBand(1);

            int rasterRows = valueRaster.RasterYSize;
            int rasterCols = valueRaster.RasterXSize;
            const int blockSize = AppUtils.GdalUtilConstants.RasterBlockSize;

            for(int row=0; row<rasterRows; row += blockSize)
            {
                int rowProcess;
                if(row + blockSize < rasterRows)
                {
                    rowProcess = blockSize;
                }
                else
                {
                    rowProcess = rasterRows - row;
                }

                for(int col=0; col < rasterCols; col += blockSize)
                {
                    int colProcess;
                    if(col + blockSize < rasterCols)
                    {
                        colProcess = blockSize;
                    }
                    else
                    {
                        colProcess = rasterCols - col;
                    }

                    double[] valueRasterValues = new double[rowProcess*colProcess];
                    double[] zoneRasterValues = new double[rowProcess*colProcess];

                    bandValueRaster.ReadRaster(col, row, colProcess, rowProcess, valueRasterValues, colProcess,rowProcess, 0, 0);
                    bandZoneRaster.ReadRaster(col, row, colProcess, rowProcess, zoneRasterValues, colProcess, rowProcess, 0, 0);
                    
                    ProcessEachRasterBlock(valueRasterValues,zoneRasterValues);
                }
            }

            StatisticsExport writer = new StatisticsExport(_zoneFile);
            writer.ExportZonalStatistics2(ref _zonalValues, _cellSize);
        }



    }
}

using System;
using System.Collections.Generic;
using OSGeo.GDAL;
using RasterizeCsharp.ZonalIO;
using RasterizeCsharp.RasterizeLayer;
using RasterizeCsharp.MaskRaster;

namespace RasterizeCsharp.ZonalStatistics
{
    class ComputeStatistics
    {
        private static Dictionary<int, List<double>> _zonalValues;
        private static Dictionary<int, List<double>> _blockZonalValues = new Dictionary<int, List<double>>();

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

        private static void ProcessRasterBlock(double[] valueRasterValues, double[] zoneRasterValues)
        {
            for(int index=0;index<valueRasterValues.Length;index++)
            {
                int zoneRasterPixelValue = Convert.ToInt32(zoneRasterValues[index]);

                double valueRasterPixelValue = valueRasterValues[index];

                //Console.WriteLine(rasterPixelValue + " X: " + col + " Y:" + row);

                if (_blockZonalValues.ContainsKey(zoneRasterPixelValue))
                {
                    _blockZonalValues[zoneRasterPixelValue].Add(valueRasterPixelValue);
                }
                else
                {
                    _blockZonalValues.Add(zoneRasterPixelValue, new List<double>() { valueRasterPixelValue });
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

                  
                    
                    ProcessRasterBlock(valueRasterValues,zoneRasterValues);
                }
            }

            StatisticsExport writer = new StatisticsExport(_zoneFile);
            writer.ExportZonalStatistics(ref _blockZonalValues, _cellSize);


        }



    }
}

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
            
            //ValueAndZoneRasters(ref alignedValueRaster, ref zoneRaster);
            ReadRasterInBlocks(ref alignedValueRaster, ref zoneRaster);
            
        }
        
        /*
        private static void ValueAndZoneRasters(ref Dataset valueRasterDataset, ref Dataset zoneRasterDataset)
        {
            double[] valueRaster;
            double[] zoneRaster;

            RasterInfo valueRasterInfo = GetRasterAsArray(ref valueRasterDataset, out valueRaster);
            RasterInfo zoneRasterInfo = GetRasterAsArray(ref zoneRasterDataset, out zoneRaster);
            
            
            //Exporting files for viewing output results
            //OSGeo.GDAL.Driver driver = Gdal.GetDriverByName("GTiff");

            //driver.CreateCopy("valueRaster.tif", valueRasterDataset, 0, null, null, null);
            //driver.CreateCopy("zoneRaster.tif", zoneRasterDataset, 0, null, null, null);
            
            
            
            valueRasterDataset.FlushCache();
            zoneRasterDataset.FlushCache();
            valueRasterDataset.Dispose();
            zoneRasterDataset.Dispose();

            if (valueRasterInfo.RasterHeight != zoneRasterInfo.RasterHeight || valueRasterInfo.RasterWidth != zoneRasterInfo.RasterWidth)
            {
                Console.WriteLine("Given input rasters have inconsistant width or height");
                //System.Environment.Exit(-1);
            }
            else
            {
                PrepareForStatistics(ref valueRaster, ref zoneRaster, valueRasterInfo, out _zonalValues);
            }
        }

        private static void PrepareForStatistics(ref double[] valueRaster, ref double[] zoneRaster, RasterInfo rasterInfo, out Dictionary<int, List<double>> zonalValues)
        {
            zonalValues = new Dictionary<int, List<double>>();
            Console.WriteLine("Calculating zonal statistics ...");
            //Do data processing for raster
            for (int col = 0; col < rasterInfo.RasterWidth; col++)
            {
                for (int row = 0; row < rasterInfo.RasterHeight; row++)
                {
                    int zoneRasterPixelValue = Convert.ToInt32(zoneRaster[col + row * rasterInfo.RasterWidth]);
                    double valueRasterPixelValue = valueRaster[col + row * rasterInfo.RasterWidth];

                    //Console.WriteLine(rasterPixelValue + " X: " + col + " Y:" + row);

                   if(zonalValues.ContainsKey(zoneRasterPixelValue))
                   {
                       zonalValues[zoneRasterPixelValue].Add(valueRasterPixelValue);
                   }else
                   {
                       zonalValues.Add(zoneRasterPixelValue,new List<double>(){valueRasterPixelValue});
                   }
                }
            }
          
          valueRaster = null;
          zoneRaster = null;
          StatisticsExport writer = new StatisticsExport(_zoneFile);
          writer.ExportZonalStatistics(ref zonalValues,_cellSize);

        

        }

        private static RasterInfo GetRasterAsArray(ref Dataset rasterDataset, out double[] rasterValues)
        {
            //raster size
            int rasterCols = rasterDataset.RasterXSize;
            int rasterRows = rasterDataset.RasterYSize;

            //Read 1st band from raster
            Band band = rasterDataset.GetRasterBand(1);
            int rastWidth = rasterCols;
            int rastHeight = rasterRows;
           
                //Need to find out an algorithm to read memory block by block instead of reading a big chunk of data
                rasterValues = new double[rastWidth * rastHeight];
                band.ReadRaster(0, 0, rastWidth, rastHeight, rasterValues, rastWidth, rastHeight, 0, 0);
            
            var info = new RasterInfo { RasterHeight = rasterRows, RasterWidth = rasterCols };
            return info;
        }

        */

        private static void ReadRasterInBlocks(ref Dataset valueRaster, ref Dataset zoneRaster)
        {
            Band bandValueRaster = valueRaster.GetRasterBand(1);
            Band bandZoneRaster = zoneRaster.GetRasterBand(1);

            int rasterRows = valueRaster.RasterYSize;
            int rasterCols = valueRaster.RasterXSize;

            for(int row=0; row <rasterRows; row++)
            {
                double[] valueRasterValues = new double[rasterCols];
                double[] zoneRasterValues = new double[rasterCols];

                bandValueRaster.ReadRaster(0, row, rasterCols, 1, valueRasterValues, rasterCols, 1, 0, 0);
                bandZoneRaster.ReadRaster(0, row, rasterCols, 1, zoneRasterValues, rasterCols, 1, 0, 0);

                ProcessRasterBlock(valueRasterValues, zoneRasterValues);
            }

            StatisticsExport writer = new StatisticsExport(_zoneFile);
            writer.ExportZonalStatistics(ref _blockZonalValues, _cellSize);

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

    }
}

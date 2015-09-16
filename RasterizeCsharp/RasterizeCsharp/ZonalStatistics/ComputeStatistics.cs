using System;
using System.Collections.Generic;

using OSGeo.OGR;
using OSGeo.OSR;
using OSGeo.GDAL;

using RasterizeCsharp.AppConstants;
using RasterizeCsharp.ZonalIO;
using RasterizeCsharp.RasterizeLayer;
using RasterizeCsharp.MaskRaster;

namespace RasterizeCsharp.ZonalStatistics
{
    class ComputeStatistics
    {
        //Dictionary<int, List<double>> zonalValues = new Dictionary<int, List<double>>();
        
        private static Dictionary<int, List<double>> _zonalValues;

        /*
        public static void ReadValueAndZoneRasters(string valueRasterName,string zoneRasterName)
        {
            double [] valueRaster;
            double [] zoneRaster;

            RasterInfo valueRasterInfo = GetRasterValue(valueRasterName, out valueRaster);
            RasterInfo zoneRasterInfo = GetRasterValue(zoneRasterName, out zoneRaster);
            
            if(valueRasterInfo.RasterHeight!=zoneRasterInfo.RasterHeight || valueRasterInfo.RasterWidth != zoneRasterInfo.RasterWidth)
            {
                Console.WriteLine("Given input rasters have inconsistant width or height");
                //System.Environment.Exit(-1);
            }else
            {
                PrepareForStatistics(ref valueRaster,ref zoneRaster,valueRasterInfo, out _zonalValues );
            }

        }
        */

        public static void ValueAndZoneRasters(ref Dataset valueRasterDataset, ref Dataset zoneRasterDataset)
        {


            double[] valueRaster;
            double[] zoneRaster;

            RasterInfo valueRasterInfo = GetRasterValue2(ref valueRasterDataset, out valueRaster);
            RasterInfo zoneRasterInfo = GetRasterValue2(ref zoneRasterDataset, out zoneRaster);

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

        public static void ComputeZonalStatistics(string valueRasterName,string featureName, string featureFieldName, int cellSize)
        {
            //Step 1: Convert feature to raster
            //Step 2: Align/mask value raster with step1's output raster
            //Setp 3: Feed into ..

            Dataset alignedValueRaster;
            Dataset zoneRaster;

            RasterizeGdal.Rasterize(featureName, out zoneRaster, featureFieldName, cellSize);
            MaskRasterBoundary.ClipRaster(featureName, valueRasterName, cellSize, out alignedValueRaster);

            ValueAndZoneRasters(ref alignedValueRaster, ref zoneRaster);

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

           // Console.WriteLine(zonalValues);
            StatisticsExport writer = new StatisticsExport("mytest.csv");
            writer.ExportZonalStatistics(ref zonalValues);

        }
        
        private static RasterInfo GetRasterValue2(ref Dataset rasterDataset, out double[] rasterValues)
        {
            //raster size
            int rasterCols = rasterDataset.RasterXSize;
            int rasterRows = rasterDataset.RasterYSize;

            //Read 1st band from raster
            Band band = rasterDataset.GetRasterBand(1);
            int rastWidth = rasterCols;
            int rastHeight = rasterRows;

            //Need to find out algorithm to read memory block by block instead of reading a big chunk of data
            rasterValues = new double[rastWidth * rastHeight];
            band.ReadRaster(0, 0, rastWidth, rastHeight, rasterValues, rastWidth, rastHeight, 0, 0);

            var info = new RasterInfo { RasterHeight = rasterRows, RasterWidth = rasterCols };
            return info;
        }
        
        /*
        private static RasterInfo GetRasterValue(string rasterName, out double[] rasterValues)
        {
            //Register all drivers
            Gdal.AllRegister();

            //Read dataset
            Dataset rasterDataset = Gdal.Open(rasterName, Access.GA_ReadOnly);
            if (rasterDataset == null)
            {
                Console.WriteLine("Unable to read input raster..");
                System.Environment.Exit(-1);
            }

            //raster bands
            int bandCount = rasterDataset.RasterCount;
            if (bandCount > 1)
            {
                Console.WriteLine("Input error, please provide single band raster image only..");
                System.Environment.Exit(-1);
            }

            //raster size
            int rasterCols = rasterDataset.RasterXSize;
            int rasterRows = rasterDataset.RasterYSize;

            //Extract geotransform
            double[] geotransform = new double[6];
            rasterDataset.GetGeoTransform(geotransform);

            //Get raster bounding box
            double originX = geotransform[0];
            double originY = geotransform[3];
            double pixelWidth = geotransform[1];
            double pixelHeight = geotransform[5];

            //Read 1st band from raster
            Band band = rasterDataset.GetRasterBand(1);
            int rastWidth = rasterCols;
            int rastHeight = rasterRows;

            //Need to find out algorithm to read memory block by block instead of reading a big chunk of data
            rasterValues = new double[rastWidth * rastHeight];
            band.ReadRaster(0, 0, rastWidth, rastHeight, rasterValues, rastWidth, rastHeight, 0, 0);

            var info = new RasterInfo {RasterHeight = rasterRows, RasterWidth = rasterCols};
            return info;

        }
        */
        
        
    }
}

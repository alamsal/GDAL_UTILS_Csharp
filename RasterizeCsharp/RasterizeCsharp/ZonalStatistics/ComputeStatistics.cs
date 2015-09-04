using System;
using System.Collections.Generic;
using OSGeo.GDAL;

namespace RasterizeCsharp.ZonalStatistics
{
    class ComputeStatistics
    {
        private Dictionary<string, List<double>> zonalValues;

        public static void ReadValueAndZoneRasters(string valueRasterName,string zoneRasterName)
        {
            double[] valueRaster;
            double[] zoneRaster;

            RasterInfo valueRasterInfo = GetRasterValue(valueRasterName, out valueRaster);
            RasterInfo zoneRasterInfo = GetRasterValue(zoneRasterName, out zoneRaster);
            
            if(valueRasterInfo.RasterHeight!=zoneRasterInfo.RasterHeight || valueRasterInfo.RasterWidth != zoneRasterInfo.RasterWidth)
            {
                Console.WriteLine("Given input rasters have inconsistant width or height");
                System.Environment.Exit(-1);
            }else
            {
                CalculateZonalStatistics(ref valueRaster,ref zoneRaster,valueRasterInfo);
            }

        }

        private static void CalculateZonalStatistics(ref double[] valueRaster, ref double []zoneRaster, RasterInfo rasterInfo)
        {
            //Do data processing for raster
            for (int col = 0; col < rasterInfo.RasterWidth; col++)
            {
                for (int row = 0; row < rasterInfo.RasterHeight; row++)
                {
                    double rasterPixelValue = valueRaster[col + row * rasterInfo.RasterWidth];
                    Console.WriteLine(rasterPixelValue + " X: " + col + " Y:" + row);
                }
            }
        }

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

        
    }
}

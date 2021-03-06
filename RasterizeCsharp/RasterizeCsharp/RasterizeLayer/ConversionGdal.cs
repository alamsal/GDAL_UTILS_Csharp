﻿using System;
using System.IO;
using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;
using RasterizeCsharp.AppUtils;

namespace RasterizeCsharp.RasterizeLayer
{
    class ConversionGdal
    {
        public static void ConvertFeatureToRaster(Layer layer, out Dataset outputDataset, double rasterCellSize, string fieldName)
        {
            DriverUtils.RegisterGdalOgrDrivers();

            Envelope envelope = new Envelope();
            layer.GetExtent(envelope, 0);

            //Compute the out raster cell resolutions
            int x_res = Convert.ToInt32((envelope.MaxX - envelope.MinX) / rasterCellSize);
            int y_res = Convert.ToInt32((envelope.MaxY - envelope.MinY) / rasterCellSize);

            //Console.WriteLine("Extent: " + envelope.MaxX + " " + envelope.MinX + " " + envelope.MaxY + " " + envelope.MinY);
            //Console.WriteLine("X resolution: " + x_res);
            //Console.WriteLine("X resolution: " + y_res);

            //Create new tiff in disk
            string tempRaster = "tempZoneRaster.tif";
            if (File.Exists(tempRaster))
            {
                File.Delete(tempRaster);
            }
            OSGeo.GDAL.Driver outputDriver = Gdal.GetDriverByName("GTiff");
            outputDataset = outputDriver.Create(tempRaster, x_res, y_res, 1, DataType.GDT_Int32, null);

            //Extrac srs from input feature 
            string inputShapeSrs;
            SpatialReference spatialRefrence = layer.GetSpatialRef();
            spatialRefrence.ExportToWkt(out inputShapeSrs);

            //Assign input feature srs to outpur raster
            outputDataset.SetProjection(inputShapeSrs);

            //Geotransform
            double[] argin = new double[] { envelope.MinX, rasterCellSize, 0, envelope.MaxY, 0, -rasterCellSize };
            outputDataset.SetGeoTransform(argin);

            //Set no data
            Band band = outputDataset.GetRasterBand(1);
            //band.SetNoDataValue(GdalUtilConstants.NoDataValue);
            band.Fill(GdalUtilConstants.NoDataValue, 0.0);

            //Feature to raster rasterize layer options

            //No of bands (1)
            int[] bandlist = new int[] { 1 };

            //Values to be burn on raster (10.0)
            double[] burnValues = new double[] { 10.0 };
            //Dataset myDataset = Gdal.Open(outputRasterFile, Access.GA_Update);

            //additional options

            string[] rasterizeOptions;
            //rasterizeOptions = new string[] { "ALL_TOUCHED=TRUE", "ATTRIBUTE=" + fieldName }; //To set all touched pixels into raster pixel

            rasterizeOptions = new string[] { "ATTRIBUTE=" + fieldName };

            //Rasterize layer
            //Gdal.RasterizeLayer(myDataset, 1, bandlist, layer, IntPtr.Zero, IntPtr.Zero, 1, burnValues, null, null, null); // To burn the given burn values instead of feature attributes
            //Gdal.RasterizeLayer(outputDataset, 1, bandlist, layer, IntPtr.Zero, IntPtr.Zero, 1, burnValues, rasterizeOptions, new Gdal.GDALProgressFuncDelegate(ProgressFunc), "Raster conversion");

            Gdal.RasterizeLayer(outputDataset, 1, bandlist, layer, IntPtr.Zero, IntPtr.Zero, 1, burnValues, rasterizeOptions, null, null);
        }

        private static int ProgressFunc(double complete, IntPtr message, IntPtr data)
        {
           Console.Write("Processing ... " + complete * 100 + "% Completed.");
	       if (message != IntPtr.Zero)
	       {
               Console.Write(" Message:" + System.Runtime.InteropServices.Marshal.PtrToStringAnsi(message));
	       }
            
	       if (data != IntPtr.Zero)
	       {
               Console.Write(" Data:" + System.Runtime.InteropServices.Marshal.PtrToStringAnsi(data));
	       }
       
            Console.WriteLine("");
            return 1;
        }
    }
}

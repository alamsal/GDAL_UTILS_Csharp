using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;

namespace RasterizeCsharp
{
    class Program
    {
        static void Main(string[] args)
        {

            // Define pixel_size and NoData value of new raster
            const int pixel_size = 25;
            const double NoData_value = -9999;

            //Register the vector drivers
            Ogr.RegisterAll();

            //Reading the vector data
            DataSource dataSource = Ogr.Open(@"D:\Ashis_Work\GDAL Utilities\sample-data\UtahBoundary.shp", 0);

            Layer layer = dataSource.GetLayerByName("UtahBoundary");

            Envelope envelope = new Envelope();
            layer.GetExtent(envelope, 0);

            //Register the raster drivers
            Gdal.AllRegister();

            //Check if test.tif exists
            string outputFile = "test.tif";


            int x_res = (int)(envelope.MaxX - envelope.MinX) / pixel_size;
            int y_res = (int)(envelope.MaxY - envelope.MinY) / pixel_size;


            string input_srs;
            SpatialReference spatialRefrence = layer.GetSpatialRef();
            spatialRefrence.ExportToWkt(out input_srs);

            Console.WriteLine(input_srs);
            Console.WriteLine("");
            //Console.WriteLine("Extent: " + envelope.MaxX + " " + envelope.MinX + " " + envelope.MaxY + " " + envelope.MinY);
            Console.WriteLine("X resolution: " + x_res);
            Console.WriteLine("X resolution: " + y_res);

            /*
            string[] options;
            options = new string[] { "BLOCKXSIZE=" + 100, "BLOCKYSIZE=" + 10 };
            Dataset outputDataset = Gdal.GetDriverByName("GTiff").Create(outputFile, x_res, y_res, 1, DataType.GDT_UInt16, options);
            Band band = outputDataset.GetRasterBand(1);
            band.SetNoDataValue(NoData_value);





            //Define spatial reference 
            SpatialReference spatialReference = layer.GetSpatialRef();
            string srs_wkt;
            spatialReference.ExportToWkt(out srs_wkt);
            outputDataset.SetProjection(srs_wkt);
            */

            int[] bandlist = new int[] { 1 };

            //bandlist[0] = 1;

            double[] burnValues = new double[] { 10.0 };

            Dataset myDataset = Gdal.Open("test.tif", Access.GA_Update);
            //myDataset.SetProjection(srs_wkt);

            string[] rasterizeOptions;
            rasterizeOptions = new string[] { "ATTRIBUTE=Shape_Area" };

            //Rasterize
            //Gdal.RasterizeLayer(outputDataset,0, bandlist, layer, IntPtr.Zero, IntPtr.Zero, 0, null, null, null, null); //Working

            Gdal.RasterizeLayer(myDataset, 1, bandlist, layer, IntPtr.Zero, IntPtr.Zero, 1, burnValues, null, null, null);



            //Gdal.RasterizeLayer(outputDataset, 1, bandlist, layer, IntPtr.Zero, IntPtr.Zero,1,burnValues, null, null, null);

            //Gdal.RasterizeLayer(outputDataset,1,bandlist, layer, IntPtr.Zero, IntPtr.Zero, 0, null, null, null, null);

            Console.WriteLine("Fill no data values ..");

            Console.Write("Done ..");
        }
    }
}

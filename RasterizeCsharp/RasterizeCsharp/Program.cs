using System;
using System.IO;
using ESRI.ArcGIS.ConversionTools;
using ESRI.ArcGIS.Geoprocessor;
using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;


namespace RasterizeCsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();

            string inputShapeFile = @"D:\Ashis_Work\GDAL Utilities\sample-data\UtahBoundary.shp";
            string fieldName = "Shape_Area";
            string outRasterNameEsri = @"D:\Ashis_Work\GDAL Utilities\sample-data\Utah_ESRI_30m.tif";
            string outRasterNameGdal = @"D:\Ashis_Work\GDAL Utilities\sample-data\Utah_gdal_30m.tif";
            int rasterCellSize = 30;

            program.VectorToRasterFromEsri(inputShapeFile, outRasterNameEsri, fieldName, rasterCellSize);
            program.VectorToRasterFromGdal(inputShapeFile, outRasterNameGdal, fieldName, rasterCellSize);

        }

        public void VectorToRasterFromGdal(string inputFeature, string outRaster, string fieldName, int cellSize)
        {
            // Define pixel_size and NoData value of new raster
            int rasterCellSize = cellSize;
            const double noDataValue = -9999;
            string outputRasterFile = outRaster;

            //Register the vector drivers
            Ogr.RegisterAll();

            //Reading the vector data
            DataSource dataSource = Ogr.Open(inputFeature, 0);
            Layer layer = dataSource.GetLayerByIndex(0);

            Envelope envelope = new Envelope();
            layer.GetExtent(envelope, 0);
            
            //Compute the out raster cell resolutions
            int x_res = Convert.ToInt32((envelope.MaxX - envelope.MinX) / rasterCellSize);
            int y_res = Convert.ToInt32((envelope.MaxY - envelope.MinY) / rasterCellSize);

            Console.WriteLine("Extent: " + envelope.MaxX + " " + envelope.MinX + " " + envelope.MaxY + " " + envelope.MinY);
            Console.WriteLine("X resolution: " + x_res);
            Console.WriteLine("X resolution: " + y_res);

            //Register the raster drivers
            Gdal.AllRegister();

            //Check if output raster exists & delete (optional)
            if(File.Exists(outputRasterFile))
            {
                File.Delete(outputRasterFile);
            }
            
            //Create new tiff 
            OSGeo.GDAL.Driver outputDriver = Gdal.GetDriverByName("GTiff");
            Dataset outputDataset = outputDriver.Create(outputRasterFile, x_res, y_res, 1, DataType.GDT_Float64, null);

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
            band.SetNoDataValue(noDataValue);
            
            //close tiff
            outputDataset.FlushCache();
            outputDataset.Dispose();

            //Feature to raster rasterize layer options
            
            //No of bands (1)
            int[] bandlist = new int[] { 1 }; 
            
            //Values to be burn on raster (10.0)
            double[] burnValues = new double[] { 10.0 };
            Dataset myDataset = Gdal.Open(outputRasterFile, Access.GA_Update);
            
            //additional options

            string[] rasterizeOptions;
            //rasterizeOptions = new string[] { "ALL_TOUCHED=TRUE", "ATTRIBUTE=Shape_Area" }; //To set all touched pixels into raster pixel

            rasterizeOptions = new string[] { "ATTRIBUTE="+fieldName };

            //Rasterize layer
            //Gdal.RasterizeLayer(myDataset, 1, bandlist, layer, IntPtr.Zero, IntPtr.Zero, 1, burnValues, null, null, null); // To burn the given burn values instead of feature attributes
            Gdal.RasterizeLayer(myDataset, 1, bandlist, layer, IntPtr.Zero, IntPtr.Zero, 1, burnValues, rasterizeOptions, null, null);

        }

        public void VectorToRasterFromEsri(string inputFeature,string outRaster,string fieldName, int cellSize)
        {
            try
            {
                //Runtime manager to find the ESRI product installed in the system
                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);

                Geoprocessor geoprocessor = new Geoprocessor();
                geoprocessor.OverwriteOutput = true;
                
                FeatureToRaster featureToRaster = new FeatureToRaster();
                featureToRaster.cell_size = cellSize;
                featureToRaster.in_features = inputFeature;
                featureToRaster.out_raster = outRaster;
                featureToRaster.field = fieldName;

                geoprocessor.Execute(featureToRaster, null);

            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

    }
}

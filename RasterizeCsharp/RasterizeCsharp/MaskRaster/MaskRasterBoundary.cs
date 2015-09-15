
using System;
using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;

using RasterizeCsharp.AppConstants;

namespace RasterizeCsharp.MaskRaster
{
    class MaskRasterBoundary
    {
        public static void AlignRaster(string featureName, string rasterName, int rasterCellSize)
        {
            
            //Register vector drivers
            Ogr.RegisterAll();

            //Reading the vector data
            DataSource dataSource = Ogr.Open(featureName, 0);
            Layer layer = dataSource.GetLayerByIndex(0);

            //Extrac srs from input feature 
            string inputShapeSrs;
            SpatialReference spatialRefrence = layer.GetSpatialRef();
            spatialRefrence.ExportToWkt(out inputShapeSrs);
            
            Envelope envelope = new Envelope();
            layer.GetExtent(envelope, 0);

            //Compute the out raster cell resolutions
            int x_res = Convert.ToInt32((envelope.MaxX - envelope.MinX) / rasterCellSize);
            int y_res = Convert.ToInt32((envelope.MaxY - envelope.MinY) / rasterCellSize);
            
            //Register vector drivers
            Gdal.AllRegister();
            
            Dataset oldRasterDataset = Gdal.Open(rasterName, Access.GA_ReadOnly);
           
            //Create new tiff 
            OSGeo.GDAL.Driver outputDriver = Gdal.GetDriverByName("GTiff");
            
            //New geotiff name
            string outputRasterFile = "mynewraster.tif";

            Dataset outputDataset = outputDriver.Create(outputRasterFile, x_res,y_res, 1, DataType.GDT_Float64, null);
            
            //Geotransform
            double[] argin = new double[] { envelope.MinX, rasterCellSize, 0, envelope.MaxY, 0, -rasterCellSize };
            outputDataset.SetGeoTransform(argin);

            
            //Set no data
            Band band = outputDataset.GetRasterBand(1);
            band.SetNoDataValue(RsacAppConstants.NO_DATA_VALUE);
            outputDataset.SetProjection(inputShapeSrs);

            string[] reprojectOptions = {"NUM_THREADS = ALL_CPUS"," INIT_DEST = NO_DATA","WRITE_FLUSH = YES" };

            Gdal.ReprojectImage(oldRasterDataset, outputDataset, null, inputShapeSrs, ResampleAlg.GRA_NearestNeighbour, 1.0,1.0, null, null, reprojectOptions);
            
            //flush cache
            outputDataset.FlushCache();
            band.FlushCache();
            dataSource.FlushCache();
            
        }
    }
}

using System;
using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;

using RasterizeCsharp.AppUtils;

namespace RasterizeCsharp.MaskRaster
{
    class MaskRasterBoundary
    {
        public static void ClipRaster(string featureName, string rasterName, double rasterCellSize, out Dataset outAlignedRaster)
        {
            //Register vector drivers
            Ogr.RegisterAll();

            //Reading the vector data
            DataSource vectorDataSource = Ogr.Open(featureName, 0);
            Layer layer = vectorDataSource.GetLayerByIndex(0);

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
           
            //Create new tiff in memory
            OSGeo.GDAL.Driver outputDriver = Gdal.GetDriverByName("GTiff");
            
            outAlignedRaster = outputDriver.Create("tempValueRaster.tif", x_res, y_res, 1, DataType.GDT_Float32, null);
            
            //Geotransform
            double[] argin = new double[] { envelope.MinX, rasterCellSize, 0, envelope.MaxY, 0, -rasterCellSize };
            outAlignedRaster.SetGeoTransform(argin);

            
            //Set no data
            Band band = outAlignedRaster.GetRasterBand(1);
            //band.SetNoDataValue(GdalUtilConstants.NoDataValue);
            outAlignedRaster.SetProjection(inputShapeSrs);

            band.Fill(GdalUtilConstants.NoDataValue, 0.0);

            string[] reprojectOptions = {"NUM_THREADS = ALL_CPUS","WRITE_FLUSH = YES" };

            Gdal.ReprojectImage(oldRasterDataset, outAlignedRaster, null, inputShapeSrs, ResampleAlg.GRA_NearestNeighbour,0.0, 0.0, null, null, reprojectOptions);
            
            //flush cache
            band.FlushCache();
            vectorDataSource.FlushCache();
            oldRasterDataset.FlushCache();

            band.Dispose();
            vectorDataSource.Dispose();
            oldRasterDataset.Dispose();
            
        }
    }
}

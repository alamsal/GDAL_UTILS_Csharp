﻿
using System;
using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;

using RasterizeCsharp.AppConstants;

namespace RasterizeCsharp.MaskRaster
{
    class MaskRasterBoundary
    {
        public static void ClipRaster(string featureName, string rasterName, int rasterCellSize, out Dataset outAlignedRaster)
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
           
            //Create new tiff 
            OSGeo.GDAL.Driver outputDriver = Gdal.GetDriverByName("MEM");
            
            //New geotiff name
            //string outputRasterFile = "mynewraster.tif";

            outAlignedRaster = outputDriver.Create("", x_res, y_res, 1, DataType.GDT_Float64, null);
            
            //Geotransform
            double[] argin = new double[] { envelope.MinX, rasterCellSize, 0, envelope.MaxY, 0, -rasterCellSize };
            outAlignedRaster.SetGeoTransform(argin);

            
            //Set no data
            Band band = outAlignedRaster.GetRasterBand(1);
            band.SetNoDataValue(RsacAppConstants.NO_DATA_VALUE);
            outAlignedRaster.SetProjection(inputShapeSrs);

            string[] reprojectOptions = {"NUM_THREADS = ALL_CPUS"," INIT_DEST = NO_DATA","WRITE_FLUSH = YES" };

            Gdal.ReprojectImage(oldRasterDataset, outAlignedRaster, null, inputShapeSrs, ResampleAlg.GRA_NearestNeighbour, 1.0, 1.0, null, null, reprojectOptions);
            
            //flush cache
            band.FlushCache();
            vectorDataSource.FlushCache();
            oldRasterDataset.FlushCache();
            
        }
    }
}

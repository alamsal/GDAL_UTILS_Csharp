
using System;
using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;


namespace RasterizeCsharp.MaskRaster
{
    class MaskRasterBoundary
    {
        public static void CorrectRaster(string featureName, string rasterName)
        {
            //https://trac.osgeo.org/gdal/ticket/1469
            
            //vector operations
            Ogr.RegisterAll();

            //Reading the vector data
            DataSource dataSource = Ogr.Open(featureName, 0);
            Layer layer = dataSource.GetLayerByIndex(0);

            //Extrac srs from input feature 
            string inputShapeSrs;
            SpatialReference spatialRefrence = layer.GetSpatialRef();
            spatialRefrence.ExportToWkt(out inputShapeSrs);


            string outputRasterFile = "myraster_reproject.tif";
            int rasterCellSize = 3;
            double noDataValue = -9;
            Envelope envelope = new Envelope();
            layer.GetExtent(envelope, 0);

            //Compute the out raster cell resolutions
            int x_res = Convert.ToInt32((envelope.MaxX - envelope.MinX) / rasterCellSize);
            int y_res = Convert.ToInt32((envelope.MaxY - envelope.MinY) / rasterCellSize);


            string wkt = "POLYGON((548558.38 3511309.36,561779.64 3511309.36, 561779.64 3532510.63, 548558.38 3532510.63,  548558.38 3511309.36))";
           
            

            //raster operations
            Gdal.AllRegister();
            
            Dataset oldRasterDataset = Gdal.Open(rasterName, Access.GA_ReadOnly);
            
            
            string oldRasterSrs = oldRasterDataset.GetProjection();

            Dataset warpedDataset = Gdal.AutoCreateWarpedVRT(oldRasterDataset, oldRasterSrs, inputShapeSrs,ResampleAlg.GRA_NearestNeighbour, 0.125);
            
            OSGeo.GDAL.Driver outRaster = Gdal.GetDriverByName("GTiff");

            //saving vrt
            Dataset outVrt = warpedDataset.GetDriver().CreateCopy("test_vrt.vrt", warpedDataset, 1, null,null,null);
            
            //Saving the warped image
            Dataset outDataset = outRaster.CreateCopy("test_vrt.tif", warpedDataset, 0, null,null,null);
            

            


            //Create new tiff 
            OSGeo.GDAL.Driver outputDriver = Gdal.GetDriverByName("GTiff");
            Dataset outputDataset = outputDriver.Create(outputRasterFile, x_res,y_res, 1, DataType.GDT_Float64, null);
            
            //Geotransform
            double[] argin = new double[] { envelope.MinX, rasterCellSize, 0, envelope.MaxY, 0, -rasterCellSize };
            outputDataset.SetGeoTransform(argin);

            
            //Set no data
            Band band = outputDataset.GetRasterBand(1);
            band.SetNoDataValue(noDataValue);
            outputDataset.SetProjection(inputShapeSrs);

            band.Fill(noDataValue,0.0);

            //string[] reprojectOptions = { "CUTLINE = POLYGON((548558.38 3511309.36,561779.64 3511309.36, 561779.64 3532510.63, 548558.38 3532510.63,  548558.38 3511309.36))", 
            //                               "NUM_THREADS = ALL_CPUS"," INIT_DEST=NO_DATA","WRITE_FLUSH = YES","UNIFIED_SRC_NODATA=YES" }; 


            //string[] reprojectOptions = {"NUM_THREADS = ALL_CPUS"," INIT_DEST=NO_DATA","WRITE_FLUSH = YES","UNIFIED_SRC_NODATA=YES" }; 



            Gdal.ReprojectImage(oldRasterDataset, outputDataset, null, inputShapeSrs, ResampleAlg.GRA_NearestNeighbour, 100.0,
                                 100.0, null, null, null);


            //Gdal.ReprojectImage()

            /*
            warpedDataset.FlushCache();
            outVrt.FlushCache();
            outDataset.FlushCache();
            */
        }
    }
}

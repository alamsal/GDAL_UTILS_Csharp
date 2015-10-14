using System;
using System.IO;
using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;

using RasterizeCsharp.AppUtils;

namespace RasterizeCsharp.MaskRaster
{
    class MaskRasterBoundary
    {
        public static void ClipRasterUsingFeature(string inFeatureName, string inRasterName, double rasterCellSize, out Dataset outAlignedRaster)
        {
            DriverUtils.RegisterGdalOgrDrivers();
            ReadFeature readFeature = new ReadFeature(inFeatureName);
            Layer layer = readFeature.GetFeatureLayer();

            StartClippingProcess(layer,inRasterName,rasterCellSize,out outAlignedRaster);

        }

        public static void ClipRasterUsingGdbFeature(string gdbPath, string inFeatureLayerName, string inRasterName, double rasterCellSize, out Dataset outAlignedRaster)
        {
            DriverUtils.RegisterGdalOgrDrivers();
            ReadFeature readFeature = new ReadFeature(gdbPath, inFeatureLayerName);
            Layer layer = readFeature.GetFeatureLayer();

            StartClippingProcess(layer, inRasterName, rasterCellSize, out outAlignedRaster);
        }

        private static void StartClippingProcess(Layer layer, string rasterName, double rasterCellSize, out Dataset outAlignedRaster)
        {
            //Extrac srs from input feature 
            string inputShapeSrs;
            SpatialReference spatialRefrence = layer.GetSpatialRef();
            spatialRefrence.ExportToWkt(out inputShapeSrs);

            Envelope envelope = new Envelope();
            layer.GetExtent(envelope, 0);

            //Compute the out raster cell resolutions
            int x_res = Convert.ToInt32((envelope.MaxX - envelope.MinX) / rasterCellSize);
            int y_res = Convert.ToInt32((envelope.MaxY - envelope.MinY) / rasterCellSize);

            Dataset oldRasterDataset = Gdal.Open(rasterName, Access.GA_ReadOnly);

            //No of bands in older dataset
            int rasterBands = oldRasterDataset.RasterCount;

            //Create new tiff in disk
            string tempRaster = "tempValueRaster.tif";
            if (File.Exists(tempRaster))
            {
                File.Delete(tempRaster);
            }
            OSGeo.GDAL.Driver outputDriver = Gdal.GetDriverByName("GTiff");
            outAlignedRaster = outputDriver.Create(tempRaster, x_res, y_res, rasterBands, DataType.GDT_Float32, null);

            //Geotransform
            double[] argin = new double[] { envelope.MinX, rasterCellSize, 0, envelope.MaxY, 0, -rasterCellSize };
            outAlignedRaster.SetGeoTransform(argin);


            //Set no data
            for (int rasBand = 1; rasBand <= rasterBands; rasBand++)
            {
                Band band = outAlignedRaster.GetRasterBand(rasBand);
                band.Fill(GdalUtilConstants.NoDataValue, 0.0);
            }

            //band.SetNoDataValue(GdalUtilConstants.NoDataValue);
            outAlignedRaster.SetProjection(inputShapeSrs);



            string[] reprojectOptions = { "NUM_THREADS = ALL_CPUS", "WRITE_FLUSH = YES" };

            Gdal.ReprojectImage(oldRasterDataset, outAlignedRaster, null, inputShapeSrs, ResampleAlg.GRA_NearestNeighbour, 0.0, 0.0, null, null, reprojectOptions);

            //flush cache
            oldRasterDataset.FlushCache();
            oldRasterDataset.Dispose();
        }


    }



}

using System;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.esriSystem;


namespace RasterizeCsharp.MaskRaster
{
    class ClipRasterBoundaryEsri
    {
        public static void ClipRaster(string inRaster, string inClipFeature, string outTempRaster)
        {
            Clip clipTool = new Clip();

            //set clip parameters
            clipTool.in_raster = inRaster;
            clipTool.out_raster = outTempRaster;

            //clip extent
            clipTool.in_template_dataset = inClipFeature;

            Geoprocessor gp = new Geoprocessor();
            gp.OverwriteOutput = true;
            gp.AddOutputsToMap = false;
            
            try
            {


               IGeoProcessorResult result = (IGeoProcessorResult)gp.ExecuteAsync(clipTool);

               while(result.Status != esriJobStatus.esriJobSucceeded)
               {
                   //Console.WriteLine(result.Status.ToString());
                   System.Threading.Thread.Sleep(100);
               }

            }
            catch (Exception ex)
            {
                object level = 0;
                Console.WriteLine(gp.GetMessages(ref level));
                Console.WriteLine(" Failed to clip raster using ESRI clip tool " + ex.Message);
            }

        }

    }
}

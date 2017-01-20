using System;
using System.IO;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using OSGeo.OGR;
using RasterizeCsharp.AppUtils;
using Feature = OSGeo.OGR.Feature;
using FieldType = OSGeo.OGR.FieldType;

namespace RasterizeCsharp.ShapeField
{
    class AttributeField
    {
        public void AddAttributeField(string oldFeatureFile)
        {
            DriverUtils.RegisterOgrDriver();
            DataSource dataSource;
            Layer layer;

            var isShapeFile = IsShapeInGdb(oldFeatureFile);
            
            if (isShapeFile)
            {
                dataSource = Ogr.Open(oldFeatureFile, 1); //second argument in open specifies mode of data, 1 RW & 0 readonly mode
                layer = dataSource.GetLayerByIndex(0);
                var layerDefn = layer.GetLayerDefn();

                int attrFieldIndex = (int)layerDefn.GetFieldIndex("FID_GDAL");
                if (attrFieldIndex == -1)
                {
                    FieldDefn gdalFiedlDefn = new FieldDefn("FID_GDAL", FieldType.OFTInteger);

                    layer.CreateField(gdalFiedlDefn, 0);

                    Feature feature = layer.GetNextFeature();

                    while (feature != null)
                    {
                        feature.SetField("FID_GDAL", feature.GetFID()); // Add FID shapefile
                        layer.SetFeature(feature);
                        feature = layer.GetNextFeature();
                    }
                    layer.SyncToDisk();
                  }
                layer.Dispose();
                dataSource.FlushCache();

            }
            else
            {
                try
                {
                    EnableEsriLiscences();

                    string gdbPath = Path.GetDirectoryName(oldFeatureFile);
                    string featureName = Path.GetFileNameWithoutExtension(oldFeatureFile);

                    IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactory();
                    IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile(gdbPath, 1);
                    IFeatureClass featureClass = featureWorkspace.OpenFeatureClass(featureName);

                    IFields fields = featureClass.Fields;
                    
                    if (fields.FindField("FID_GDAL") == -1)
                    {
                        // Create a Int field called "FID_GDAL" for the fields collection
                        IField gdalField = new FieldClass();
                        IFieldEdit gField = (IFieldEdit)gdalField;
                        gField.Name_2 = "FID_GDAL";
                        gField.Type_2 = esriFieldType.esriFieldTypeInteger;
                        //fieldsEdit.AddField(gField);

                        featureClass.AddField(gdalField);

                    }

                    

                    IFeatureCursor featureCursor = featureClass.Search(null, false);
                    IFeature feature = featureCursor.NextFeature();
                    while (feature != null)
                    {
                        Console.WriteLine(feature.OID);
                        Console.WriteLine(feature.Fields.FindField("FID_GDAL"));
                        
                        

                        feature.set_Value(feature.Fields.FindField("FID_GDAL"),feature.OID); //  [feature.Fields.FindFieldByAliasName("FID_GDAL")]
                        feature.Store();
                        Console.WriteLine(feature.OID);
                        feature = featureCursor.NextFeature();
                    }
                    
                    featureCursor.Flush();
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

               
            }
            

        }

        private static bool IsShapeInGdb(string oldFeatureFile)
        {
            bool hasExtension = Path.HasExtension(oldFeatureFile);
            return hasExtension;
        }
        private static void EnableEsriLiscences()
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);
            ESRI.ArcGIS.RuntimeManager.BindLicense(ESRI.ArcGIS.ProductCode.Desktop);

            UID pUid = new UIDClass();
            pUid.Value = "esriSpatialAnalystUI.SAExtension";

            // Add Spatial Analyst extension to the license manager.
            object v = null;
            IExtensionManagerAdmin extensionManagerAdmin = new ExtensionManagerClass();
            extensionManagerAdmin.AddExtension(pUid, ref v);

            // Enable the license.
            IExtensionManager extensionManager = (IExtensionManager)extensionManagerAdmin;
            IExtension extension = extensionManager.FindExtension(pUid);
            IExtensionConfig extensionConfig = (IExtensionConfig)extension;

            if (extensionConfig.State != esriExtensionState.esriESUnavailable)
            {
                extensionConfig.State = esriExtensionState.esriESEnabled;
            }
            else
            {
                Console.WriteLine("No Spatial Analyst License available");
            }

        }
       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Microsoft.VisualStudio.Tools.Applications;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Sql;
using Odp.Data.Views;
using Odp.InteractiveSdk.Mvc.Models;
using Odp.InteractiveSdk.Mvc.Repository;
using Odp.InteractiveSdk.Mvc.Models.Rating;
using Resources;

namespace Odp.InteractiveSdk.Mvc.Controllers
{
    public class DataLoaderController : Controller
    {
        DataLoaderModel model = new DataLoaderModel();

        //
        // GET: /DataLoader/        
        public ActionResult Index(string container, string entitySetName)
        {
            model = new DataLoaderModel();
            model.OtherModel = new DatasetListModel(0, 0, new OrderByInfo(), null, null);

            if (container == string.Empty || entitySetName == string.Empty)
            {
                model.NewRecord = true;
            }
            else
            {
                //Set the ViewData for controls according to queryString
                LoadControls(container, entitySetName);

                model.Container = container;
                model.EntitySetName = entitySetName;
                model.Name = model.ViewDataModel.EntitySetWrapper.EntitySet.Name;
                model.Category = model.ViewDataModel.EntitySetWrapper.EntitySet.CategoryValue;
                model.DataSource = model.ViewDataModel.EntitySetWrapper.EntitySet.ContainerAlias;
                model.Description = model.ViewDataModel.EntitySetWrapper.EntitySet.Description;
                model.Table = model.ViewDataModel.EntitySetWrapper.EntitySet.EntitySetName;
            }

            return View("Index", System.Configuration.ConfigurationManager.AppSettings["MasterPageName"], model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(DataLoaderModel model)
        {
            var sql = Odp.Data.Sql.sqlServerConnection.GetDataServiceInstance();
            string error = string.Empty;

            this.model = model;
            model.OtherModel = new DatasetListModel(0, 0, new OrderByInfo(), null, null);

            // ***
            // Error Checking Tree
            model.ErrorMessage = string.Empty;
            if (model.Name == string.Empty || model.Name == null)
                model.ErrorMessage += "Name,";
            if (model.DataSource == string.Empty || model.DataSource == null)
                model.ErrorMessage += "Data Source,";
            if (model.Category == string.Empty || model.Category == null)
                model.ErrorMessage += "Category,";

            if (model.NewRecord)
            {
            }

            // ***
            // If everything is ok, begin saving the record
            if (model.ErrorMessage == string.Empty)
            {
                // ***
                // Save File to Webserver
                if (model.File.HasFile())
                {
                    model.FileLocation = Server.MapPath("//DataFiles") + model.File.FileName;
                    model.File.SaveAs(model.FileLocation);
                    model.FileExtention = System.IO.Path.GetExtension(model.FileLocation).Replace(".", "").ToLower();
                }

                // ***
                // Save file processing
                if (model.Save == "Save")
                {
                    if(sql.UpdateMetaData(model.Name, model.Table, model.DataSource, model.Category, model.Description, model.Icon))
                        return RedirectToAction("DataSetList", "DataCatalog");
                    else
                        return View("Index", System.Configuration.ConfigurationManager.AppSettings["MasterPageName"], model);
                }
                else if (model.Save == "Upload")
                {
                    if (model.FileExtention == "kml" || model.FileExtention == "kmz" || model.FileExtention == "shp" || model.FileExtention == "rss" || model.FileExtention == "rss")
                    {
                        model.UploadOnly = true;
                        model.Table = "DownloadedFile";
                        model.EntitySetName = "DownloadedFile"; 
                        
                        if (sql.InsertMetaData(model.Name, model.Table, model.DataSource, model.Category, model.Icon, model.Description, "/DataFiles/" + model.File.FileName, model.FileExtention.ToUpper()))
                            return RedirectToAction("DataSetList", "DataCatalog");
                        else
                            return View("Index", System.Configuration.ConfigurationManager.AppSettings["MasterPageName"], model);
                    }
                    else
                    {
                        model.ErrorMessage = "You can only upload KML, KMZ, SHP, RSS or XLS files.";
                        return View("Index", System.Configuration.ConfigurationManager.AppSettings["MasterPageName"], model);
                    }
                }
                else if (model.Save == "Import")
                {
                    if (model.FileExtention == "csv" || model.FileExtention == "txt")
                    {
                        model.UploadOnly = false;
                        model.DataSet = UploadFile(model.FileLocation, model.HasHeaders, model.Delimiter);
                    }
                    else
                    {
                        model.ErrorMessage = "You can only import CSV or TXT delimted files.";
                        return View("Index", System.Configuration.ConfigurationManager.AppSettings["MasterPageName"], model);
                    }

                    if (model.FileLocation != string.Empty && model.FileLocation != null)
                    {
                        model.DataSet = UploadFile(model.FileLocation, model.HasHeaders, model.Delimiter);
                    }
                }
            }

            return View("Index", System.Configuration.ConfigurationManager.AppSettings["MasterPageName"], model);
        }


        private object UploadFile(string file, bool hasHeader, string delimiter)
        {
            object retVal = null;

            try
            {
                using (GenericParsing.GenericParser rawFileParser = new GenericParsing.GenericParser(file))
                {
                    rawFileParser.SetDataSource(file);
                    rawFileParser.ColumnDelimiter = delimiter == "Comma" ? ',' : '\t';
                    rawFileParser.FirstRowHasHeader = hasHeader;
                    rawFileParser.SkipStartingDataRows = 10;
                    rawFileParser.MaxBufferSize = 4096;
                    rawFileParser.MaxRows = -1;
                    rawFileParser.TextQualifier = '\"';

                    Type rowType = null;
                    rawFileParser.Read();
                    rowType = CreateRowType(rawFileParser);

                    Type[] types = new Type[] { rowType };
                    Type listType = typeof(List<>);
                    Type dataTableType = listType.MakeGenericType(types);
                    object allData = Activator.CreateInstance(dataTableType);

                    do
                    {
                        object row = null;
                        row = Activator.CreateInstance(rowType);

                        for (int col = 0; col < rawFileParser.ColumnCount; col++)
                        {
                            string propertyName = rawFileParser.GetColumnName(col);
                            rowType.GetProperty(propertyName).SetValue(row, rawFileParser[propertyName]);
                        }

                        dataTableType.GetMethod("Add").Invoke(allData, new object[] { row });

                    } while (rawFileParser.Read());

                    retVal = allData;
                }
            }
            catch (Exception ex)
            {
                Odp.Data.ErrorLog.WriteError(ex.Message);
            }

            return retVal;
        }


        private static Type CreateRowType(GenericParsing.GenericParser parser)
        {
            // create a dynamic assembly and module 
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "tmpAssembly";
            AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder module = assemblyBuilder.DefineDynamicModule("tmpModule");

            // create a new type builder
            TypeBuilder typeBuilder = module.DefineType("BindableRowCellCollection", TypeAttributes.Public | TypeAttributes.Class);
            for (int col = 0; col < parser.ColumnCount; col++)
            {
                string propertyName = parser.GetColumnName(col);

                // Generate a private field
                FieldBuilder field = typeBuilder.DefineField("_" + propertyName, typeof(string), FieldAttributes.Private);

                // Generate a public property
                PropertyBuilder property = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, typeof(string), new Type[] { typeof(string) });

                MethodAttributes GetSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig;

                // Define the "get" accessor method for current private field.
                MethodBuilder currGetPropMthdBldr =
                    typeBuilder.DefineMethod("get_value",
                                               GetSetAttr,
                                               typeof(string),
                                               Type.EmptyTypes);

                // Intermediate Language stuff...
                ILGenerator currGetIL = currGetPropMthdBldr.GetILGenerator();
                currGetIL.Emit(OpCodes.Ldarg_0);
                currGetIL.Emit(OpCodes.Ldfld, field);
                currGetIL.Emit(OpCodes.Ret);

                // Define the "set" accessor method for current private field.
                MethodBuilder currSetPropMthdBldr =
                    typeBuilder.DefineMethod("set_value",
                                               GetSetAttr,
                                               null,
                                               new Type[] { typeof(string) });

                // Again some Intermediate Language stuff...
                ILGenerator currSetIL = currSetPropMthdBldr.GetILGenerator();
                currSetIL.Emit(OpCodes.Ldarg_0);
                currSetIL.Emit(OpCodes.Ldarg_1);
                currSetIL.Emit(OpCodes.Stfld, field);
                currSetIL.Emit(OpCodes.Ret);

                // Last, we must map the two methods created above to our PropertyBuilder to 
                // their corresponding behaviors, "get" and "set" respectively. 
                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
            }

            return typeBuilder.CreateType();
        }

        private void LoadControls(string container, string entitySetName)
        {
            model.ViewDataModel = new DataBrowserModel();
            model.ViewDataModel.EntitySetName = entitySetName;
            model.ViewDataModel.Container = container;

            EntitySet entitySet = EntitySetRepository.GetEntitySet(container, entitySetName);

            var viewDs = new DatasetInfoDataSource();
            var views = viewDs.GetAnalyticSummary(Helper.GenerateDatasetItemKey(entitySet.ContainerAlias, entitySet.EntitySetName));

            model.ViewDataModel.EntitySetWrapper = new EntitySetWrapper()
            {
                EntitySet = entitySet,
                PositiveVotes = views.PositiveVotes,
                NegativeVotes = views.NegativeVotes,
                Views = views.views_total
            };

            if (!entitySet.IsDownloadOnly && !entitySet.IsEmpty)
            {
                EntitySetDetails metaDataDetails = EntitySetDetailsRepository.GetMetaData(container, entitySetName);
                model.ViewDataModel.EntitySetDetails = metaDataDetails;
            }
        }

    }
}

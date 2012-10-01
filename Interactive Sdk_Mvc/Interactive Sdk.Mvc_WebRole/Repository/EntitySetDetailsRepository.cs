
/* 
 *   Copyright (c) Microsoft Corporation.  All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of Microsoft Corporation nor the names of its contributors 
 *       may be used to endorse or promote products derived from this software
 *       without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE REGENTS AND CONTRIBUTORS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE REGENTS AND CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Odp.InteractiveSdk.Mvc.Models;
using Odp.InteractiveSdk.Mvc;
using Odp.InteractiveSdk.Mvc.Repository;
using Odp.Azure;

namespace Odp.InteractiveSdk.Mvc.Repository
{
    internal class EntitySetDetailsRepository
    {
        #region Constructors

        // Fxcop : added private constructor to prevent the compiler from generating
        // a default constructor.
        /// <summary>
        /// Default Constructor
        /// </summary>
        private EntitySetDetailsRepository()
        {
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Fetches the EntitySetDetails and returns EntitySetDetails object
        /// XML format will be different depending on tableName parameter.
        /// </summary>
        /// <param name="container">Container: Alias</param>
        /// <param name="tableName">EntitySetName</param>
        /// <param name="filter">Filter Parameters</param>
        /// <param name="pageSize">PageSize - For Paging Purpose</param>
        /// <param name="nextPartitionKey">Next Partition Key - 
        /// For Paging Purpose</param>
        /// <param name="nextRowKey">Next Row Key - For Paging Purpose</param>
        /// <param name="isFullData">true if required full data else false</param>
        /// <returns>returns an object of EntitySetDetails</returns>
        internal static EntitySetDetails GetBrowserData(string container,
            string tableName, string filter, int pageSize, string nextPartitionKey,
            string nextRowKey, bool isFullData, int skip)
        {
            // Declare object of class EntitySetDetails
            EntitySetDetails entitySetDetails = null;

            // Validatie the parameters
            if ((!String.IsNullOrEmpty(container)) &&
                !(String.IsNullOrEmpty(tableName))
               && pageSize > 0)
            {

                // Create an instance of class Storage
                IsdkStorageProviderInterface storage = Helper.ServiceObject;

                // Define entitySetDetails
                entitySetDetails = new EntitySetDetails();

                // Set the properties of entitySetDetails object
                entitySetDetails.ContainerAlias = container;
                entitySetDetails.EntitySetName = tableName;

                // Set the filter
                string tableNameFilter = "entityset eq '" + tableName + "'";

                //sl-king
                //TODO make this a reusable code (find all sfdgsdfgsdfgd)
                EntitySet es = EntitySetRepository.GetEntitySet(container, tableName);
                //since this is OData address looks like this .../service/category/entityset
                //metadata is at address .../service/category/$metadata
                string realUri = es.DataSvcLink;

                //first get rid of ?option1=...&option2=....
                var uriParts = realUri.Split('?');
                realUri = uriParts[0];
                if (realUri[realUri.Length - 1] != '/')//if it's not / terminated, let's terminate it
                    realUri = realUri + "/";

                string metadataUri = realUri;
                uriParts = metadataUri.Split('/');
                metadataUri = uriParts[0];
                for (int i = 1; i < uriParts.Length - 2; i++)
                {
                    metadataUri += "/" + uriParts[i];
                }
                metadataUri += "/$metadata";

                // Fetches the data from Azure Table Storage
                XElement metaDataXML = storage.GetMetadata(metadataUri, container, tableName);//, tableNameFilter);

                // Remove the unnecessary columns

                //sl-king: why is this done twice??? see GetMetaData
                var entityType = metaDataXML.Element("EntityType");
                var propertyMetaData = entityType.Elements("Property");//properties");
                //properties.Elements("entityset").Remove();
                //properties.Elements("entitykind").Remove();

                // Set the column list
                //var propertyMetaData = metaDataXML.Elements("Property");//Elements("properties").First().Elements();//properties

                // Add the column names in the detailsTable of the object entitySetDetails
                foreach (var property in propertyMetaData)
                {
                    if (property.Attribute("Name").Value.ToLower() == "partitionkey" ||
                            property.Attribute("Name").Value.ToLower() == "rowkey" ||
                            property.Attribute("Name").Value.ToLower() == "timestamp")
                        continue;

                    string sName = property.Attribute("Name").Value;
                    if (sName == "entityid")//property.Name
                    {
                        entitySetDetails.DetailsTable.Columns.Add(sName, Type.GetType("System.Guid"));
                    }
                    else
                    {
                        string sType = property.Attribute("Type").Value;
                        sType = sType.Replace("Edm.", "System.");

                        entitySetDetails.DetailsTable.Columns.Add(sName, Type.GetType(sType));//property.Value));
                    }
                }

                // Get the browser data
                XElement browserDataXML = null;
                if (isFullData == false)
                {
                    browserDataXML = storage.GetData(realUri, container, tableName,
                             filter, pageSize, nextPartitionKey, nextRowKey, skip);

                    int read = browserDataXML.Elements("entry").Count();
                    //for now let's just reuse the old mechanism with dummy values
                    if (read == pageSize)
                    {
                        entitySetDetails.NextPartitionKey = "dummypartitionkey";
                        entitySetDetails.NextRowKey = "dummyrowkey";
                    }
                    else
                    {
                        entitySetDetails.NextPartitionKey = "";
                        entitySetDetails.NextRowKey = "";
                    }
                    entitySetDetails.Skip += skip + read;

                    // set the properties of entitySetDetails object depending on the
                    // fetched results
                    //entitySetDetails.NextPartitionKey = 
                    //     browserDataXML.Attribute("nextPartitionKey").Value;

                    //entitySetDetails.NextRowKey = 
                    //     browserDataXML.Attribute("nextRowKey").Value;

                    //entitySetDetails.CurrentPartitionKey = 
                    //     browserDataXML.Attribute("currentPartitionKey").Value;

                    //entitySetDetails.CurrentRowKey =
                    //     browserDataXML.Attribute("currentRowKey").Value;
                }
                else
                {
                    browserDataXML = storage.GetData(realUri, container, tableName, filter);
                }

                // validate the XElement
                if (browserDataXML != null)
                {
                    // for each XML node, fetch the internal details

                    foreach (var element in browserDataXML.Elements("entry"))//properties"))
                    {
                        try
                        {
                            XElement content = element.Element("content");
                            XElement props = content.Element("properties");
                            // Get the row list for each elements
                            DataRow row = entitySetDetails.DetailsTable.NewRow();

                            // Add each cell in the row
                            foreach (var cell in props.Elements())//element.Elements()
                            {
                                try
                                {
                                    row[cell.Name.ToString()] = cell.Value.ToString();
                                }
                                catch (Exception) { } //To handle the wrong cells
                            }

                            // Add the newly created row in the table
                            entitySetDetails.DetailsTable.Rows.Add(row);
                        }
                        catch (Exception)
                        {
                            // To handle the wrong rows
                        }
                    }
                }
            }

            // Return entitySetDetails
            return entitySetDetails;
        }


        /// <summary>
        /// This method gives the meta data for the given container & entitySet
        /// </summary>
        /// <param name="container">Container: Alias</param>
        /// <param name="tableName">EntitySetName</param>
        /// <returns>returns an object of EntitySetDetails</returns>
        internal static EntitySetDetails GetMetaData(string container,
            string tableName)
        {
            // Declare object of class EntitySetDetails
            EntitySetDetails entitySetDetails = null;

            // Validatie the parameters
            if ((!String.IsNullOrEmpty(container)) &&
                !(String.IsNullOrEmpty(tableName)))
            {

                // Create an instance of class Storage
                IsdkStorageProviderInterface storage = Helper.ServiceObject;

                // Define entitySetDetails
                entitySetDetails = new EntitySetDetails();

                // Set the properties of entitySetDetails object
                entitySetDetails.ContainerAlias = container;
                entitySetDetails.EntitySetName = tableName;

                // Set the filter
                string tableNameFilter = "entityset eq '" + tableName + "'";

                //sl-king
                //TODO make this a reusable code (find all sfdgsdfgsdfgd)
                EntitySet es = EntitySetRepository.GetEntitySet(container, tableName);
                //since this is OData address looks like this .../service/category/entityset
                //metadata is at address .../service/category/$metadata
                string metadataUri = es.DataSvcLink;

                //first get rid of ?option1=...&option2=....
                var uriParts = metadataUri.Split('?');
                metadataUri = uriParts[0];
                if (metadataUri[metadataUri.Length - 1] != '/')//if it's not / terminated, let's terminate it
                    metadataUri = metadataUri + "/";

                uriParts = metadataUri.Split('/');
                metadataUri = uriParts[0];
                for (int i = 1; i < uriParts.Length - 2; i++)
                {
                    metadataUri += "/" + uriParts[i];
                }
                metadataUri += "/$metadata";

                // Fetches the data from Azure Table Storage
                XElement metaDataXML = storage.GetMetadata(metadataUri, container, tableName);//, tableNameFilter);

                //sl-king: why is this done twice??? see GetBrowserData
                // Remove the unnecessary columns
                var properties = metaDataXML.Element("EntityType");//Elements("properties");
                properties.Elements("Key").Remove();
                //properties.Elements("entityset").Remove();
                //properties.Elements("entitykind").Remove();

                // Set the column list
                var propertyMetaData = properties.Elements();// metaDataXML.Elements("properties").First().Elements();

                // Add the column names in the detailsTable of the object entitySetDetails
                foreach (var property in propertyMetaData)
                {
                    //sl-king: we tryed to get rid of these in GetData, but Azure XML is different from OGDI's
                    if (property.Attribute("Name").Value.ToLower() == "partitionkey" ||
                            property.Attribute("Name").Value.ToLower() == "rowkey" ||
                            property.Attribute("Name").Value.ToLower() == "timestamp")
                        continue;

                    string sName = property.Attribute("Name").Value;
                    if (sName == "entityid")//.Name == "entityid")
                    {
                        entitySetDetails.DetailsTable.Columns.Add(property.Name.ToString(),
                            Type.GetType("System.Guid"));
                    }
                    else
                    {
                        string sType = property.Attribute("Type").Value;
                        sType = sType.Replace("Edm.", "System.");

                        entitySetDetails.DetailsTable.Columns.Add(sName, Type.GetType(sType));//property.Name.ToString(), property.Value));
                    }
                }
            }

            return entitySetDetails;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Globalization;
using System.Collections.Specialized;
using Odp.UserInterface.Models;

namespace Odp.UserInterface
{
    /// <summary>
    /// Cache need to be implemented
    /// </summary>
    public class Cache
    {
        static public IEnumerable<EntitySet> EntitySets(String container)
        {
            if (HttpContext.Current == null)
                return GetEntitySets(container);

            IDictionary<string, IEnumerable<EntitySet>> entitySets;

            if (HttpContext.Current.Session["EntitySetCache"] == null)
            {
                HttpContext.Current.Session["EntitySetCache"] =
                entitySets = new Dictionary<string, IEnumerable<EntitySet>>();
            }
            else
            {
                entitySets = (IDictionary<string, IEnumerable<EntitySet>>)HttpContext.Current.Session["EntitySetCache"];
            }

            if (!entitySets.ContainsKey(container))
            {
                return entitySets[container] = GetEntitySets(container);
            }

            return entitySets[container];
        }

        private static EntitySet CreateEntitySet(XElement element, string containerAlias)
        {
            DateTime updateDate;
            DateTime.TryParse((element.Element("lastupdatedate") ?? new XElement("Dumb")).Value, out updateDate);
            DateTime releaseDate;
            if (!DateTime.TryParse((element.Element("releaseddate") ?? new XElement("Dumb")).Value, out releaseDate))
                releaseDate = updateDate;
            DateTime expiredDate;
            DateTime.TryParse((element.Element("expireddate") ?? new XElement("Dumb")).Value, out expiredDate);
            List<OrderedDictionary> downloadlinks = new List<OrderedDictionary>();

            foreach (XElement link in element.Element("downloadlinks").Elements())
            {
                OrderedDictionary dod = new OrderedDictionary();

                dod.Add("Name", link.Element("downloadlinkname").Value.ToString());
                dod.Add("Type", link.Element("downloadlinktype").Value.ToString());
                dod.Add("Link", link.Element("downloadlinkurl").Value.ToString());
                dod.Add("IconLink", link.Element("downloadlinkiconurl").Value.ToString());
                dod.Add("Description", link.Element("downloadlinkdescription").Value.ToString());
                dod.Add("DownloadCount", link.Element("downloadcount").Value.ToString());
                dod.Add("ID", link.Element("downloadid").Value.ToString());

                downloadlinks.Add(dod);
            }

            return new EntitySet(
                new Guid(element.Element("entityid").Value),
                element.Element("datasetname") != null ? element.Element("datasetname").Value : null,
                //element.Element("description") != null ? element.Element("description").Value : null,
                element.Element("entitykind") != null ? element.Element("entitykind").Value : null,
                element.Element("category") != null ? element.Element("category").Value : null,
                element.Element("description") != null ? element.Element("description").Value : null,
                element.Element("datasource") != null ? element.Element("datasource").Value : null,
                element.Element("datasourcedescription") != null ? element.Element("datasourcedescription").Value : null,
                element.Element("metadataurl") != null ? element.Element("metadataurl").Value : null,
                element.Element("entityset") != null ? element.Element("entityset").Value : null,
                element.Element("downloadlink") != null ? element.Element("downloadlink").Value : null,
                //element.Element("longlatcolumns") != null ? element.Element("longlatcolumns").Value : null,
                //element.Element("kmlcolumn") != null ? element.Element("kmlcolumn").Value : null,
                element.Element("datasvclink") != null ? element.Element("datasvclink").Value : null,
                element.Element("datasvckmllink") != null ? element.Element("datasvckmllink").Value : null,
                element.Element("datasvckmltype") != null ? element.Element("datasvckmltype").Value : null,
                containerAlias,
                element.Element("datasetimage") != null ? element.Element("datasetimage").Value : null,
                downloadlinks
                )
                {
                    LastUpdateDate = updateDate,
                    ReleasedDate = releaseDate,
                    ExpiredDate = expiredDate,
                    UpdateFrequency =
                        element.Element("updatefrequency") != null ? element.Element("updatefrequency").Value : null,
                    Keywords = element.Element("keywords") != null ? element.Element("keywords").Value : null,
                    Links = element.Element("links") != null ? element.Element("links").Value : null,
                    PeriodCovered = element.Element("periodcovered") != null ? element.Element("periodcovered").Value : null,
                    GeographicCoverage =
                        element.Element("geographiccoverage") != null ? element.Element("geographiccoverage").Value : null,
                    AdditionalInformation =
                        element.Element("additionalinfo") != null ? element.Element("additionalinfo").Value : null,
                    IsEmpty = element.Element("isempty") != null && element.Element("isempty").Value.Length == 4,
                    CollectionMode = element.Element("collectionmode") != null ? element.Element("collectionmode").Value : null,
                    CollectionInstruments = element.Element("collectioninstruments") != null ? element.Element("collectioninstruments").Value : null,
                    DataDictionaryVariables = element.Element("datadictionary_variables") != null ? element.Element("datadictionary_variables").Value : null,
                    TechnicalInfo = element.Element("technicalinfo") != null ? element.Element("technicalinfo").Value : null

                };
        }

        private static IEnumerable<EntitySet> GetEntitySets(string containerAlias)
        {
            var data = Helper.ServiceObject.GetData(null, "Metadata", containerAlias, null, 0/*Convert.ToInt32(Resources.EntitySetPageSize, CultureInfo.InvariantCulture)*/, null, null, 0);
            var list = new List<EntitySet>();
            var els = data.Elements("collection");//would be EntityType for $metadata

            foreach (var el in els)
                list.Add(CreateEntitySet(el, containerAlias));
            return list;
        }
    }
}

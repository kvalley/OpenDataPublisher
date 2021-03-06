﻿/* 
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
using System.Collections.Specialized;
using System.Collections.Generic;
using Odp.UserInterface;

namespace Odp.UserInterface.Models
{
    [Serializable]
    public class EntitySet : IComparable
    {
        #region Properties

    	/// <summary>
    	/// This property represents EntityId of the EntitySet.
    	/// This property is uniques for each EntitySet
    	/// </summary>
    	public Guid EntityId { get; set; }

    	/// <summary>
    	/// This property represents Name of the EntitySet.
    	/// </summary>
    	public string Name { get; set; }

    	/// <summary>
    	/// This property represents CategoryValue of the EntitySet.
    	/// </summary>
    	public string CategoryValue { get; set; }

    	/// <summary>
    	/// This property represents Description of the EntitySet.
    	/// </summary>
    	public string Description { get; set; }

        /// <summary>
        /// This property represents Image of the EntitySet.
        /// </summary>
        public string Image { get; set; } // nanaimo:rc

    	/// <summary>
    	/// This property represents Source of the EntitySet.
    	/// </summary>
    	public string Source { get; set; }

    	/// <summary>
    	/// This property represents description of the Source of the EntitySet.
		/// intended for presentation purposes only
    	/// </summary>
		public string SourceDescription { get; set; }

    	/// <summary>
    	/// This property represents LastUpdateDate of the EntitySet.
    	/// </summary>
    	public DateTime LastUpdateDate { get; set; }

    	/// <summary>
    	/// This property represents MetadataUrl of the EntitySet.
    	/// </summary>
    	public string MetadataUrl { get; set; }

    	/// <summary>
    	/// This property represents EntitySetName of the EntitySet.
    	/// </summary>
    	public string EntitySetName { get; set; }

    	/// <summary>
    	/// This property represents EntityKind of the EntitySet.
    	/// </summary>
    	public string EntityKind { get; set; }

    	/// <summary>
    	/// This property represents Parent of the EntitySet.
    	/// </summary>
    	public string ContainerAlias { get; set; }

    	public DateTime ReleasedDate { get; set; }
        public string UpdateFrequency { get; set; }
        public string Keywords { get; set; }
        public string Links { get; set; }
        public string PeriodCovered { get; set; }
        public string GeographicCoverage { get; set; }
        public string AdditionalInformation { get; set; }

        public string CollectionMode { get; set; }
        public string CollectionInstruments { get; set; }
        public string DataDictionaryVariables { get; set; }
        public string TechnicalInfo { get; set; }
        public DateTime ExpiredDate { get; set; }

    	public bool IsDownloadOnly
		{
			get
			{
				//return string.IsNullOrEmpty(DataSvcLink) && !string.IsNullOrEmpty(DownloadLink);
                return string.IsNullOrEmpty(DataSvcLink);// && (DownloadLinks.Count > 0);
			}
		}
			
		/// <summary>
    	/// This property represents an external link to download-only EntitySet.
        /// Note: Old individual download link, no longer used
    	/// </summary>
		public string DownloadLink {get;set;}

        /// <summary>
        /// This property represents a set of external download links
        /// </summary>
        public List<OrderedDictionary> DownloadLinks;

    	/// <summary>
    	/// This property represents source fileds for point KML ('longitude_field,latitude_field')
    	/// </summary>
		public string LongLatColumns {get;set;}

    	/// <summary>
    	/// This property represents a source field with prepared KML snippet
    	/// </summary>
		public string KmlColumn {get;set;}

    	/// <summary>
    	/// This property represents a direct link to OData feed for this data set
    	/// </summary>
		public string DataSvcLink {get;set;}

		/// <summary>
    	/// This property represents a direct link to filtered KML feed for this data set
    	/// </summary>
		public string DataSvcKmlLink {get;set;}

            

		/// <summary>
    	/// This property describes a type of KML provider
		/// KML queries are not standardized as are OData so we need to know in advance and support known providers
		/// 1. ogdi = OGDI
		/// 2. georest = GeoREST
    	/// </summary>
		public string DataSvcKmlType {get;set;}

		public static string GetEntityDateAsString(DateTime date)
        {
            return (date != DateTime.MinValue) ? date.ToString("D") : string.Empty;
        }

		/// <summary>
		/// Gets or sets a value indicating weather data set is uploaded or not.
		/// </summary>
		public bool IsEmpty { get; set; }

        public String ItemKey
        {
            get
            {
                return Helper.GenerateDatasetItemKey(ContainerAlias, EntitySetName);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public EntitySet()
        {
            //EntityId = Guid.Empty;
            Name = string.Empty;
            CategoryValue = string.Empty;
            Description = string.Empty;
            Source = string.Empty;
            //LastUpdateDate = DateTime.MinValue;
            MetadataUrl = string.Empty;
            EntitySetName = string.Empty;
            //EntityKind = string.Empty;
            ContainerAlias = string.Empty;
            Image = string.Empty;
            DownloadLinks = new List<OrderedDictionary>();
        }

        /// <summary>
        /// Parameterized Constructor.
        /// </summary>
        /// <param name="entityId">entityId of the EntitySet</param>
        /// <param name="name">name of the EntitySet</param>
        /// <param name="categoryValue">categoryValue of the EntitySet</param>
        /// <param name="description">description of the EntitySet</param>
        /// <param name="source">source of the EntitySet</param>
        /// <param name="metadataUri">metadataUri of the EntitySet</param>
        /// <param name="entitySetName">entitySetName of the EntitySet</param>
        /// <param name="entityKind">entityKind of the EntitySet</param>
        /// <param name="containerAlias">containerAlias of the EntitySet</param>        
        public EntitySet(Guid entityId, string name, string entityKind,
				string categoryValue, string description, string source, string sourceDescription,
            string  metadataUri, string entitySetName, string downloadlink,
						//string longlatCols, string kmlCol,
						string dataSvcLink, string dataSvcKmlLink, string dataSvcKmlType,
            string containerAlias, string image, List<OrderedDictionary> downloadlinks)
        {
            EntityId = entityId;
            Name = name;
            Image = image; // nanaimo:rc
            CategoryValue = categoryValue;
            Description = description;
            Source = source;
            SourceDescription = sourceDescription;
            MetadataUrl = metadataUri;
            EntitySetName = entitySetName;
            EntityKind = entityKind;

			DownloadLink = downloadlink;
			//LongLatColumns = longlatCols;
			//KmlColumn = kmlCol;
			DataSvcLink = dataSvcLink;
			DataSvcKmlLink = dataSvcKmlLink;
			DataSvcKmlType = dataSvcKmlType;

            ContainerAlias = containerAlias;

            DownloadLinks = downloadlinks;
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// This method is overridden for Sort() method
        /// </summary>
        /// <param name="obj">instance of object class</param>
        /// <returns>integer result of comparison</returns>
        public int CompareTo(object obj)
        {
            EntitySet c = obj as EntitySet;

            //return Name.CompareTo(c.Name);
            //Fxcop: 
            return string.Compare(Name, c.Name, 
                StringComparison.Ordinal);
        }

        /// <summary>
        /// This method overrides == operator
        /// </summary>
        /// <param name="es1">first EntitySey for
        /// comparison</param>
        /// <param name="es2">second EntitySey for 
        /// comparison</param>
        /// <returns>returns true if both passed 
        /// containers are same else false</returns>
        public static bool operator ==(EntitySet es1, EntitySet es2)
        {
            // returns true if both passed containers 
            // are same else false
            if ((object)es1 == null)
                return (object)es2 == null;
            return es1.Equals(es2);
        }

        /// <summary>
        /// This method overrides != operator
        /// </summary>
        /// <param name="es1">first EntitySey for
        /// comparison</param>
        /// <param name="es2">second EntitySey for 
        /// comparison</param>
        /// <returns>returns true if first container 
        /// != second container else false</returns>
        public static bool operator !=(EntitySet es1, EntitySet es2)
        {
            // returns true if first container != second 
            // container else false
            return !(es1 == es2);
        }

        /// <summary>
        /// This method overrides less than operator
        /// </summary>
        /// <param name="es1">first EntitySey for 
        /// comparison</param>
        /// <param name="es2">second EntitySey for 
        /// comparison</param>
        /// <returns>returns true if first container 
        /// is less than second container else false</returns>
        public static bool operator <(EntitySet es1, EntitySet es2)
        {
            // returns true if first container is less than 
            // second container else false
            return (es1.CompareTo(es2) < 0);
        }

        /// <summary>
        /// This method overrides greater than operator
        /// </summary>
        /// <param name="es1">first EntitySey for
        /// comparison</param>
        /// <param name="es2">second EntitySey for 
        /// comparison</param>
        /// <returns>returns true if first container is 
        /// greter than second container else false</returns>
        public static bool operator >(EntitySet es1, EntitySet es2)
        {
            // returns true if first container is greter than
            // second container else false
            return (es1.CompareTo(es2) > 0);
        }  
        #endregion
    }
}
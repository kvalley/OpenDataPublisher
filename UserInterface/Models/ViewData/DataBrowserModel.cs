using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;

namespace Odp.UserInterface.Models
{
    public class DataBrowserModel
    {
        private char isBookmarked;

        public char IsBookmarked
        {
            get { return isBookmarked; }
            set { isBookmarked = value; }
        }

        private short visibleView;

        public short VisibleView
        {
            get { return visibleView; }
            set { visibleView = value; }
        }

        private string sampleCodeDataView;

        public string SampleCodeDataView
        {
            get { return sampleCodeDataView; }
            set { sampleCodeDataView = value; }
        }

        private string filterText;

        public string FilterText
        {
            get { return filterText; }
            set { filterText = value; }
        }

        private string sampleCodeMapView;

        public string SampleCodeMapView
        {
            get { return sampleCodeMapView; }
            set { sampleCodeMapView = value; }
        }

        private string sampleCodeBarChartView;

        public string SampleCodeBarChartView
        {
            get { return sampleCodeBarChartView; }
            set { sampleCodeBarChartView = value; }
        }

        private string sampleCodePieChartView;

        public string SampleCodePieChartView
        {
            get { return sampleCodePieChartView; }
            set { sampleCodePieChartView = value; }
        }

        private short visibleTag;

        public short VisibleTag
        {
            get { return visibleTag; }
            set { visibleTag = value; }
        }

        private int zoomLevel;

        public int ZoomLevel
        {
            get { return zoomLevel; }
            set { zoomLevel = value; }
        }

        private string container;

        public string Container
        {
            get { return container; }
            set { container = value; }
        }

        private string entitySetName;

        public string EntitySetName
        {
            get { return entitySetName; }
            set { entitySetName = value; }
        }

		public EntitySetDetails EntitySetDetails { get; set; }

        public EntitySetWrapper EntitySetWrapper { get; set; }

        private string dBErrorLine1;

        public string DBErrorLine1
        {
            get { return dBErrorLine1; }
            set { dBErrorLine1 = value; }
        }

        private string dBErrorLine2;

        public string DBErrorLine2
        {
            get { return dBErrorLine2; }
            set { dBErrorLine2 = value; }
        }

        private string baseQueryName;

        public string BaseQueryName
        {
            get { return baseQueryName; }
            set { baseQueryName = value; }
        }

        //sl-king
				//interactive SDK is now only an OData client
			  //kml is not OData, so it can't be filtered as such (OGDI -> /vi/.../?$filter=..&format=kml)
				//only by looking at metadata we know what kind of filter to use
				private string kmlQueryName;

        public string KmlQueryName
        {
            get { return kmlQueryName; }
            set { kmlQueryName = value; }
        }
        
        private string filteredQueryName;

        public string FilteredQueryName
        {
            get { return filteredQueryName; }
            set { filteredQueryName = value; }
        }

        private bool nextEnable;

        public bool NextEnable
        {
            get { return nextEnable; }
            set { nextEnable = value; }
        }
        
        private bool prevEnable;

        public bool PrevEnable
        {
            get { return prevEnable; }
            set { prevEnable = value; }
        }
            
        private DataTable tableBrowserData;

        public DataTable TableBrowserData
        {
            get { return tableBrowserData; }
            set { tableBrowserData = value; }
        }

        private SelectList dataViewLanguages;

        public SelectList DataViewLanguages
        {
            get { return dataViewLanguages; }
            set { dataViewLanguages = value; }
        }

        private double longitude;

        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        private double latitude;

        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        private string mapStyle;

        public string MapStyle
        {
            get { return mapStyle; }
            set { mapStyle = value; }
        }

        private int mapMode;

        public int MapMode
        {
            get { return mapMode; }
            set { mapMode = value; }
        }

        private long sceneId;

        public long SceneId
        {
            get { return sceneId; }
            set { sceneId = value; }
        }

        private string birdseyeSceneOrientation;

        public string BirdseyeSceneOrientation
        {
            get { return birdseyeSceneOrientation; }
            set { birdseyeSceneOrientation = value; }
        }

        private SelectList mapViewLanguages;

        public SelectList MapViewLanguages
        {
            get { return mapViewLanguages; }
            set { mapViewLanguages = value; }
        }

        private Dictionary<string, double> chart;

        public Dictionary<string, double> Chart
        {
            get { return chart; }
            set { chart = value; }
        }

        private SelectList barHorizontal;

        public SelectList BarHorizontal
        {
            get { return barHorizontal; }
            set { barHorizontal = value; }
        }

        private SelectList barVertical;

        public SelectList BarVertical
        {
            get { return barVertical; }
            set { barVertical = value; }
        }

        private SelectList barDateRange;

        public SelectList BarDateRange
        {
            get { return barDateRange; }
            set { barDateRange = value; }
        }

        private string barYOption;

        public string BarYOption
        {
            get { return barYOption; }
            set { barYOption = value; }
        }

        private string barYColOption;

        public string BarYColOption
        {
            get { return barYColOption; }
            set { barYColOption = value; }
        }

        private bool isSelectOne;

        public bool IsSelectOne
        {
            get { return isSelectOne; }
            set { isSelectOne = value; }
        }
        
        private SelectList barChartViewLanguages;

        public SelectList BarChartViewLanguages
        {
            get { return barChartViewLanguages; }
            set { barChartViewLanguages = value; }
        }
        
        private SelectList pieHorizontal;

        public SelectList PieHorizontal
        {
            get { return pieHorizontal; }
            set { pieHorizontal = value; }
        }
                
        private SelectList pieVertical;

        public SelectList PieVertical
        {
            get { return pieVertical; }
            set { pieVertical = value; }
        }
        
        private SelectList pieDateRange;

        public SelectList PieDateRange
        {
            get { return pieDateRange; }
            set { pieDateRange = value; }
        }
        
        private string pieYOption;

        public string PieYOption
        {
            get { return pieYOption; }
            set { pieYOption = value; }
        }

        private string pieYColOption;

        public string PieYColOption
        {
            get { return pieYColOption; }
            set { pieYColOption = value; }
        }
        
        private SelectList pieChartViewLanguages;

        public SelectList PieChartViewLanguages
        {
            get { return pieChartViewLanguages; }
            set { pieChartViewLanguages = value; }
        }

        private string xColName;

        public string XColName
        {
            get { return xColName; }
            set { xColName = value; }
        }
        
        private string yColName;

        public string YColName
        {
            get { return yColName; }
            set { yColName = value; }
        }
        
        private int xCount;

        public int XCount
        {
            get { return xCount; }
            set { xCount = value; }
        }

    	public bool IsPlanned
    	{
			get { return EntitySetWrapper.EntitySet.IsEmpty; }
    	}
    }
}

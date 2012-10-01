<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Odp.InteractiveSdk.Mvc.Models.DataBrowserModel>" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, 
    Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%  
    
    //  Create a new instance of the Chart class at run time
    //  However, for simplicity, it is recommended that 
    //  you create a Chart instance at design time.
    //  This is the root object of the Chart control.
    System.Web.UI.DataVisualization.Charting.Chart OgdiBarChart =
        new System.Web.UI.DataVisualization.Charting.Chart();

    //  Number of elements on X-axis
    int calculatedWidth = Convert.ToInt32(ViewData.Model.XCount) * 15;

    //  If calculated width is greater than 860 then ...
    if (calculatedWidth > 860)
    {
        //  Set Bar Chart width to Calculated width
        OgdiBarChart.Width = calculatedWidth;
        
    }
    else
    {
        //  Set Bar Chart width to 860
        OgdiBarChart.Width = 860;
        
    }

    //  Set Bar Chart height to 500
    OgdiBarChart.Height = 500;

    //  Set Unique ID for Bar Chart
    OgdiBarChart.ID = "imgBarChart";

    //  Specify how an image of the chart will be rendered. 
    //  BinaryStreaming --> Chart is streamed directly to the client. 
    //  ImageMap --> Chart is rendered as an image map. 
    //  ImageTag --> Chart is rendered using an HTML image tag. 
    OgdiBarChart.RenderType = RenderType.ImageTag;

    //  Set the chart rendering type.
    //  This property defines the type of storage used to render chart images.
    OgdiBarChart.ImageStorageMode = ImageStorageMode.UseHttpHandler;

    //  Set the palette for the Chart control. 
    //  Berry --> utilizes blues and purples. 
    //  Bright  --> utilizes bright colors. 
    //  BrightPastel  --> utilizes bright pastel colors. 
    //  Chocolate  --> utilizes shades of brown. 
    //  EarthTones  --> utilizes earth tone colors such as
    //                  green and brown. 
    //  Excel  --> utilizes Excel-style colors. 
    //  Fire  --> utilizes red, orange and yellow colors. 
    //  Grayscale  --> utilizes grayscale colors, that is, 
    //              shades of black and white. 
    //  Light --> utilizes light colors. 
    //  None --> No palette is used.  
    //  Pastel --> utilizes pastel colors. 
    //  SeaGreen --> utilizes colors that range from green to blue. 
    //  SemiTransparent --> utilizes semi-transparent colors. 
    OgdiBarChart.Palette = ChartColorPalette.BrightPastel;

    //  Add a Title object to the end of the title collection. 
    OgdiBarChart.Titles.Add(new Title(ViewData.Model.EntitySetName,
        Docking.Top, new System.Drawing.Font("Calibri", 14, 
            System.Drawing.FontStyle.Bold),
        System.Drawing.Color.FromArgb(26, 59, 105)));

    //  Create a ChartArea class object which represents a 
    // chart area on the chart image. 
    ChartArea chartArea = new ChartArea("ColumnChartArea");

    //  Create a Series class object which stores data points 
    // and series attributes. 
    Series series = new Series("Series 1");

    //  Set an Axis object that represents the primary X-axis.
    chartArea.AxisX.Title = ViewData.Model.XColName;
    chartArea.AxisX.TitleFont = new System.Drawing.Font("Calibri", 12, 
        System.Drawing.FontStyle.Bold);
    chartArea.AxisX.IsLabelAutoFit = true;
    chartArea.AxisX.IntervalType =  DateTimeIntervalType.Auto;
    chartArea.AxisX.LabelStyle.Font = 
        new System.Drawing.Font("Calibri", 9);
    chartArea.AxisX.Interval = 1;

    //  Set an Axis object that represents the primary Y-axis.
    chartArea.AxisY.Title = ViewData.Model.YColName;
    chartArea.AxisY.TitleFont =
        new System.Drawing.Font("Calibri", 12, 
        System.Drawing.FontStyle.Bold);
    chartArea.AxisY.IsLabelAutoFit = true;
    chartArea.AxisY.LabelStyle.Font = 
        new System.Drawing.Font("Calibri", 9);

    //  This "Position" property defines the position of a ChartArea 
    // object within the Chart.      
    chartArea.Position.Width = 98;  //  this value is in percent 
    chartArea.Position.Height = 90;   //  this value is in percent
    chartArea.Position.Y = 8;   //  this value is in percent
    
    //  Add a ChartArea object to the  ChartAreas collection. 
    OgdiBarChart.ChartAreas.Add(chartArea);

    //  Set the chart type of a series. 
    series.ChartType = SeriesChartType.Column;

    //  Set the value type plotted along the X-axis to String
    series.XValueType = ChartValueType.String;

    //  Set the value type plotted along the Y-axis to Double
    series.YValueType = ChartValueType.Double;

    //  Set the font of the data point. 
    series.Font = new System.Drawing.Font("Calibri", 8,
        System.Drawing.FontStyle.Bold);

    //  Set a flag that indicates whether to show the value of 
    //  the data point on the label.
    series.IsValueShownAsLabel = true;

    series.Label = "#VALY{#,0}";

    //  Set the tooltip
    series.ToolTip = "'#VALX' with value '#VALY{#,0}'";

    //  Add a Series object to the  Series collection. 
    OgdiBarChart.Series.Add(series);

    //  If there is data for representing chart in Viewdata then ...
    if (ViewData.Model.Chart != null)
    {
        //  For each XY value pair do ...   
        foreach (KeyValuePair<string, double> XyPair in 
            ViewData.Model.Chart )
        {
            //  Add a DataPoint object to the end of the collection,
            //  with the specified X-value and Y-value. 
            OgdiBarChart.Series[0].Points.AddXY(

                //  If key is empty then put xValue = <<BLANK>>...
                XyPair.Key.Trim().Equals(string.Empty) ? "<<BLANK>>" :

                //  else if Key length is greater than 20 then xValue becomes 
                //  first 18 characters appeneded with two dots
                //  else if Key length is less than or equal to 20 then 
                //  xValue will be considered as entire value of key
                XyPair.Key.Length > 20 ? 
                    XyPair.Key.Substring(0, 18) + ".." : XyPair.Key,
                //  yValue is assigned to value in Xypair
                    XyPair.Value);
        }
    }

    //  Set a BorderSkin object, which provides border skin
    // functionality for the Chart control.
    OgdiBarChart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

    //  Set BorderColor
    OgdiBarChart.BorderColor = System.Drawing.Color.FromArgb(26, 59, 105);

    //  Set the style of the entire chart image border line.
    OgdiBarChart.BorderlineDashStyle = ChartDashStyle.Solid;

    //  Set BorderWidth
    OgdiBarChart.BorderWidth = 2;  

    //  Set Page for chart control
    OgdiBarChart.Page = this.Parent.Page as System.Web.UI.Page;

    //  Set the render control with an object that receives
    // the control content
    OgdiBarChart.RenderControl(new HtmlTextWriter(this.Response.Output));	
    				
%>

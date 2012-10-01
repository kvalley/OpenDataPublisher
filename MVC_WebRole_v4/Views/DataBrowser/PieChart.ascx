<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Odp.InteractiveSdk.Mvc.Models.DataBrowserModel>" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, 
Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting"
    TagPrefix="asp" %>
<%
    System.Web.UI.DataVisualization.Charting.Chart OgdiPieChart =
        new System.Web.UI.DataVisualization.Charting.Chart();
    int calculatedHeight = 650;
    int count = Convert.ToInt32(ViewData.Model.XCount);
    if (count >= 0 && count <= 30)
    {
        OgdiPieChart.Height = 650;
    }
    else if (count > 30 && count <= 60)
    {
        calculatedHeight = Convert.ToInt32(ViewData.Model.XCount) * 16;
        OgdiPieChart.Height = calculatedHeight;
    }
    else if (count > 60 && count <= 90)
    {
        calculatedHeight = Convert.ToInt32(ViewData.Model.XCount) * 8;
        OgdiPieChart.Height = calculatedHeight;
    }
    else if (count > 90)
    {
        calculatedHeight = Convert.ToInt32(ViewData.Model.XCount) * 7;
        OgdiPieChart.Height = calculatedHeight;
    }

    calculatedHeight += 150;
    OgdiPieChart.Width = 880;    
%>

<script type="text/javascript">
    var h = '<%= calculatedHeight %>';
    document.getElementById("divPieResultHeight").style.height = parseInt(h) + "px";  
</script>

<%
    //  Set Unique ID for Pie Chart
    OgdiPieChart.ID = "imgPieChart";

    //  Specify how an image of the chart will be rendered. 
    //  BinaryStreaming --> Chart is streamed directly to the client. 
    //  ImageMap --> Chart is rendered as an image map. 
    //  ImageTag --> Chart is rendered using an HTML image tag. 
    OgdiPieChart.RenderType = RenderType.ImageTag;

    //  Set the chart rendering type.
    //  This property defines the type of storage used to render chart images.
    OgdiPieChart.ImageStorageMode = ImageStorageMode.UseHttpHandler;

    Title t = new Title(ViewData.Model.EntitySetName,
        Docking.Top, new System.Drawing.Font("Calibri", 14,
            System.Drawing.FontStyle.Bold),
            System.Drawing.Color.FromArgb(26, 59, 105));

    //  Add a Title object to the end of the title collection. 
    OgdiPieChart.Titles.Add(t);

    //  Create a ChartArea class object which represents a 
    //  chart area on the chart image. 
    ChartArea chartArea = new ChartArea("ColumnChartArea");

    //  Create a Series class object which stores data points 
    //  and series attributes. 
    Series series = new Series("Series 1");

    //  Add a ChartArea object to the  ChartAreas collection. 
    OgdiPieChart.ChartAreas.Add(chartArea);

    //  Set the chart type of a series. 
    series.ChartType = SeriesChartType.Pie;

    //  Set the value type plotted along the Y-axis to Double
    series.YValueType = ChartValueType.Double;

    //  Set the tooltip             
    series.ToolTip = "'#VALX' : '#VALY'";

    series.Label = "#PERCENT{P2}";
    series.LabelToolTip = "'#VALX' : '#VALY'";
    series.LegendText = "'#VALX' : #PERCENT{P2}";
    series.LegendToolTip = "'#VALX' : '#VALY'";
    series["PieLabelStyle"] = "Disabled";

    //  Add a Series object to the  Series collection. 
    OgdiPieChart.Series.Add(series);

    if (ViewData.Model.Chart != null)
    {
        foreach (KeyValuePair<string, double> XyPair in ViewData.Model.Chart
            as Dictionary<string, double>)
        {
            OgdiPieChart.Series[0].Points.AddXY(XyPair.Key.Trim().
                Equals(string.Empty) ? "<<BLANK>>" :
                XyPair.Key.Length > 30 ? XyPair.Key.Substring(0, 28) + ".."
                : XyPair.Key, XyPair.Value);
        }
    }

    //  Set a BorderSkin object, which provides border skin
    // functionality for the Chart control.
    OgdiPieChart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

    //  Set BorderColor
    OgdiPieChart.BorderColor = System.Drawing.Color.FromArgb(26, 59, 105);

    //  Set the style of the entire chart image border line.
    OgdiPieChart.BorderlineDashStyle = ChartDashStyle.Solid;

    //  Set BorderWidth
    OgdiPieChart.BorderWidth = 2;

    OgdiPieChart.Legends.Add("Legend1");
    OgdiPieChart.Legends[0].Docking = Docking.Bottom;
    OgdiPieChart.Legends[0].Alignment = System.Drawing.StringAlignment.Center;
    OgdiPieChart.Legends[0].LegendStyle = LegendStyle.Table;
    OgdiPieChart.Legends[0].IsTextAutoFit = false;
    OgdiPieChart.Legends[0].TableStyle = LegendTableStyle.Wide;
    OgdiPieChart.Legends[0].TextWrapThreshold = 80;

    //  Set Page for chart control
    OgdiPieChart.Page = this.Parent.Page as System.Web.UI.Page;

    HtmlTextWriter writer = new HtmlTextWriter(this.Response.Output);

    //  Set the render control with an object that receives
    //  the control content
    OgdiPieChart.RenderControl(writer);					
%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ODP_MasterOriginal.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Odp.InteractiveSdk.Mvc.Models"%>
<%@ Import Namespace="System.Web.Mvc"%>
<%@ Import Namespace="Odp.InteractiveSdk.Mvc.App_GlobalResources"%>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript" language="javascript" src="<%= Url.Content("../../Scripts/DataBrowser.min.js") %>"></script>
    <script type="text/javascript" language="javascript" src="<%= Url.Content("../../Scripts/ogdi/list.js") %>"></script>
          
    <script type="text/javascript">
        var fieldNames = ["Image", "Name", "Description", "Category", "Status", "Date", "Rating", "Views"];
        setListParameters("/DataCatalog/DataSets", "/DataCatalog/GetListDataJSON", fieldNames);

        $(document).ready(function() {
            $("#SubmitFilter").click(function() {
                filter = submitFilter();
                setPage(1);
                updateListData();
            });
            $("#ClearFilter").click(function() {
                clearFilter();
                filter = submitFilter();
                setPage(1);
                updateListData();
            });
            $("#SubmitFilterSimple").click(function() {
                filter = submitFilterSimple();
                setPage(1);
                updateListData();
            });
            $("#ClearFilterSimple").click(function() {
                clearFilterSimple();
                filter = submitFilterSimple();
                setPage(1);
                updateListData();
            });
        });

    </script>

</asp:Content>
<asp:Content ID="mainContent" ContentPlaceHolderID="MainContent" runat="server">
    
<%--    <div class="top5">
		<table cellpadding="0" cellspacing="0" border="0"><tr>
		<td style="border-width:0px;"><% Html.RenderPartial("TopList", Model, new ViewDataDictionary(ViewData){{"TopOf", Field.Date}}); %></td>
		<td><% Html.RenderPartial("TopList", Model, new ViewDataDictionary(ViewData){{"TopOf", Field.Rating}}); %></td>
		<td><% Html.RenderPartial("TopList", Model, new ViewDataDictionary(ViewData){{"TopOf", Field.Views}}); %></td>
		</tr></table>
    </div>--%>

	<div class="block">
		<% Html.RenderPartial("DataSetsFilter", Model); %>
    </div>
    <% var sortImageSrc = ResolveUrl("~/Images/t.gif"); %>
	<div class="dataset-list block">
		<table cellpadding="0" cellspacing="0" border="0">
		<thead><tr>
            <td id="Image" style="width: 170px;">Image</td>   
            <td id="Name" style="width: 450px;">Name<img src="<%= sortImageSrc %>" /></td>
            <td id="ColumnAPI" style="width: 40px;text-align:center;">API</td>
            <td id="DownloadHTML" style="width:40px;text-align: center;">SHP</td>
            <td id="DownloadKML" style="width:40px;text-align: center;">KML</td>
            <td id="DownloadRSS" style="width:40px;text-align: center;">RSS</td>
            
            <td></td>
			<%--
			  <td id="Category" width="12.5%" class="ascna">Category<img src="<%= sortImageSrc %>" /></td>
			<td id="Status" width="12.5%" class="ascna">Status<img src="<%= sortImageSrc %>" /></td>
			<td id="Date" width="12.5%" class="descna">Date<img src="<%= sortImageSrc %>" /></td> --%>
		</tr></thead>
		<tbody class="rows"></tbody>
		<tfoot>
		<tr>
			<td colspan="2" >
			    <%if(false){ %>
				Have an idea about the data you would like to see here? Request it <a href="/Request/Index/">here</a>
				<%} %>
			</td>
			<td colspan="5">
        <% Html.RenderPartial("PageControl"); %>
				<div class="clear"></div>
			</td>
		</tr>
		</tfoot>
		</table>
	</div>
    <div id="BackGroundLoadingIndicator" class="bgLoadingIndicator" style="display: none">
    </div>
    <div id="LoadingIndicatorPanel"  style="display: none; position:  ">
        <img id="imgLoading" class="loader" alt='<%= UIConstants.GC_LoadingAltText %>' style="display: none" src='<%=UIConstants.GC_LoadingImagePath %>' longdesc='<%= UIConstants.GC_LoadingLongDesc %>' />
    </div>

	<% Html.RenderPartial("Bookmark"); %>
</asp:Content>

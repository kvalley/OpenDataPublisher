<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ODP_MasterOriginal.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Odp.InteractiveSdk.Mvc" %>
<%@ Import Namespace="Odp.InteractiveSdk.Mvc.Models" %>
<%@ Import Namespace="Resources" %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" language="javascript" src="<%= Url.Content("/Scripts/DataBrowser.min.js") %>"></script>
    <script type="text/javascript" language="javascript" src="<%= Url.Content("/Scripts/ogdi/list.js") %>"></script>
    <script type="text/javascript">
        var fieldNames = ["Image", "Name", "Description", "Category", "Status", "Date", "Rating", "Views"];
        setListParameters("/DataCatalog/DataSets", "/DataCatalog/GetListDataJSON", fieldNames);

        $(document).ready(function () {
            $("#SubmitFilter").click(function () {
                filter = submitFilter();
                setPage(1);
                updateListData();
            });
            $("#ClearFilter").click(function () {
                clearFilter();
                filter = submitFilter();
                setPage(1);
                updateListData();
            });
            $("#SubmitFilterSimple").click(function () {
                filter = submitFilter();
                setPage(1);
                updateListData();
            });
            $("#ClearFilterSimple").click(function () {
                clearFilterSimple();
                filter = submitFilter();
                setPage(1);
                updateListData();
            });
        });

    </script>
</asp:Content>
<asp:Content ID="mainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="top5">
        <table border="0">
            <tr>
                <td style="border-width: 0px;">
                    <% Html.RenderPartial("TopList", Model, new ViewDataDictionary(ViewData) { { "TopOf", Field.Date } }); %>
                </td>
                <td>
                    <% Html.RenderPartial("TopList", Model, new ViewDataDictionary(ViewData) { { "TopOf", Field.Rating } }); %>
                </td>
                <td>
                    <% Html.RenderPartial("TopList", Model, new ViewDataDictionary(ViewData) { { "TopOf", Field.Views } }); %>
                </td>
            </tr>
        </table>
    </div>
    <div class="dataset-filter form">

    <div class="bar">Datasets</div>
        <div class="content">
            <% 
                DatasetListModel model = ViewData.Model as DatasetListModel;
            %>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td width="33%" align="left" valign="top">
                        <div class="category-block">
                            <div class="label">
                                Category</div>
                            <div id="CategoryGroup" class="items">
                                <%	var index = 0; 
                                    foreach (String category in model.Categories)
                                    {
                                        var id = "cat" + (index++);
                                %>
                                <div class="item">
                                    <input type="checkbox" name="<%=category%>" value="<%=category%>" id="<%= id %>" />
                                    <label for="<%= id %>"><%= Html.Encode(category) %></label>
                                    <% if (Page.User.Identity.IsAuthenticated)
                                       {
                                    %>
                                    <a href = "/Category/<%= category%>">edit</a>
                                    <% } %>
                                </div>
                                <% } %>
                            </div>
                        </div>
                    </td>
                    <td width="33%" align="left" valign="top">
                        <%if (model.AllContainers.GetEnumerator().MoveNext())
                          {%>
                        <div class="data-source-block">
                            <div class="label">
                                Contributor</div>
                            <div id="DataSourceGroup" class="items">
                                <% index = 0; foreach (Container container in model.AllContainers)
                                   {
                                       var id = "ds" + (index++);
                                %>
                                <div class="item">
                                    <input type="checkbox" name="<%=container.Alias%>" value="<%=container.Alias%>" id="Checkbox1" />
                                    <label for="<%= id %>"><%= Html.Encode(container.Alias) %></label>
                                    <% if (Page.User.Identity.IsAuthenticated)
                                       {
                                    %>
                                    <a href = "/Contributor/<%= container.Alias%>">edit</a>
                                    <% } %>
                                </div>
                                <% } %>
                            </div>
                        </div>
                        <% } %>
                    </td>
                    <td width="33%" align="left" valign="top">
                            <% Html.RenderPartial("DataSetsFilter", Model); %>
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <% var sortImageSrc = ResolveUrl("~/Images/t.gif"); %>
    <br />
    <div class="dataset-list">
        <table border="0">
            <thead>
                <tr>
                    <td id="Image" style="width: 170px;">Image</td>
                    <td id="Name" style="width: 450px;">Name<img src="<%= sortImageSrc %>" /></td>
                    <td id="ColumnAPI" style="width: 40px; text-align: center;">API</td>
                    <td id="DownloadHTML" style="width: 40px; text-align: center;">SHP</td>
                    <td id="DownloadKML" style="width: 40px; text-align: center;">KML</td>
                    <td id="DownloadRSS" style="width: 40px; text-align: center;">RSS</td>
                    <td></td>
                    <%--
			  <td id="Category" width="12.5%" class="ascna">Category<img src="<%= sortImageSrc %>" /></td>
			<td id="Status" width="12.5%" class="ascna">Status<img src="<%= sortImageSrc %>" /></td>
			<td id="Date" width="12.5%" class="descna">Date<img src="<%= sortImageSrc %>" /></td> --%>
                </tr>
            </thead>
            <tbody class="rows"></tbody>
            <tfoot>
                <tr>
                    <td colspan="2">
                        <%if (false)
                          { %>
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
    <div id="LoadingIndicatorPanel" style="display: none; position: ">
        <img id="imgLoading" class="loader" alt='<%= UIConstants.GC_LoadingAltText %>' style="display: none" src='<%=UIConstants.GC_LoadingImagePath %>' longdesc='<%= UIConstants.GC_LoadingLongDesc %>' />
    </div>
    <% Html.RenderPartial("Bookmark"); %>
</asp:Content>

<%@ Import Namespace="Odp.InteractiveSdk.Mvc.Models" %>
<%@ Import Namespace="Odp.InteractiveSdk.Mvc.Models.Rating" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Odp.InteractiveSdk.Mvc.Models.DatasetListModel>" %>
<%@ Import Namespace="Resources"%>
<% 
	var odd = true;
	foreach (EntitySetWrapper entity in ViewData.Model.MainList)
   {%>

	<%
		string link = this.ResolveUrl("~/DataBrowser/" + entity.EntitySet.ContainerAlias + "/" + entity.EntitySet.EntitySetName + "#param=NOFILTER--DataView--Results");
	%>
<tr class="<%= odd ? "co" : "ce" %>">
	<td style="text-align: center;">
	    <a href="<%=link %>"><img src="<%= entity.EntitySet.Image %>" style="margin:0px; height: 70px; width: 150px; border: 1px solid #282828;"/></a>
	</td>
	<td>
		<a href="<%=link%>"><b><%=entity.EntitySet.Name%></b></a>
		<div class="description" style="padding-top: 10px;"><%=entity.EntitySet.Description%></div>
	</td>
    
    <% 
       var downloadHTML = "";
       var downloadKML = "";
       var downloadRSS = "";
       var apiEnabledText = "";
      
        foreach (OrderedDictionary download in entity.EntitySet.DownloadLinks)
       {
                  // TODO pull types out to config file
            if(download["Type"].ToString() == "SHP") {
                downloadHTML = "<a href=\"" + download["Link"].ToString() + "\"><img src=\"/Images/Valid_Green.png\" style = \"height: 20px; width: 20px;\"></a>";
            }
            else if(download["Type"].ToString() =="KML") {
                downloadKML = "<a href=\"" + download["Link"].ToString() + "\"><img src=\"/Images/Valid_Green.png\" style = \"height: 20px; width: 20px;\"></a>";
            }
            else if (download["Type"].ToString() == "RSS") {
                downloadRSS = "<a href=\"" + download["Link"].ToString() + "\"><img src=\"/Images/Valid_Green.png\" style = \"height: 20px; width: 20px;\"></a>";
            }
          
       }

        if (!entity.EntitySet.IsDownloadOnly)
        {
            apiEnabledText = "<a href=" + link + "><img src=\"" + UIConstants.DPC_ApiEnabledImagePath + "\" style='height: 20px; width: 20px;'></a>";
        }
        %>
    
   

    <td class="downloadLink"><%= apiEnabledText %></td>
    <td class="downloadLink"><%= downloadHTML %></td>
    <td class="downloadLink"><%= downloadKML %></td>
    <td class="downloadLink"><%= downloadRSS %></td>
    <td class="downloadLink">
        <%
            var isAuth = Page.User.Identity.IsAuthenticated;

            if (isAuth)
                cmdEdit.Visible = true;
            else
                cmdEdit.Visible = false;

            cmdEdit.NavigateUrl = "/DataLoader/" + entity.EntitySet.ContainerAlias + "/" + entity.EntitySet.EntitySetName;
        %>
        <a href="<%=link%>">View More Formats</a> <br /><br />
        <asp:HyperLink ID="cmdEdit" NavigateUrl = "/Login/" runat="server">Edit</asp:HyperLink>
        <input id="total" type="hidden" value="<%=ViewData.Model.PageCount%>" />
    </td>
	<%-- <td>
		<%=entity.EntitySet.CategoryValue%>
	</td>
	<td>
	  <%=entity.EntitySet.IsEmpty ? "Planned" : "Published"%>
	</td>
	<td>
		<%=entity.EntitySet.LastUpdateDate.ToString("MM/dd/yyyy")%>
	</td>
	<td>
		<% Html.RenderPartial("Rates", new RateInfo(entity.EntitySet.ItemKey , entity.PositiveVotes, entity.NegativeVotes) ); %>	</td> 
	<td>
		<%=entity.Views.ToString()%>
	</td> 
    --%>
</tr>
<% odd = !odd; } %>

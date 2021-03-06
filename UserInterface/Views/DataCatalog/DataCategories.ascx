<%@ Import Namespace="Odp.UserInterface"%>
<%@ Import Namespace="Odp.UserInterface.Models"%>
<%@ Import Namespace="Odp.UserInterface.App_Resources"%>
<%@ Control Language="C#" Inherits="Odp.UserInterface.OgdiViewUserControl<Odp.UserInterface.Models.DataCatalogModel>" %>

<td valign="top">
    <div class="leftmenu">
        <div id="CategoryPanel">
            <table cellpadding="0" cellspacing="0">
            <tr>
                <td>
                <% if ((string.IsNullOrEmpty(ViewData.Model.CurrentCategory)) || (ViewData.Model.CurrentCategory.Equals(UIConstants.DCPC_AllDataSetsText))) { %>
                    <a id="All" href="javascript:categoryLinkClicked('All');" style="font-weight: bold">All</a>
                <% } else { %> 
                    <a id="All" href="javascript:categoryLinkClicked('All');">All</a>
                <% } %>
                </td>
            </tr>
            <% try
               {
                   foreach (string item in ViewData.Model.CategoryList)
                   { %>
            <tr>
                <td>
                    <% if (!string.IsNullOrEmpty(ViewData.Model.CurrentCategory) && item.Equals(ViewData.Model.CurrentCategory)) { %>
						<a id="<%= item %>" href="javascript:categoryLinkClicked('<%= item %>');" style="font-weight: bold"><%= item%></a>
                    <% } else { %>                    
						<a id="<%= item %>" href="javascript:categoryLinkClicked('<%= item %>');"><%= item%></a>
                    <% } %>
                </td>
            </tr>
            <%		}
               }
               catch (Exception) { } %>
            </table>
        </div>
    </div>
</td>
<td valign="top" id="rightPanelDiv">
    <% 
        Html.RenderPartial("EntitySets", Model); 
    %></td>

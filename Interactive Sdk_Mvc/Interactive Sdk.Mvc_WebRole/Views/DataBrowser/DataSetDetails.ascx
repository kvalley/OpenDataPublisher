<%@ Import Namespace="Odp.InteractiveSdk.Mvc.Models" %>
<%@ Import Namespace="Odp.InteractiveSdk.Mvc.Models.Rating" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EntitySetWrapper>" %>



<div class="dataset-details">
	<% var model = this.Model.EntitySet; %>

	<div class="bar">
		<%--onclick="$(this).toggleClass('expanded');$('#eidDatasetDetails').toggle();" title="Click to see details">--%>
		Dataset Details
		<%--<div class="toggler">
			<img src="<%= this.ResolveUrl("~/Content/ico.png") %>" title="Expand" class="ico icoPlus" />
			<img src="<%= this.ResolveUrl("~/Content/ico.png") %>" title="Collapse" class="ico icoMinus" />
		</div>--%>
	</div>
	<div class="content" id="eidDatasetDetails">
		<table class="record" cellpadding="0" cellspacing="0" border="0">
		
		<tr class="field">
			<td class="label">Dataset name</td>
			<td class="value"><%= Html.Encode(model.Name) %></td>
		</tr>
		
		<tr>
			<td class="label">Data source</td>
			<td class="value"><%= Html.Encode(model.SourceDescription) %></td>
		</tr>
		
		<tr>
			<td class="label">Category</td>
			<td class="value"><%= Html.Encode(model.CategoryValue) %></td>
		</tr>		
		
        <%var emptyDate = new DateTime();
          if(model.ReleasedDate != emptyDate){ %>
		<tr>
			<td class="label">Released Date</td>
			<td class="value"><%= Html.Encode(EntitySet.GetEntityDateAsString(model.ReleasedDate))%></td>
		</tr>
		<%} %>

        <%if(model.LastUpdateDate != emptyDate){ %>
		<tr>
			<td class="label">Last Updated Date</td>
			<td class="value"><%= Html.Encode(EntitySet.GetEntityDateAsString(model.LastUpdateDate))%></td>
		</tr>
		<%} %>

        <%if(model.ExpiredDate != emptyDate){ %>
		<tr>
			<td class="label">Expired Date</td>
			<td class="value"><%= Html.Encode(EntitySet.GetEntityDateAsString(model.ExpiredDate))%></td>
		</tr>
		<%} %>
		
        <%if(!string.IsNullOrEmpty(model.UpdateFrequency)){ %>
		<tr>
			<td class="label">Update frequency</td>
			<td class="value"><%= Html.Encode(model.UpdateFrequency) %></td>
		</tr>
		<%} %>
		
        <%if(!string.IsNullOrEmpty(model.Description)){ %>
		<tr>
			<td class="label">Description</td>
			<td class="value"><%= Html.Encode(model.Description) %></td>
		</tr>
		<%} %>
		
		<tr>
			<td class="label">Status</td>
			<td class="value">
			    <% if(model.IsEmpty) { %>
			        <%= Html.Encode("Planned") %>
			    <% } else { %>
			        <%= Html.Encode("Published") %>
			    <% } %>			
			</td>
		</tr>				

        <%if(!string.IsNullOrEmpty(model.PeriodCovered)){ %>
		<tr>
			<td class="label">Time period covered</td>
			<td class="value"><%= Html.Encode(model.PeriodCovered) %></td>
		</tr>
		<%} %>
		
        <%if(!string.IsNullOrEmpty(model.GeographicCoverage)){ %>
		<tr>
			<td class="label">Geographic area covered</td>
			<td class="value"><%= Html.Encode(model.GeographicCoverage) %></td>
		</tr>
		<%} %>
		
        <%if(!string.IsNullOrEmpty(model.CollectionMode)){ %>
		<tr>
			<td class="label">Collection Mode</td>
			<td class="value"><%= Html.Encode(model.CollectionMode) %></td>
		</tr>		
		<%} %>
		
        <%if(!string.IsNullOrEmpty(model.MetadataUrl)){ %>
		<tr>
			<td class="label">Metadata Url</td>
			<td class="value"><%= Html.Encode(model.MetadataUrl) %></td>
		</tr>		
		<%} %>
		
        <%if(!string.IsNullOrEmpty(model.Keywords)){ %>
		<tr>
			<td class="label">Keywords</td>
			<td class="value"><%= Html.Encode(model.Keywords) %></td>
		</tr>
		<%} %>
		
        <%if(!string.IsNullOrEmpty(model.Links)){ %>
		<tr>
			<td class="label">Links and references</td>
			<td class="value"><a href="<%=model.Links %>"><%= Html.Encode(model.Links) %></a></td>
		</tr>
		<%} %>
		
        <%if(!string.IsNullOrEmpty(model.CollectionInstruments)){ %>
		<tr>
			<td class="label">Collection Instruments</td>
			<td class="value"><%= Html.Encode(model.CollectionInstruments) %></td>
		</tr>
		<%} %>
		
        <%if(!string.IsNullOrEmpty(model.TechnicalInfo)){ %>
		<tr>
			<td class="label">Technical Documentation</td>
			<td class="value"><%= Html.Encode(model.TechnicalInfo) %></td>
		</tr>				
		<%} %>
		
        <%if(!string.IsNullOrEmpty(model.DataDictionaryVariables)){ %>
		<tr>
			<td class="label">Data Dictinary/Variables</td>
			<td class="value"><%= Html.Encode(model.DataDictionaryVariables) %></td>
		</tr>
		<%} %>
		
        <%if(!string.IsNullOrEmpty(model.AdditionalInformation)){ %>
		<tr>
			<td class="label">Additional information</td>
			<td class="value"><%= Html.Encode(model.AdditionalInformation) %></td>
		</tr>		
		<%} %>
		
		<%if(model.IsDownloadOnly){ %>
		<tr>
			<td class="label">Download</td>
			<td class="value"><a href="<%= model.DownloadLink %>"><%= Html.Encode(model.DownloadLink) %></a></td>
		</tr>		
		<%} %>
		
		</table>
	</div>
	
	<div class="bar">
	    Dataset Downloads
	</div>
    <div class="content" id="iedDatasetDownloads">
        <%if (model.DownloadLinks.Count == 0)
          { %>
        No additional downloads for this dataset. <br />
        <% }
          else
          { %>
        <% foreach (OrderedDictionary download in model.DownloadLinks)
           {
               if (download != null)
               { %>
                <div class="iedDatasetDownload">
                    <div class="iedDatasetDownloadDetail" id="downloadIcon">
                        <img src="<%= download["IconLink"] %>" />
                    </div>
                    <div class="iedDatasetDownloadDetail" id="downloadLink">
                        <a href="<%= Url.Action("Index","Download", new { container = model.ContainerAlias, entitySetName = model.EntitySetName, downloadID = download["ID"]} ) %>" target="_blank"><%= download["Name"]%></a>
                    </div>
                    <div class="iedDatasetDownloadDetail" id="downloadCount">
                        <% if (string.IsNullOrEmpty(download["DownloadCount"].ToString())) { %>
                            0 downloads
                        <% } else { %>
                           <%= download["DownloadCount"]%> downloads
                        <% }  %>
                    </div>
                    <div class="clear"></div>
                </div>
        <%  }
           }
          }%>
        
    </div>
	
</div>

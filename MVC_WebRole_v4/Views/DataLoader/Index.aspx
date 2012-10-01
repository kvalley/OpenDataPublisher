<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ODP_MasterOriginal.Master" Inherits="System.Web.Mvc.ViewPage<Odp.InteractiveSdk.Mvc.Models.DataLoaderModel>" %>
<%@ Import Namespace="Odp.InteractiveSdk.Mvc.Models" %>
<%@ Import Namespace="Odp.InteractiveSdk.Mvc.Models.Rating" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="Odp.InteractiveSdk.Mvc" %>
<%@ Import Namespace="Odp.InteractiveSdk.Mvc.Models" %>
<%@ Import Namespace="Odp.InteractiveSdk.Mvc.Models.Comments" %>
<%@ Import Namespace="Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% 
        var model = this.Model;
        var error = string.Empty;

        if (model.ErrorMessage != null)
        {
            if (model.ErrorMessage.Length > 0)
            {
                error = "A required field is missing or blank (" + model.ErrorMessage.Substring(0, model.ErrorMessage.Length - 1) + "). Please correct the error and press save.";
                errorMessage.InnerText = error;
            }
        }

        List<SelectListItem> categories = new List<SelectListItem>();
        categories.Add(new SelectListItem { Text = "", Value = "" });
        foreach (string item in this.Model.OtherModel.Categories)
        {
            SelectListItem listItem = new SelectListItem { Text = item, Value = item, Selected = model.Category == item ? true : false };
            categories.Add(listItem);
        }

        List<SelectListItem> sources = new List<SelectListItem>();
        sources.Add(new SelectListItem { Text = "", Value = "" });
        foreach (Container item in this.Model.OtherModel.AllContainers)
        {
            SelectListItem listItem = new SelectListItem { Text = item.Alias, Value = item.Alias, Selected = model.DataSource == item.Alias ? true : false };
            sources.Add(listItem);
        }

        List<SelectListItem> delimiters = new List<SelectListItem>();
        delimiters.Add(new SelectListItem { Text = "Comma", Value = "Comma" });
        delimiters.Add(new SelectListItem { Text = "Tab", Value = "Tab" });

        List<SelectListItem> images = new List<SelectListItem>();
        images.Add(new SelectListItem { Text = "Property", Value = "http://data.northcowichan.ca/Images/custom/property.rec.gif" });
        images.Add(new SelectListItem { Text = "Building", Value = "http://data.northcowichan.ca/Images/custom/building.rec.gif" });
        images.Add(new SelectListItem { Text = "Business", Value = "http://data.northcowichan.ca/Images/custom/business.rec.gif" });
        images.Add(new SelectListItem { Text = "Garbage", Value = "http://data.northcowichan.ca/Images/custom/garbage.rec.gif" });
        images.Add(new SelectListItem { Text = "Assessment", Value = "http://data.northcowichan.ca/Images/custom/assessment.rec.gif" });
        images.Add(new SelectListItem { Text = "Recreation", Value = "http://data.northcowichan.ca/Images/custom/aqatics.rec.gif" });
    %>

    <form action="" method="post" enctype="multipart/form-data">
    <div class="dataset">
        <div class="name">Add/Edit Dataset Properties</div>
        <div class="description">Administration</div>		
    </div>
    <div class="tab-content" id="eidDataTabContent" style="display:block;">
        <div class="dataset-data">
            <div class="content">
                <div id="inputArea">
                    <label for="name">Name</label><%= Html.TextBox("name", model.Name) %>
                    <label for="dataSource">Contributor</label><%= Html.DropDownList("dataSource", sources) %>
                    <label for="category">Category</label><%= Html.DropDownList("category", categories) %>
                    <label for="icon">Icon</label><%= Html.DropDownList("icon", images) %>
                    <label for="description">Description</label><%= Html.TextArea("description", model.Description) %>

                    <%
                        if (model.FileLocation == null || model.FileLocation == string.Empty)
                    {
                        if (model.FileLocation == string.Empty || model.FileLocation == null)
                        {
                    %>
                            <label for="file">Source File (.xls)</label><input type="file" name="file" id="file" />
                    <%
                        }
                        else
                        {
                    %>
                            <label for="name" style ="width:100%"><%= model.FileLocation %></label>
                    <% 
                        } 
                    }
                    %>
                    <br /><br />
                    <fieldset>
                        <legend>Importing Delimited Files</legend>
                        <label for="table">Data Table</label><%= Html.TextBox("table", model.Table) %>
                        <div><label for="headers">Has Headers</label><%= Html.CheckBox("hasHeaders") %></div>                    
                        <div><label for="delimiter">Delimiter</label><%= Html.DropDownList("delimiter", delimiters) %></div>   
                    </fieldset>

                    <div id="inputSubmit">
                    <%
                    if (model.NewRecord)
                    {
                    %>
                        <input name="save" value="Upload" type = "submit"/>
                        <input name="save" value="Import" type = "submit"/>
                    <%
                    }
                    else
                    {
                    %>
                        <input name="save" value="Save" type = "submit"/>
                    <% 
                    }
                    %>
                        <input name="save" value="Cancel" type = "submit"/>       
                    </div>
                    <label id ="errorMessage" style ="color:red; text-align:center;" runat="server"></label>
                    <%= Html.Hidden("newRecord") %>
                    <%= Html.Hidden("dataSet") %>
                    <%= Html.Hidden("fileLocation") %>
                    <%= Html.Hidden("uploadOnly") %>
                </div>
            </div>
        </div>
    </div>
    </form>
</asp:Content>



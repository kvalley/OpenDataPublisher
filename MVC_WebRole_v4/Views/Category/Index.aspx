<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ODP_MasterOriginal.Master" Inherits="System.Web.Mvc.ViewPage<Odp.InteractiveSdk.Mvc.Models.CategoryModel>" %>
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
    %>
    <form action="" method="post" enctype="multipart/form-data">
        <div class="dataset">
            <div class="name">Add/Edit Categories</div>
            <div class="description">Administration</div>		
        </div>
        <div class="tab-content" id="eidDataTabContent" style="display:block;">
            <div class="dataset-data">
                <div class="content">
                    <div id="inputArea">
                        <%= Html.Hidden("id") %>
                        <label for="name">Name</label><%= Html.TextBox("name", model.Name) %>
                        <label id ="errorMessage" style ="color:red; text-align:center;" runat="server"></label>
                        <input name="save" value="Save" type = "submit"/>
                        <input name="save" value="Cancel" type = "submit"/>      
                    </div>
                </div>
           </div>
        </div>
    </form>
</asp:Content>
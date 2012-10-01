<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ODP_MasterOriginal.Master" Inherits="System.Web.Mvc.ViewPage<Odp.InteractiveSdk.Mvc.Models.LoginModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Html.BeginForm())
       { %>
    <div class="dataset">
        <div class="name">Administration Logon</div>
        <div class="description">CivicInfo BC Data Catalouge</div>
    </div>
    <div class="tab-content" id="eidDataTabContent" style="display: block;">
        <div class="dataset-data">
            <div class="content">
                <div id="inputArea">
                    <label for="username">User Name</label><%= Html.TextBox("username") %>
                    <label for="username">Password</label><%= Html.Password("password") %>
                    <%= Html.CheckBox("persistent") %><label for="persistent">Remember Me?</label>
                    <div id="inputSubmit">
                        <input name="login" value="Login" type = "submit"/>
                        <input name="login" value="Cancel" type = "submit"/>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

 
<script type="text/javascript">
  $().ready(function() {
    //add the target overlay area
    $('body').append('<div id="openid_overlay" style="-moz-border-radius:8px;-webkit-border-radius:8px; text-align:left;background:#fcfcfc;display:none;padding:1em 2em;border:solid 3px #555"><div class="wrap" style="width:512px"/></div>');
    //plugin the overlay to the trigger button
    $('#login_control a').overlay({ expose: '#444'
        , onBeforeLoad: function() { var wrap = this.getContent().find('div.wrap'); if (wrap.is(':empty')) { wrap.load(this.getTrigger().attr('href'), function() { $('form.openid').attr('action', '/CustomAccount/LogOn?returnUrl=<%=Request.Url%>').openid(); }); } }
    });
  });
</script>

<div id="login_control">
    <% if (Request.IsAuthenticated) { %>
        Welcome <b><%= Html.Encode(Page.User.Identity.Name) %></b>!    
    <% } else { %>
       <a title="Login" rel="#openid_overlay" href="/CustomAccount/OpenIdForm">Login using OpenID</a>
    <% } %>        
	
</div>




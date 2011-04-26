<%@ Page Title="Log in" Language="C#" Inherits="System.Web.Mvc.ViewPage<LoginViewModel>" %>
<%@ Import Namespace="DotNetOpenAuth.Mvc" %>
<%@ Import Namespace="DotNetOpenAuth.OpenId.RelyingParty" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title>Log in</title>

    <link rel="Stylesheet" type="text/css" href="<%= Url.Content("~/content/site.css") %>" />
    <%= Html.OpenIdSelectorStyles(this) %>
  
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.3/jquery.min.js" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jquery.cookie.js") %>" type="text/javascript"></script>
    <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.1/jquery-ui.min.js" type="text/javascript"></script>
    <%= Html.OpenIdSelectorScripts(this, null, new OpenIdAjaxOptions {
        PreloadedDiscoveryResults = Model.PreloadedDiscoveryResults
    }) %>
  </head>
  <body>
    <div id="background">
      <div id="login">
        <%= Html.ValidationMessage("Login") %>
   
        <% using (Html.BeginForm("OpenIdLoginPostAssertion", "Access", FormMethod.Post, new { target = "_top", @class = "login" })) { %>
          <%= Html.AntiForgeryToken() %>
          <%= Html.Hidden("ReturnUrl", Request.QueryString["ReturnUrl"], new { id = "ReturnUrl" }) %>
          <%= Html.Hidden("openid_openidAuthData") %>
         
          Log in using <%= Html.OpenIdSelector(this.Page, new SelectorButton[] {
              new SelectorProviderButton("https://www.google.com/accounts/o8/id", Url.Content("~/Content/images/openid/google.png")),
              new SelectorOpenIdButton(Url.Content("~/Content/images/openid.png"))
          }) %>
        <% } %>
      </div>
    </div>
  </body>
</html>
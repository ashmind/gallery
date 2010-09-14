<%@ Page Title="Log in" Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="System.Web.Mvc.ViewPage<LoginViewModel>" %>
<%@ Import Namespace="DotNetOpenAuth.Mvc" %>
<%@ Import Namespace="DotNetOpenAuth.OpenId.RelyingParty" %>

<asp:Content ContentPlaceHolderID="head" runat="server">
  <%= Html.OpenIdSelectorStyles(this) %>
  
  <script src="<%= Url.Content("~/Scripts/jquery.cookie.js") %>" type="text/javascript"></script>
  <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.1/jquery-ui.min.js" type="text/javascript"></script>
  <%= Html.OpenIdSelectorScripts(this, null, new OpenIdAjaxOptions {
      PreloadedDiscoveryResults = Model.PreloadedDiscoveryResults
  }) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="content" runat="server">
  <div class="background">
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
</asp:Content>
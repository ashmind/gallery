<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="System.Web.Mvc.ViewPage<GalleryViewModel>" %>

<asp:Content ContentPlaceHolderID="head" runat="server">
  <script src="<%= Url.Content("~/scripts/jquery.fancybox-1.3.1.js") %>" type="text/javascript"></script>
  <script src="<%= Url.Content("~/scripts/jquery.lazyload.js") %>" type="text/javascript"></script>
  <script src="<%= Url.Content("~/scripts/jquery.dragToSelect.js") %>" type="text/javascript"></script>
  <script src="<%= Url.Content("~/scripts/jquery.form.js") %>" type="text/javascript"></script>
  <script src="<%= Url.Content("~/scripts/jquery.history.js") %>" type="text/javascript"></script>
  <script src="<%= Url.Content("~/scripts/gallery.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="title" runat="server">
  <%= Request.Url.Host %> gallery <% if (Model.Selected != null) { %>
    : <%= Model.Selected.Album.Name %>
  <% } %>
</asp:Content>

<asp:Content ContentPlaceHolderID="content" runat="server">
  <div id="left">
    <% Html.RenderPartial("AlbumNames"); %>
  </div>
  
  <div id="main">    
    <% if (Model.Selected != null) { %>
      <% Html.RenderPartial("Album", Model.Selected); %>
    <% } %>
  </div>

  <!--div id="right-menu">
    <h2>Selected photos:</h2>
  </div-->
</asp:Content>
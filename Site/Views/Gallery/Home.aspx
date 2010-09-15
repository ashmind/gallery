<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="System.Web.Mvc.ViewPage<GalleryViewModel>" %>

<asp:Content ContentPlaceHolderID="head" runat="server">
  <script src="<%= Url.Content("~/scripts/gallery.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="title" runat="server">
  <%= Request.Url.Host %> gallery <% if (Model.SelectedAlbum != null) { %>
    : <%= Model.SelectedAlbum.Name %>
  <% } %>
</asp:Content>

<asp:Content ContentPlaceHolderID="content" runat="server">
  <div id="left">
    <% Html.RenderPartial("AlbumNames"); %>
  </div>
  
  <div id="main">    
    <% if (Model.SelectedAlbum != null) { %>
      <% Html.RenderPartial("Album", Model.SelectedAlbum); %>
    <% } %>
  </div>
</asp:Content>
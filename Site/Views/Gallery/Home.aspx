<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="System.Web.Mvc.ViewPage<GalleryViewModel>" %>

<asp:Content ContentPlaceHolderID="title" runat="server">
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
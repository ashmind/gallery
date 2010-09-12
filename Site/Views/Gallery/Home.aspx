<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="System.Web.Mvc.ViewPage<GalleryViewModel>" %>

<asp:Content ContentPlaceHolderID="title" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="content" runat="server">
  <div id="left">
    <% foreach (var album in Model.Albums) { %>
    <a class="block-link <%= Model.IsSelected(album) ? "selected" : "" %>" data-id='<%= album.ID %>' href='<%= Url.Action("Home", new { album = album.ID }) %>' title="<%= album.Name %>">
      <span class="image-count">(<%= album.Items.Count %>)</span> <%= album.Name %>
    </a>
    <% } %>
  </div>
  
  <div id="main">
    <div id="wall">
      <% if (Model.SelectedAlbum != null) { %>
        <% foreach (var item in Model.SelectedAlbum.Items) { %>
          <span class="item">
            <a rel="gallery-item" href="<%= Url.Action("Get", "Image", new { album = Model.SelectedAlbum.ID, item = item.Name, size = ImageSize.Preview.ToString().ToLowerInvariant() }) %>" class="image-view"> 
               <img src="<%= Url.Action("Get", "Image", new { album = Model.SelectedAlbum.ID, item = item.Name, size = ImageSize.Small.ToString().ToLowerInvariant() }) %>" />
            </a>
          </span>
        <% } %>
      <% } %>
    </div>
  </div>
</asp:Content>
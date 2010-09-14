<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<GalleryAlbum>" %>

<div class="album-view">
  <!--div class="header">
    <div class="title"><%= Model.Name %></div>
  </div-->

  <div class="wall">
    <% foreach (var item in Model.Items) { %>
      <span class="item">
        <a rel="gallery-item" href="<%= Url.Action("Get", "Image", new { album = Model.ID, item = item.Name, size = ImageSize.Preview.ToString().ToLowerInvariant() }) %>" class="image-view"> 
            <img src="<%= Url.Action("Get", "Image", new { album = Model.ID, item = item.Name, size = ImageSize.Small.ToString().ToLowerInvariant() }) %>" />
        </a>
      </span>
    <% } %>
  </div>
</div>
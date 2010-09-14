<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<GalleryAlbum>" %>

<% foreach (var item in Model.Items) { %>
  <span class="item">
    <a rel="gallery-item" href="<%= Url.Action("Get", "Image", new { album = Model.ID, item = item.Name, size = ImageSize.Preview.Name.ToLowerInvariant() }) %>" class="image-view"> 
        <img src="<%= Url.Content("~/content/images/blank.gif") %>"
             data-lazysrc="<%= Url.Action("Get", "Image", new { album = Model.ID, item = item.Name, size = ImageSize.Small.Name.ToLowerInvariant() }) %>" />
    </a>
  </span>
<% } %>
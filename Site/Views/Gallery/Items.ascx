<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AlbumViewModel>" %>

<% foreach (var item in Model.Album.Items.OrderBy(i => i.Date)) { %>
  <span class="item">
    <a rel="gallery-item"
       href="<%= Url.Action("Get", "Image", new { album = Model.ID, item = item.Name, size = ImageSize.Large.Name.ToLowerInvariant() }) %>"
       data-name="<%= item.Name %>"
       data-action-download="<%= Url.Action("Get", "Image", new { album = Model.ID, item = item.Name }) %>"
       data-action-comment="<%= Url.Action("View", "Gallery", new { album = Model.ID, item = item.Name }) %>"
       class="image-view"> 
        <img src="<%= Url.Content("~/content/images/blank.gif") %>"
             data-lazysrc="<%= Url.Action("Get", "Image", new { album = Model.ID, item = item.Name, size = ImageSize.Small.Name.ToLowerInvariant() }) %>" />
    </a>
  </span>
<% } %>
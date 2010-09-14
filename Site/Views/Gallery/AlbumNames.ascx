<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<GalleryViewModel>" %>
<% foreach (var album in Model.Albums) { %>
<a class="block-link album-name <%= Model.IsSelected(album) ? "selected" : "" %>" data-id='<%= album.ID %>' href='<%= Url.Action("Home", new { album = album.ID, count = Model.Albums.Count }) %>' title="<%= album.Name %>">
  <span class="image-count">(<%= album.Items.Count %>)</span> <%= album.Name %>
</a>
<% } %>

<a class="block-link more"
   href="<%= Url.Action("Home", new { album = Model.SelectedAlbum != null ? Model.SelectedAlbum.ID : null, count = Model.Albums.Count + 20 }) %>"
   data-ajax="<%= Url.Action("AlbumNames", new { start = Model.Albums.Count + 1, count = 20 }) %>">
  More
</a>
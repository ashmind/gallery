<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<GalleryViewModel>" %>
<% foreach (var album in Model.Albums) { %>
<a class="block-link <%= Model.IsSelected(album) ? "selected" : "" %>" data-id='<%= album.ID %>' href='<%= Url.Action("Home", new { album = album.ID }) %>' title="<%= album.Name %>">
  <span class="image-count">(<%= album.Items.Count %>)</span> <%= album.Name %>
</a>
<% } %>
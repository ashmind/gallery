<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<GalleryViewModel>" %>

<section class="people">
  <% if (Model.CurrentUserAlbum != null || Model.OtherPeopleAlbums.Count > 0) { %>
    <span class="album-name-section">People</span>
  <% } %>

  <% if (Model.CurrentUserAlbum != null) { %>
    <% var albumModel = Model.CurrentUserAlbum; %>
    <% var album = albumModel.Album; %>

    <a class="block-link album-name <%= Model.IsSelected(album) ? "selected" : "" %>" data-id='<%= albumModel.ID %>' href='<%= Url.Action("Home", new { album = albumModel.ID }) %>' title="<%= album.Name %>">
      <span class="image-count">(<%= album.Items.Count %>)</span>
      You: <%= album.Name %>
    </a>
  <% } %>

  <% var index = 0; %>
  <% foreach (var albumModel in Model.OtherPeopleAlbums) { %>
    <% index += 1; %>

    <% if (index == 5) { %>
      <a class="block-link more">All</a>
      <div class="more">
    <% } %>

        <a class="block-link album-name <%= Model.IsSelected(albumModel.Album) ? "selected" : "" %>" data-id='<%= albumModel.ID %>' href='<%= Url.Action("Home", new { album = albumModel.ID }) %>' title="<%= albumModel.Album.Name %>">
          <span class="image-count">(<%= albumModel.Album.Items.Count%>)</span>
          <%= albumModel.Album.Name%>
        </a>
  <% } %>

  <% if (index >= 5) { %>
      </div>
  <% } %>
</section>

<section class="folders">
  <% Html.RenderPartial("StandardAlbumNames"); %>
</section>
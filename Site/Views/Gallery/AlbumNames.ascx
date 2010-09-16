﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<GalleryViewModel>" %>
<% const int PageSize = 20; %>

<% for (var i = 0; i < Model.Albums.Count; i++) { %>
  <% var album = Model.Albums.List[i]; %>
  <% var index = Model.Albums.Start + i; %>

  <a class="block-link album-name <%= Model.IsSelected(album) ? "selected" : "" %>" data-id='<%= album.ID %>' href='<%= Url.Action("Home", new { album = album.ID, albumCount = index > PageSize ? (int?)index : null }) %>' title="<%= album.Name %>">
    <span class="image-count">(<%= album.Items.Count %>)</span> <%= album.Name %>
  </a>
<% } %>

<% if (Model.Albums.Count >= PageSize) { %>
  <a class="block-link more"
     href="<%= Url.Action("Home", new { album = Model.Selected != null ? Model.Selected.Album.ID : null, albumCount = Model.Albums.Count + 20 }) %>"
     data-ajax="<%= Url.Action("AlbumNames", new { start = Model.Albums.Count + 1, count = PageSize }) %>">
    More
  </a>
<% } %>
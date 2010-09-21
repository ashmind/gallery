﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<GalleryViewModel>" %>

<% const int PageSize = 20; %>

<% var year = Model.StandardAlbums.YearBeforeStart; %>
<% for (var i = 0; i < Model.StandardAlbums.Count; i++) { %>
  <% var albumModel = Model.StandardAlbums.List[i];
     var album = albumModel.Album;
     var index = Model.StandardAlbums.Start + i; %> 

  <% if (year != album.Date.Year) { %>
      <span class="album-name-section"><%= album.Date.Year %></span>
      <% year = album.Date.Year; 
   } %>

  <a class="block-link album-name <%= Model.IsSelected(album) ? "selected" : "" %>" data-id='<%= albumModel.ID %>' href='<%= Url.Action("Home", new { album = albumModel.ID, albumCount = index > PageSize ? (int?)index : null }) %>' title="<%= album.Name %>">
    <span class="image-count">(<%= album.Items.Count %>)</span>
    <%= album.Name %>
  </a>
<% } %>

<% if (Model.StandardAlbums.Count >= PageSize) { %>
  <a class="block-link more"
     href="<%= Url.Action("Home", new { album = Model.Selected != null ? Model.Selected.ID : null, albumCount = Model.StandardAlbums.Start + Model.StandardAlbums.Count + PageSize }) %>"
     data-ajax="<%= Url.Action("StandardAlbumNames", new { start = Model.StandardAlbums.Start + Model.StandardAlbums.Count + 1, count = PageSize }) %>">
    More
  </a>
<% } %>
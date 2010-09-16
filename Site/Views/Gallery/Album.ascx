<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AlbumViewModel>" %>

<div class="album-view">
  <!--div class="header">
    <div class="title"><%= Model.Album.Name %></div>
  </div-->

  <div class="wall">
    <% Html.RenderPartial("Items", Model.Album); %>
  </div>
</div>

<% if (Model.CanManageSecurity) { %>
  <% Html.RenderPartial("AlbumVisibleTo"); %>
<% } %>
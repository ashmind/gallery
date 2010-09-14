<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<GalleryAlbum>" %>

<div class="album-view">
  <!--div class="header">
    <div class="title"><%= Model.Name %></div>
  </div-->

  <div class="wall">
    <% Html.RenderPartial("Items"); %>
  </div>
</div>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AlbumViewModel>" %>

<a href="<%= Url.Action("Grant", "Access", new { albumID = Model.Album.ID }) %>" class="album-visible-to">
  visible to
  <% foreach (var group in Model.VisibleToGroups) { %>
    <div class="user-group" title="<%= group.Name %>">
      <% foreach (var user in group.Users) { %>
        <%= Html.Gravatar(user.Email, new { s = 20, d = "identicon" }, new { @class = "user" })%>
      <% } %>
    </div>
  <% } %>
</a>
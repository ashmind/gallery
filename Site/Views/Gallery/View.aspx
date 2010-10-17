<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="ViewPage<ItemDetailsViewModel>" %>

<asp:Content ContentPlaceHolderID="head" runat="server">
  <link rel="stylesheet" type="text/css" href="<%= Url.Content("~/content/item-details.css") %>" />
</asp:Content>

<asp:Content ContentPlaceHolderID="title" runat="server">
  <%= Model.Item.Name %>
</asp:Content>

<asp:Content ContentPlaceHolderID="content" runat="server">
  <div id="foreground">
    <div class="navigation">
      <%= Html.ActionLink("Back to Album", "Home", new { album = Model.AlbumID }, new { id = "back" }) %>
    </div>
    <img class="view" src="<%= Url.Action("Get", "Image", new { album = Model.AlbumID, item = Model.Item.Name, size = ImageSize.Small.Name.ToLowerInvariant() }) %>" />

    <div id="comments">
      <h2>Comments</h2>
      <% foreach (var comment in Model.Item.Comments) { %>
        <div class="comment">
          <%= Html.Gravatar(comment.Author.Email, new { s = "40" }) %>
          <span class="comment-text">
            <%= comment.Text %>
          </span>
        </div>
      <% } %>

      <% var realUser = Model.CurrentUser as AshMind.Gallery.Core.Security.User; %>
      <% if (realUser != null) { %>
        <form id="add-comment" class="comment" method="post" action="<%= Url.Action("Comment", new { album = Model.AlbumID, item = Model.Item.Name }) %>">
          <%= Html.Gravatar(realUser.Email, new { s = "40" })%>
          <%= Html.TextArea("comment")%>

          <button type="submit">Submit</button>
        </form>
      <% } %>
      <% else { %>
        <div>Key access is not enough to post comments, please log in.</div>
      <% } %>
    </div>
  </div>

  <div id="background"></div>
</asp:Content>
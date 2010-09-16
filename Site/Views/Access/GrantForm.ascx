<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<GrantViewModel>" %>

<% using (Html.BeginForm("Grant", "Access", null, FormMethod.Post, new { @class = "grant" })) { %>
  <%= Html.Hidden("albumID", this.Model.AlbumID) %>

  <% var index = 0; %>
  <% foreach (var group in Model.AllUserGroups) { %>
    <div title="<%= group.Name %>" class="user-group<%= Model.HasAccess(group) ? " granted" : "" %>" data-key="<%= group.Key %>">
      <% foreach (var user in group.Users) { %>
        <%= Html.Gravatar(user.Email, new { s = 60, d = "identicon" }, new { @class = "user" }) %>        
      <% } %>
      <%= Html.Hidden("groupKeys[" + index + "]", Model.HasAccess(group) ? group.Key : "")%>
    </div>

    <% index += 1; %>
  <% } %>

  <div>
    <button type="submit">Save</button>
  </div>
<% } %>
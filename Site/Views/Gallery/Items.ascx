<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AlbumViewModel>" %>

<% foreach (var item in Model.Album.Items.OrderBy(i => i.Date)) { %>
  <% var proposedToBeDeletedByCurrentUser = item.DeleteProposals.Contains(Model.CurrentUser); %>

  <span class="item<%= item.IsProposedToBeDeleted ? " to-delete" : "" %>">
    <a <% if (!item.IsProposedToBeDeleted) { %>rel="normal-gallery-item"<% } %> 
       href="<%= Url.Action("Get", "Image", new { album = Model.ID, item = item.Name, size = ImageSize.Medium.Name.ToLowerInvariant() }) %>"
       data-name="<%= item.Name %>"
       data-json="{
          name : '<%= item.Name %>',

          <% if (item.PrimaryAlbum != null) { %>
          primaryAlbumID : '<%= Model.GetAdditionalAlbumID(item.PrimaryAlbum) %>',
          <% } %>

          actions : {
              'delete' : {
                  action : '<%= Url.Action(
                       (proposedToBeDeletedByCurrentUser ? "Revert" : "Propose") + "Delete", "Gallery",
                       new { album = Model.ID, item = item.Name }
                  ) %>',

                  text : '<%= proposedToBeDeletedByCurrentUser ? "Restore" : "Propose to delete" %>'
              },

              download : {
                  action : '<%= Url.Action("Get", "Image", new { album = Model.ID, item = item.Name }) %>',
                  sizes : {
                      '<%= ImageSize.Small.Size %>px' : '<%= ImageSize.Small.Name.ToLowerInvariant() %>',
                      '<%= ImageSize.Medium.Size %>px' : '<%= ImageSize.Medium.Name.ToLowerInvariant() %>',
                      '<%= ImageSize.Large.Size %>px' : '<%= ImageSize.Large.Name.ToLowerInvariant() %>',
                      '<%= ImageSize.Original.Name %>' : '<%= ImageSize.Original.Name.ToLowerInvariant() %>'
                  }
              },
              comment : '<%= Url.Action("View", "Gallery", new { album = Model.ID, item = item.Name }) %>'
          }
       }"
       class="image-view"> 
        <img src="<%= Url.Content("~/content/images/blank.gif") %>"
             data-lazysrc="<%= Url.Action("Get", "Image", new { album = Model.ID, item = item.Name, size = ImageSize.Thumbnail.Name.ToLowerInvariant() }) %>" />
    </a>
  </span>
<% } %>
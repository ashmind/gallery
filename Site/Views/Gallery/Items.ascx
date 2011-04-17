<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AlbumViewModel>" %>

<% Action<AlbumItem> renderItem = item => { %>
  <% var proposedToBeDeletedByCurrentUser = item.DeleteProposals.Contains(Model.CurrentUser); %>

  <span class="item">
    <a <% if (!item.IsProposedToBeDeleted) { %>rel="normal-gallery-item"<% } %> 
       href="<%= Model.ImageAccess.GetActionUrl(ViewContext.RequestContext, Model.ID, item.Name) + "/" + ImageSize.Medium.Name.ToLowerInvariant() %>"
       data-name="<%= item.Name %>"
       data-json="{
          name : '<%= item.Name %>',

          <% if (item.PrimaryAlbum != null) { %>
          primaryAlbumID : '<%= Model.GetAdditionalAlbumID(item.PrimaryAlbum) %>',
          <% } %>

          actions : {
              '<%= (proposedToBeDeletedByCurrentUser ? "restore" : "delete") %>' : {
                  action : '<%= Url.Action(
                       (proposedToBeDeletedByCurrentUser ? "Revert" : "Propose") + "Delete", "Gallery",
                       new { album = Model.ID, item = item.Name }
                  ) %>',

                  text : '<%= proposedToBeDeletedByCurrentUser ? "Restore" : "Propose to delete" %>'
              },

              download : {
                  action : '<%= Model.ImageAccess.GetActionUrl(ViewContext.RequestContext, Model.ID, item.Name) %>',
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
             data-lazysrc="<%= Model.ImageAccess.GetActionUrl(ViewContext.RequestContext, Model.ID, item.Name) + "/" + ImageSize.Thumbnail.Name.ToLowerInvariant() %>" />
    </a>
  </span>
<% }; %>

<section>
  <% foreach (var item in Model.Album.Items.Where(i => !i.IsProposedToBeDeleted).OrderBy(i => i.Date)) { %>
    <% renderItem(item); %>
  <% } %>
</section>

<% var proposedToBeDeleted = Model.Album.Items.Where(i => i.IsProposedToBeDeleted).ToArray(); %>

<% if (proposedToBeDeleted.Any()) { %>
  <a class="expand-to-delete" href="#">Show proposed to be deleted</a>
  <section class="to-delete">
    <h2>Proposed to be deleted</h2>
    <% foreach (var item in proposedToBeDeleted.OrderBy(i => i.Date)) { %>
      <% renderItem(item); %>
    <% } %>
  </section>
<% } %>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;

using AshMind.Gallery.Core;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Site.Logic;

namespace AshMind.Gallery.Site.Controllers {
    [Authorize]
    public class AlbumItemController : ControllerBase {
        private readonly IAlbumFacade gallery;

        public AlbumItemController(IAlbumFacade gallery, IUserAuthentication authentication)
            : base(authentication) {
            this.gallery = gallery;
        }

        public ActionResult ProposeDelete(string album, string item) {
            return ToggleDelete(album, item, albumItem => albumItem.ProposedToBeDeletedBy = (KnownUser)User);
        }

        public ActionResult RevertDelete(string album, string item) {
            return ToggleDelete(album, item, albumItem => albumItem.ProposedToBeDeletedBy = null);
        }

        private ActionResult ToggleDelete(string albumID, string itemName, Action<AlbumItem> action) {
            if (!Request.IsAjaxRequest())
                throw new NotImplementedException();

            var album = this.gallery.GetAlbum(albumID, User);
            if (album == null)
                return Unauthorized();

            var item = album.Items.Value.Single(i => i.Name == itemName);
            action(item);

            this.gallery.SaveItem(item);

            return Content(item.IsProposedToBeDeleted.ToString(), MediaTypeNames.Text.Plain);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using AshMind.Web.Gallery.Core;
using AshMind.Web.Gallery.Site.Models;
using AshMind.Web.Gallery.Core.Security;

namespace AshMind.Web.Gallery.Site.Controllers {
    [Authorize]
    public class GalleryController : Controller {
        private readonly AlbumFacade gallery;
        private readonly IRepository<User> userRepository;

        public GalleryController(AlbumFacade gallery, IRepository<User> userRepository) {
            this.gallery = gallery;
            this.userRepository = userRepository;
        }

        public ActionResult Home(string album, int count = 20) {
            var user = this.userRepository.FindByEmail(User.Identity.Name);

            if (Request.IsAjaxRequest())
                return AjaxAlbum(album, user);

            var albums = this.gallery.GetAlbums(user).Take(count).ToArray();
            var selected = albums.FirstOrDefault(f => f.ID == album);

            return View(new GalleryViewModel(albums, selected));
        }

        private ActionResult AjaxAlbum(string albumID, User user) {
            var album = this.gallery.GetAlbum(albumID, user) ?? GalleryAlbum.Empty;
            return PartialView("Album", album);
        }

        public ActionResult AlbumNames(int start, int count) {
            var user = this.userRepository.FindByEmail(User.Identity.Name);
            var albums = this.gallery.GetAlbums(user).Skip(start - 1).Take(count).ToArray();

            return PartialView(new GalleryViewModel(albums, null));
        }
    }
}

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

        public ActionResult Home(string album) {
            var user = this.userRepository.FindByEmail(User.Identity.Name);

            var albums = this.gallery.GetAlbums(user).Take(20).ToArray();
            var selected = albums.FirstOrDefault(f => f.ID == album);

            return View(new GalleryViewModel(albums, selected));
        }
    }
}

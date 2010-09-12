using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using AshMind.Web.Gallery.Core;
using AshMind.Web.Gallery.Site.Models;

namespace AshMind.Web.Gallery.Site.Controllers {
    [Authorize]
    public class GalleryController : Controller {
        private readonly AlbumFacade gallery;

        public GalleryController(AlbumFacade gallery) {
            this.gallery = gallery;
        }

        public ActionResult Home(string album) {
            var albums = this.gallery.GetAlbums().Take(20).ToArray();
            var selected = albums.FirstOrDefault(f => f.ID == album);

            return View(new GalleryViewModel(albums, selected));
        }
    }
}

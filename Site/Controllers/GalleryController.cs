using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using AshMind.Web.Gallery.Core;
using AshMind.Web.Gallery.Site.Models;
using AshMind.Web.Gallery.Core.Security;
using AshMind.Web.Gallery.Core.Commenting;

namespace AshMind.Web.Gallery.Site.Controllers {
    [Authorize]
    public class GalleryController : Controller {
        private readonly AlbumFacade gallery;
        private readonly ICommentRepository commentRepository;
        private readonly IRepository<User> userRepository;

        public GalleryController(
            AlbumFacade gallery,
            ICommentRepository commentRepository,
            IRepository<User> userRepository
        ) {
            this.gallery = gallery;
            this.commentRepository = commentRepository;
            this.userRepository = userRepository;
        }

        public ActionResult Home(string album, int count = 20) {
            var user = GetCurrentUser();

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
            var user = GetCurrentUser();
            var albums = this.gallery.GetAlbums(user).Skip(start - 1).Take(count).ToArray();

            return PartialView(new GalleryViewModel(albums, null));
        }

        public new ActionResult View(string album, string item) {
            var user = GetCurrentUser();
            var albumItem = this.gallery.GetItem(album, item, user);
            return View(
                new ItemViewModel(album, albumItem, user)
            );
        }

        public ActionResult Comment(string album, string item, string comment) {
            var author = GetCurrentUser();
            var path = this.gallery.GetFullPath(album, item);
            commentRepository.SaveComment(
                path, new Comment(author, DateTimeOffset.Now, comment)
            );

            return RedirectToAction("View", new { album, item });
        }

        private User GetCurrentUser() {
            return this.userRepository.FindByEmail(User.Identity.Name);
        }
    }
}

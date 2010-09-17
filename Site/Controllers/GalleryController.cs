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
        private readonly AuthorizationService authorization;
        private readonly ICommentRepository commentRepository;
        private readonly IRepository<User> userRepository;

        public GalleryController(
            AlbumFacade gallery,
            AuthorizationService authorization,
            ICommentRepository commentRepository,
            IRepository<User> userRepository
        ) {
            this.gallery = gallery;
            this.authorization = authorization;
            this.commentRepository = commentRepository;
            this.userRepository = userRepository;
        }

        public ActionResult Home(string album, int albumCount = 20) {
            var user = GetCurrentUser();

            if (Request.IsAjaxRequest())
                return AjaxAlbum(album, user);

            var albums = this.gallery.GetAlbums(user).Take(albumCount).ToArray();
            var selected = albums.FirstOrDefault(f => f.ID == album);

            return View(new GalleryViewModel(
                new AlbumListViewModel(albums, 0, null),
                ToViewModel(selected))
            );
        }

        private ActionResult AjaxAlbum(string albumID, User user) {
            var album = this.gallery.GetAlbum(albumID, user) ?? Album.Empty;
            return PartialView("Album", ToViewModel(album));
        }

        public ActionResult AlbumNames(int start, int count) {
            var user = GetCurrentUser();
            var albums = this.gallery.GetAlbums(user).Skip(start).Take(count + 1).ToArray();

            return PartialView(new GalleryViewModel(
                new AlbumListViewModel(
                    count > 0 ? albums.Skip(1).ToArray() : albums,
                    start,
                    count > 0 ? (int?)albums[0].Date.Year : null
                ),
                null
            ));
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

        private AlbumViewModel ToViewModel(Album album) {
            if (album == null)
                return null;

            if (!authorization.IsAuthorized(GetCurrentUser(), SecurableAction.ManageSecurity, null))
                return new AlbumViewModel(album);

            var token = this.gallery.GetAlbumToken(album.ID);
            return new AlbumViewModel(
                album, true, (
                    from @group in this.authorization.GetAuthorizedTo(SecurableAction.View, token)
                    select new UserGroupViewModel(@group)
                ).ToList()
            );
        }
    }
}

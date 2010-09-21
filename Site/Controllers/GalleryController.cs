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

        public ActionResult Home(string album, int albumCount = 20, string @as = null) {
            var user = GetCurrentUser();
            if (@as != null) {
                if (!this.authorization.IsAuthorized(user, SecurableAction.ManageSecurity, null))
                    return new HttpUnauthorizedResult();

                user = this.userRepository.Load(@as);
            }

            if (Request.IsAjaxRequest())
                return AjaxAlbum(album, user);

            var standardAlbums = this.GetStandardAlbums(user).Take(albumCount).ToArray();
            var peopleAlbumsAndCurrentPerson = this.GetPeopleAlbumsAndCurrentUserAlbum(user);
            var peopleAlbums = peopleAlbumsAndCurrentPerson.Item1;
            var currentUserAlbum = peopleAlbumsAndCurrentPerson.Item2;

            var selected = standardAlbums.FirstOrDefault(f => f.ID == album)
                        ?? peopleAlbums.FirstOrDefault(f => f.ID == album);

            if (selected == null && currentUserAlbum != null && currentUserAlbum.ID == album)
                selected = currentUserAlbum;

            return View(new GalleryViewModel(
                currentUserAlbum,
                peopleAlbums,
                new AlbumListViewModel(standardAlbums, 0, null),
                selected != null ? ToViewModel(selected.Album, true) : null
            ));
        }

        private ActionResult AjaxAlbum(string albumID, User user) {
            var album = this.gallery.GetAlbum(albumID, user) ?? Album.Empty;
            return PartialView("Album", ToViewModel(album, true));
        }

        public ActionResult StandardAlbumNames(int start, int count) {
            var user = GetCurrentUser();
            var albums = GetStandardAlbums(user)
                                     .Skip(start).Take(count + 1)
                                     .ToArray();

            return PartialView(new GalleryViewModel(
                null,
                new AlbumViewModel[0],
                new AlbumListViewModel(
                    count > 0 ? albums.Skip(1).ToArray() : albums,
                    start,
                    count > 0 ? (int?)albums[0].Album.Date.Year : null
                ),
                null
            ));
        }

        private IEnumerable<AlbumViewModel> GetStandardAlbums(User user) {
            return this.gallery.GetAlbums(AlbumProviderKeys.Default, user)
                               .OrderByDescending(a => a.Date)
                               .Select(a => ToViewModel(a, false));
        }

        private Tuple<IList<AlbumViewModel>, AlbumViewModel> GetPeopleAlbumsAndCurrentUserAlbum(User user) {
            var personAlbums = this.gallery.GetAlbums(AlbumProviderKeys.People, user)
                                           .OrderBy(p => p.Name)
                                           .Select(a => ToViewModel(a, false))
                                           .ToList();

            var indexOfUserAlbum = personAlbums.FindIndex(m => m.Album.Descriptor.ProviderSpecificPath == user.Email);
            var userAlbum = (AlbumViewModel)null;
            if (indexOfUserAlbum >= 0) {
                userAlbum = personAlbums[indexOfUserAlbum];
                personAlbums.RemoveAt(indexOfUserAlbum);
            }

            return new Tuple<IList<AlbumViewModel>, AlbumViewModel>(personAlbums, userAlbum);
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
            var path = this.gallery.GetItem(album, item, author).File.Path;
            commentRepository.SaveComment(
                path, new Comment(author, DateTimeOffset.Now, comment)
            );

            return RedirectToAction("View", new { album, item });
        }

        private User GetCurrentUser() {
            return this.userRepository.Load(User.Identity.Name);
        }

        private AlbumViewModel ToViewModel(Album album, bool manageSecurity) {
            if (album == null)
                return null;

            var id = this.gallery.GetAlbumID(album);
            var user = GetCurrentUser();
            if (!manageSecurity || !authorization.IsAuthorized(user, SecurableAction.ManageSecurity, null))
                return new AlbumViewModel(album, id);

            return new AlbumViewModel(
                album, id, true, (
                    from @group in this.authorization.GetAuthorizedTo(SecurableAction.View, album.SecurableToken)
                    select new UserGroupViewModel(@group)
                ).ToList()
            );
        }
    }
}

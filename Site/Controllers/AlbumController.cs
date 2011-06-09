using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using AshMind.Gallery.Core;
using AshMind.Gallery.Core.Security.Actions;
using AshMind.Gallery.Site.Models;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Site.Logic;

namespace AshMind.Gallery.Site.Controllers {
    [Authorize]
    public class AlbumController : ControllerBase {
        private readonly IAlbumFacade gallery;
        private readonly IAuthorizationService authorization;
        private readonly IImageRequestStrategy requestStrategy;

        public AlbumController(
            IAlbumFacade gallery,
            IAuthorizationService authorization,
            IUserAuthentication authentication,
            IImageRequestStrategy requestStrategy
        ) : base(authentication) {
            this.gallery = gallery;
            this.authorization = authorization;
            this.requestStrategy = requestStrategy;
        }

        public ActionResult Gallery(string album, int albumCount = 20) {
            var user = this.User;
            if (Request.IsAjaxRequest())
                return AjaxAlbum(album, user);

            var standardAlbums = this.GetAllStandard(user).Take(albumCount).ToArray();
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

        private ActionResult AjaxAlbum(string albumID, IUser user) {
            var album = this.gallery.GetAlbum(albumID, user) ?? Album.Empty;
            return PartialView("_View", ToViewModel(album, true));
        }

        public ActionResult StandardNames(int start, int count) {
            var albums = GetAllStandard(this.User)
                                     .Skip(start - 1).Take(count)
                                     .ToArray();

            return PartialView("_StandardNames", new GalleryViewModel(
                null,
                new AlbumViewModel[0],
                new AlbumListViewModel(
                    albums,
                    start,
                    count > 0 ? (int?)albums[0].Date.Year : null
                ),
                null
            ));
        }

        private IEnumerable<AlbumViewModel> GetAllStandard(IUser user) {
            return this.gallery.GetAlbums(AlbumProviderKeys.Default, user)
                               .OrderByDescending(a => a.Date)
                               .Select(a => ToViewModel(a, false));
        }

        private Tuple<IList<AlbumViewModel>, AlbumViewModel> GetPeopleAlbumsAndCurrentUserAlbum(IUser user) {
            var personAlbums = this.gallery.GetAlbums(AlbumProviderKeys.People, user)
                                           .OrderBy(p => p.Name)
                                           .Select(a => ToViewModel(a, false))
                                           .ToList();

            var realUser = user as KnownUser;
            var indexOfUserAlbum = realUser != null
                                 ? personAlbums.FindIndex(m => (string)m.Album.ProviderData == realUser.Email)
                                 : -1;

            var userAlbum = (AlbumViewModel)null;
            if (indexOfUserAlbum >= 0) {
                userAlbum = personAlbums[indexOfUserAlbum];
                personAlbums.RemoveAt(indexOfUserAlbum);
            }

            return new Tuple<IList<AlbumViewModel>, AlbumViewModel>(personAlbums, userAlbum);
        }

        private AlbumViewModel ToViewModel(Album album, bool manageSecurity) {
            if (album == null)
                return null;

            if (!manageSecurity || !authorization.IsAuthorized(this.User, SecurableActions.ManageSecurity))
                return new AlbumViewModel(album, this.gallery.GetAlbumID, this.User, this.requestStrategy, false, null);

            return new AlbumViewModel(
                album, this.gallery.GetAlbumID, this.User,
                this.requestStrategy,
                true, (
                    from userOrGroup in this.authorization.GetAuthorizedTo(SecurableActions.View(album))
                    where !(userOrGroup is AnonymousMember)
                    select new UserGroupViewModel(userOrGroup)
                ).ToList()        
            );
        }
    }
}

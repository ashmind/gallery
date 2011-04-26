using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

using AshMind.Gallery.Core;
using AshMind.Gallery.Site.Models;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core.Commenting;
using AshMind.Gallery.Site.Logic;

namespace AshMind.Gallery.Site.Controllers {
    [Authorize]
    public class GalleryController : ControllerBase {
        private readonly IAlbumFacade gallery;
        private readonly IAuthorizationService authorization;
        private readonly ICommentRepository commentRepository;
        private readonly IImageRequestStrategy requestStrategy;

        public GalleryController(
            IAlbumFacade gallery,
            IAuthorizationService authorization,
            ICommentRepository commentRepository,
            IUserAuthentication authentication,
            IImageRequestStrategy requestStrategy
        ) : base(authentication) {
            this.gallery = gallery;
            this.authorization = authorization;
            this.commentRepository = commentRepository;
            this.requestStrategy = requestStrategy;
        }

        public ActionResult Home(string album, int albumCount = 20) {
            var user = this.User;
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

        private ActionResult AjaxAlbum(string albumID, IUser user) {
            var album = this.gallery.GetAlbum(albumID, user) ?? Album.Empty;
            return PartialView("Album", ToViewModel(album, true));
        }

        public ActionResult StandardAlbumNames(int start, int count) {
            var albums = GetStandardAlbums(this.User)
                                     .Skip(start - 1).Take(count)
                                     .ToArray();

            return PartialView(new GalleryViewModel(
                null,
                new AlbumViewModel[0],
                new AlbumListViewModel(
                    albums,
                    start,
                    count > 0 ? (int?)albums[0].Album.Date.Year : null
                ),
                null
            ));
        }

        private IEnumerable<AlbumViewModel> GetStandardAlbums(IUser user) {
            return this.gallery.GetAlbums(AlbumProviderKeys.Default, user)
                               .OrderByDescending(a => a.Date)
                               .Select(a => ToViewModel(a, false));
        }

        private Tuple<IList<AlbumViewModel>, AlbumViewModel> GetPeopleAlbumsAndCurrentUserAlbum(IUser user) {
            var personAlbums = this.gallery.GetAlbums(AlbumProviderKeys.People, user)
                                           .OrderBy(p => p.Name)
                                           .Select(a => ToViewModel(a, false))
                                           .ToList();

            var realUser = user as User;
            var indexOfUserAlbum = realUser != null
                                 ? personAlbums.FindIndex(m => m.Album.Descriptor.ProviderSpecificPath == realUser.Email)
                                 : -1;
            var userAlbum = (AlbumViewModel)null;
            if (indexOfUserAlbum >= 0) {
                userAlbum = personAlbums[indexOfUserAlbum];
                personAlbums.RemoveAt(indexOfUserAlbum);
            }

            return new Tuple<IList<AlbumViewModel>, AlbumViewModel>(personAlbums, userAlbum);
        }

        public new ActionResult View(string album, string item) {
            var albumItem = this.gallery.GetItem(album, item, this.User);
            return View(
                new ItemDetailsViewModel(album, albumItem, this.User)
            );
        }

        public ActionResult Comment(string album, string item, string comment) {
            var author = (User)this.User;
            var path = this.gallery.GetItem(album, item, author).File.Path;
            commentRepository.SaveComment(
                path, new Comment(author, DateTimeOffset.Now, comment)
            );

            return RedirectToAction("View", new { album, item });
        }

        public ActionResult ProposeDelete(string album, string item) {
            return ToggleDelete(album, item, albumItem => albumItem.DeleteProposals.Add((User)User));
        }

        public ActionResult RevertDelete(string album, string item) {
            return ToggleDelete(album, item, albumItem => albumItem.DeleteProposals.Remove((User)User));
        }

        private ActionResult ToggleDelete(string albumID, string itemName, Action<AlbumItem> action) {
            if (!Request.IsAjaxRequest())
                throw new NotImplementedException();          

            var album = this.gallery.GetAlbum(albumID, User);
            if (album == null)
                return Unauthorized();

            var item = album.Items.Single(i => i.Name == itemName);
            action(item);

            this.gallery.SaveItem(item);

            return Content(item.DeleteProposals.Count.ToString(), MediaTypeNames.Text.Plain);
        }

        private AlbumViewModel ToViewModel(Album album, bool manageSecurity) {
            if (album == null)
                return null;

            var id = this.gallery.GetAlbumID(album);
            if (!manageSecurity || !authorization.IsAuthorized(this.User, SecurableAction.ManageSecurity, null))
                return new AlbumViewModel(album, this.gallery.GetAlbumID, this.User, this.requestStrategy, false, null);

            return new AlbumViewModel(
                album, this.gallery.GetAlbumID, this.User,
                this.requestStrategy,
                true, (
                    from @group in this.authorization.GetAuthorizedTo(SecurableAction.View, album.SecurableToken)
                    select new UserGroupViewModel(@group)
                ).ToList()        
            );
        }
    }
}

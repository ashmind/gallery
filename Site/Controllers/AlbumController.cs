﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;

using AshMind.Gallery.Core;
using AshMind.Gallery.Core.Albums;
using AshMind.Gallery.Core.Security.Actions;
using AshMind.Gallery.Site.Models;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Site.Logic;
using AshMind.IO.Abstraction;
using Ionic.Zip;

namespace AshMind.Gallery.Site.Controllers {
    [Authorize]
    public class AlbumController : ControllerBase {
        private readonly IAlbumFacade gallery;
        private readonly IAuthorizationService authorization;
        private readonly IImageRequestStrategy requestStrategy;
        private readonly ILocation zipLocation;

        public AlbumController(
            IAlbumFacade gallery,
            IAuthorizationService authorization,
            IUserAuthentication authentication,
            IImageRequestStrategy requestStrategy,
            ILocation dataLocation
        ) : base(authentication) {
            this.gallery = gallery;
            this.authorization = authorization;
            this.requestStrategy = requestStrategy;
            this.zipLocation = dataLocation.GetLocation("zips", ActionIfMissing.CreateNew);
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

            if (selected != null) {
                this.SetupSecurityManagement(selected);
                if (selected.Album.ViewedBy.Add(user))
                    this.gallery.SaveAlbum(selected.Album);
            }

            return View(new GalleryViewModel(
                currentUserAlbum,
                peopleAlbums,
                new AlbumListViewModel(standardAlbums, 0, null),
                selected
            ));
        }

        private ActionResult AjaxAlbum(string albumID, IUser user) {
            var album = this.gallery.GetAlbum(albumID, user);
            
            if (album != null) {
                if (album.ViewedBy.Add(user))
                    this.gallery.SaveAlbum(album);
            }
            else {
                album = Album.Empty;
            }
            var albumModel = ToViewModel(album);
            this.SetupSecurityManagement(albumModel);
            
            return PartialView("_View", albumModel);
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

        public ActionResult Download(string albumID) {
            var album = this.gallery.GetAlbum(albumID, this.User) ?? Album.Empty;
            if (album.Items.Value.Count == 0)
                return Empty();

            var zipFile = this.zipLocation.GetFile(album.Name + ".zip", ActionIfMissing.ReturnAsIs);
            using (var zip = new ZipFile()) {
                foreach (var item in album.Items.Value) {
                    if (item.IsProposedToBeDeleted)
                        continue;

                    zip.AddFile(item.File.Path, "");
                }
                
                zip.Save(zipFile.Path);
            }

            return File(
                zipFile.Open(FileLockMode.Write, FileOpenMode.ReadOrWrite),
                MediaTypeNames.Application.Zip,
                zipFile.Name
            );
        }

        private IEnumerable<AlbumViewModel> GetAllStandard(IUser user) {
            return this.gallery.GetAlbums(AlbumProviderKeys.Default, user)
                               .OrderByDescending(a => a.Date)
                               .Select(ToViewModel);
        }

        private Tuple<IList<AlbumViewModel>, AlbumViewModel> GetPeopleAlbumsAndCurrentUserAlbum(IUser user) {
            var personAlbums = this.gallery.GetAlbums(AlbumProviderKeys.People, user)
                                           .OrderBy(p => p.Name)
                                           .Select(ToViewModel)
                                           .ToList();

            var realUser = user as KnownUser;
            var indexOfUserAlbum = realUser != null
                                 ? personAlbums.FindIndex(m => ((PersonAlbum)m.Album).IsOf(realUser))
                                 : -1;

            var userAlbum = (AlbumViewModel)null;
            if (indexOfUserAlbum >= 0) {
                userAlbum = personAlbums[indexOfUserAlbum];
                personAlbums.RemoveAt(indexOfUserAlbum);
            }

            return new Tuple<IList<AlbumViewModel>, AlbumViewModel>(personAlbums, userAlbum);
        }

        private AlbumViewModel ToViewModel(Album album) {
            if (album == null)
                return null;
            
            return new AlbumViewModel(album, this.gallery.GetAlbumID, this.User, this.requestStrategy);
        }

        private void SetupSecurityManagement(AlbumViewModel model) {
            if (!authorization.IsAuthorized(this.User, SecurableActions.ManageSecurity))
                return;

            model.SetupSecurityManagement(
                from userOrGroup in this.authorization.GetAuthorizedTo(SecurableActions.View(model.Album))
                where !(userOrGroup is AnonymousMember)
                select new UserGroupViewModel(userOrGroup)
            );
        }
    }
}

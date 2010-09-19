using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.AlbumSupport;
using AshMind.Web.Gallery.Core.Commenting;
using AshMind.Web.Gallery.Core.ImageProcessing;
using AshMind.Web.Gallery.Core.Internal;
using AshMind.Web.Gallery.Core.IO;
using AshMind.Web.Gallery.Core.Metadata;
using AshMind.Web.Gallery.Core.Security;

namespace AshMind.Web.Gallery.Core {
    public class AlbumFacade {
        private readonly IFileSystem fileSystem;
        private readonly IAlbumIDProvider idProvider;
        private readonly AuthorizationService authorization;
        private readonly ICommentRepository commentRepository;
        private readonly IAlbumFilter[] albumFilters;

        internal string RootPath { private set; get; }

        internal AlbumFacade(
            string rootPath,       
            IFileSystem fileSystem,
            IAlbumIDProvider idProvider,
            AuthorizationService authorization,
            ICommentRepository commentRepository,
            IAlbumFilter[] albumFilters
        ) {
            this.RootPath = rootPath;

            this.fileSystem = fileSystem;
            this.idProvider = idProvider;
            this.authorization = authorization;
            this.commentRepository = commentRepository;
            this.albumFilters = albumFilters;
        }

        public IEnumerable<Album> GetAlbums(User user) {
            var locations = this.fileSystem.GetLocations(this.RootPath);
            return from location in locations
                   where !this.albumFilters.Any(a => a.ShouldSkip(location))
                   let album = GetAlbumAtLocation(location, user)
                   where album != null
                   orderby album.Date descending
                   select album;
        }

        private Album GetAlbumAtLocation(ILocation location, User user) {
            if (!authorization.IsAuthorized(user, SecurableAction.View, location.Path))
                return null;

            var items = this.GetItemsAtLocation(location, user).ToArray();
            if (items.Length == 0)
                return null;

            return new Album(location, items) {
                ID   = idProvider.GetAlbumID(location),
                Name = location.Name,
                Date = items.Min(item => item.Date)
            };
        }
        
        private IEnumerable<AlbumItem> GetItemsAtLocation(ILocation location, User user) {
            return from file in location.GetFiles()
                   let itemType = GuessItemType.Of(file.Name)
                   where itemType == GalleryItemType.Image
                   let item = GetItem(file, itemType)
                   orderby item.Date
                   select item;
        }

        public AlbumItem GetItem(string albumID, string itemName, User user) {
            var file = GetItemFile(albumID, itemName);
            var albumLocation = this.idProvider.GetAlbumLocation(albumID);

            if (!this.authorization.IsAuthorized(user, SecurableAction.View, albumLocation.Path))
                throw new UnauthorizedAccessException();

            var type = GuessItemType.Of(file.Name);
            if (type != GalleryItemType.Image)
                throw new NotSupportedException("Item does not have a known type.");

            return GetItem(file, type);
        }

        private AlbumItem GetItem(IFile file, GalleryItemType itemType) {
            return new AlbumItem(
                file.Name,
                itemType,
                file.GetLastWriteTime(),
                () => this.commentRepository.LoadCommentsOf(file.Path)
            );
        }

        public IFile GetItemFile(string albumID, string itemName) {
            if (!this.fileSystem.IsFileName(itemName))
                throw new InvalidOperationException(itemName + " is not a valid item name.");

            var location = this.idProvider.GetAlbumLocation(albumID);
            return location.GetFile(itemName);
        }

        public Album GetAlbum(string albumID, User user) {
            var location = this.idProvider.GetAlbumLocation(albumID);
            return this.GetAlbumAtLocation(location, user);
        }

        public string GetAlbumToken(string albumID) {
            return this.idProvider.GetAlbumLocation(albumID).Path;
        }
    }
}
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
        private readonly PreviewFacade preview;
        private readonly IAlbumIDProvider idProvider;
        private readonly AuthorizationService authorization;
        private readonly ICommentRepository commentRepository;
        private readonly IAlbumFilter[] albumFilters;

        internal string RootPath { private set; get; }

        internal AlbumFacade(
            string rootPath,
            PreviewFacade preview,            
            IFileSystem fileSystem,
            IAlbumIDProvider idProvider,
            AuthorizationService authorization,
            ICommentRepository commentRepository,
            IAlbumFilter[] albumFilters
        ) {
            this.RootPath = rootPath;

            this.fileSystem = fileSystem;
            this.preview = preview;
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

        private Album GetAlbumAtLocation(string location, User user) {
            if (!authorization.IsAuthorized(user, SecurableAction.View, location))
                return null;

            var items = this.GetItemsAtLocation(location, user).ToArray();
            if (items.Length == 0)
                return null;

            return new Album(items) {
                ID   = idProvider.GetAlbumID(location),
                Name = this.fileSystem.GetFileName(location),
                Date = items.Min(item => item.Date)
            };
        }
        
        private IEnumerable<GalleryItem> GetItemsAtLocation(string location, User user) {
            return from file in this.fileSystem.GetFileNames(location)
                   let itemType = GuessItemType.Of(file)
                   where itemType != GalleryItemType.Unknown
                   let item = GetItem(file, itemType)
                   orderby item.Date
                   select item;
        }

        public GalleryItem GetItem(string albumID, string itemName, User user) {
            var path = GetFullPath(albumID, itemName);
            var albumLocation = this.idProvider.GetAlbumLocation(albumID);

            if (!this.authorization.IsAuthorized(user, SecurableAction.View, albumLocation))
                throw new UnauthorizedAccessException();

            var type = GuessItemType.Of(path);
            if (type == GalleryItemType.Unknown)
                throw new NotSupportedException("Item does not have a known type.");

            return GetItem(path, type);
        }

        private GalleryItem GetItem(string filePath, GalleryItemType itemType) {            
            return new GalleryItem(
                this.fileSystem.GetFileName(filePath),
                itemType,
                this.fileSystem.GetCreationTime(filePath),
                size => this.preview.GetPreviewMetadata(filePath, size),
                () => this.commentRepository.LoadCommentsOf(filePath)
            );
        }

        public string GetFullPath(string albumID, string itemName) {
            if (!this.fileSystem.IsFileName(itemName))
                throw new InvalidOperationException(itemName + " is not a valid item name.");

            var path = this.idProvider.GetAlbumLocation(albumID);
            return fileSystem.BuildPath(path, itemName);
        }

        public Album GetAlbum(string albumID, User user) {
            var location = this.idProvider.GetAlbumLocation(albumID);
            return this.GetAlbumAtLocation(location, user);
        }

        public string GetAlbumToken(string albumID) {
            return this.idProvider.GetAlbumLocation(albumID);
        }
    }
}
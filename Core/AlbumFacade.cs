using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.AlbumSupport;
using AshMind.Web.Gallery.Core.ImageProcessing;
using AshMind.Web.Gallery.Core.Internal;
using AshMind.Web.Gallery.Core.IO;
using AshMind.Web.Gallery.Core.Metadata;
using AshMind.Web.Gallery.Core.Security;

namespace AshMind.Web.Gallery.Core
{
    public class AlbumFacade {
        private readonly IFileSystem fileSystem;
        private readonly PreviewFacade preview;
        private readonly IAlbumIDProvider idProvider;
        private readonly AuthorizationService authorization;
        private readonly IAlbumFilter[] albumFilters;
        private readonly ITagProvider[] tagProviders;

        internal string RootPath { private set; get; }

        internal AlbumFacade(
            string rootPath,
            PreviewFacade preview,
            IFileSystem fileSystem,
            IAlbumIDProvider idProvider,
            AuthorizationService authorization,
            IAlbumFilter[] albumFilters,
            ITagProvider[] tagProviders
        ) {
            this.RootPath = rootPath;

            this.fileSystem = fileSystem;
            this.preview = preview;
            this.idProvider = idProvider;
            this.authorization = authorization;
            this.albumFilters = albumFilters;
            this.tagProviders = tagProviders;
        }

        public IEnumerable<GalleryAlbum> GetAlbums(User user) {
            var locations = this.fileSystem.GetLocations(this.RootPath);
            return from location in locations
                   where !this.albumFilters.Any(a => a.ShouldSkip(location))
                   let album = GetAlbumAtLocation(location, user)
                   where album != null
                   orderby album.Date descending
                   select album;
        }

        private GalleryAlbum GetAlbumAtLocation(string location, User user) {
            var tags = this.tagProviders.SelectMany(p => p.GetTags(location)).ToArray();
            if (!authorization.IsAuthorized(user, SecurableAction.View, tags))
                return null;

            var items = this.GetItemsAtLocation(location, user).ToArray();
            if (items.Length == 0)
                return null;

            return new GalleryAlbum(items, tags) {
                ID   = idProvider.GetAlbumID(location),
                Name = this.fileSystem.GetFileName(location),
                Date = items.Min(item => item.Date)
            };
        }
        
        private IEnumerable<GalleryItem> GetItemsAtLocation(string location, User user) {
            return from file in this.fileSystem.GetFileNames(location)
                   let tags = this.tagProviders.SelectMany(p => p.GetTags(location)).ToArray()
                   where this.authorization.IsAuthorized(user, SecurableAction.View, tags)
                   let itemType = GuessItemType.Of(file)
                   where itemType != GalleryItemType.Unknown
                   let date = this.fileSystem.GetCreationTime(file)
                   orderby date
                   select new GalleryItem(
                       this.fileSystem.GetFileName(file),
                       itemType,
                       date,
                       size => this.preview.GetPreviewMetadata(file, size)
                   );
        }

        public string GetFullPath(string albumID, string itemName) {
            if (!this.fileSystem.IsFileName(itemName))
                throw new InvalidOperationException(itemName + " is not a valid item name.");

            var path = this.idProvider.GetAlbumLocation(albumID);
            return fileSystem.BuildPath(path, itemName);
        }

        public GalleryAlbum GetAlbum(string albumID, User user) {
            var location = this.idProvider.GetAlbumLocation(albumID);
            return this.GetAlbumAtLocation(location, user);
        }
    }
}
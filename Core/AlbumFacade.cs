using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.AlbumSupport;
using AshMind.Web.Gallery.Core.Internal;
using AshMind.Web.Gallery.Core.IO;
using AshMind.Web.Gallery.Core.Metadata;
using AshMind.Web.Gallery.Core.Security;

namespace AshMind.Web.Gallery.Core
{
    public class AlbumFacade {
        private readonly IFileSystem fileSystem;
        private readonly IAlbumIDProvider idProvider;
        private readonly AuthorizationService authorization;
        private readonly ITagProvider[] tagProviders;

        internal string RootPath { private set; get; }

        internal AlbumFacade(
            string rootPath,
            IFileSystem fileSystem,
            IAlbumIDProvider idProvider,
            AuthorizationService authorization,
            ITagProvider[] tagProviders
        ) {
            this.RootPath = rootPath;

            this.fileSystem = fileSystem;
            this.idProvider = idProvider;
            this.authorization = authorization;
            this.tagProviders = tagProviders;
        }

        public IEnumerable<GalleryAlbum> GetAlbums(User user) {
            var locations = this.fileSystem.GetLocations(this.RootPath);
            return from location in locations
                   let tags = this.tagProviders.SelectMany(p => p.GetTags(location)).ToArray()
                   where this.authorization.IsAuthorized(user, SecurableAction.View, tags)
                   let items = this.GetItemsAtLocation(location, user).ToArray()
                   where items.Count() > 0
                   let date = items.Min(item => item.Date)
                   orderby date descending
                   select new GalleryAlbum(items, tags) {
                       ID        = idProvider.GetAlbumID(location),
                       Name      = this.fileSystem.GetFileName(location),
                       Date      = date
                   };
        }
        
        private IEnumerable<GalleryItem> GetItemsAtLocation(string location, User user) {
            return from file in this.fileSystem.GetFileNames(location)
                   let tags = this.tagProviders.SelectMany(p => p.GetTags(location)).ToArray()
                   where this.authorization.IsAuthorized(user, SecurableAction.View, tags)
                   let itemType = GuessItemType.Of(file)
                   where itemType != GalleryItemType.Unknown
                   select new GalleryItem {
                       Name = this.fileSystem.GetFileName(file),
                       Date = this.fileSystem.GetCreationTime(file),
                       Type = itemType
                   };
        }

        public string GetFullPath(string albumID, string itemName) {
            if (!this.fileSystem.IsFileName(itemName))
                throw new InvalidOperationException(itemName + " is not a valid item name.");

            var path = this.idProvider.GetAlbumLocation(albumID);
            return fileSystem.BuildPath(path, itemName);
        }
    }
}
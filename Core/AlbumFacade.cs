using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;
using AshMind.Web.Gallery.Core.AlbumSupport;
using AshMind.Web.Gallery.Core.Internal;

namespace AshMind.Web.Gallery.Core
{
    public class AlbumFacade {
        private readonly IFileSystem fileSystem;
        private readonly IAlbumIDProvider idProvider;

        internal string RootPath { private set; get; }

        internal AlbumFacade(string rootPath, IFileSystem fileSystem, IAlbumIDProvider idProvider) {
            this.RootPath = rootPath;

            this.fileSystem = fileSystem;
            this.idProvider = idProvider;
        }

        public IEnumerable<GalleryAlbum> GetAlbums() {
            var locations = this.fileSystem.GetLocations(this.RootPath);
            return from location in locations
                   let items = this.GetItemsAtLocation(location).ToArray()
                   where items.Count() > 0
                   let date = this.fileSystem.GetCreationTime(location)
                   orderby date descending
                   select new GalleryAlbum(items) {
                       ID        = idProvider.GetAlbumID(location),
                       Name      = this.fileSystem.GetFileName(location),
                       Date      = date
                   };
        }
        
        private IEnumerable<GalleryItem> GetItemsAtLocation(string location) {
            return from file in this.fileSystem.GetFileNames(location)
                   let itemType = GuessItemType.Of(file)
                   where itemType != GalleryItemType.Unknown
                   select new GalleryItem {
                       Name = this.fileSystem.GetFileName(file),
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
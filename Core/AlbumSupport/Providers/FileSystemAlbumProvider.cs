using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;
using AshMind.Web.Gallery.Core.Security;

namespace AshMind.Web.Gallery.Core.AlbumSupport.Providers {
    public class FileSystemAlbumProvider : IAlbumProvider {
        private readonly IFileSystem fileSystem;
        private readonly AlbumItemFactory itemFactory;
        private readonly AuthorizationService authorization;

        public FileSystemAlbumProvider(
            IFileSystem fileSystem,
            AlbumItemFactory itemFactory,
            AuthorizationService authorization
        ) {
            this.fileSystem = fileSystem;
            this.itemFactory = itemFactory;
            this.authorization = authorization;
        }

        public IEnumerable<Album> GetAllAlbums(IEnumerable<ILocation> locations, User user) {
            return from location in locations
                   let album = GetAlbumAtLocation(location, user)
                   where album != null
                   orderby album.Date descending
                   select album;
        }

        public Album GetAlbum(IEnumerable<ILocation> locations, string providerSpecificPath, User user) {
            return GetAlbumAtLocation(this.fileSystem.GetLocation(providerSpecificPath), user);
        }

        private Album GetAlbumAtLocation(ILocation location, User user) {
            if (!authorization.IsAuthorized(user, SecurableAction.View, location))
                return null;

            var items = this.GetItemsAtLocation(location, user).ToArray();
            if (items.Length == 0)
                return null;

            return new Album(
                new AlbumDescriptor(this.ProviderKey, location.Path),
                location.Name, items,
                location
            );
        }

        private IEnumerable<AlbumItem> GetItemsAtLocation(ILocation location, User user) {
            return from file in location.GetFiles()
                   let itemType = GuessItemType.Of(file.Name)
                   where itemType == AlbumItemType.Image
                   let item = this.itemFactory.CreateFrom(file, itemType)
                   select item;
        }
        
        public string ProviderKey {
            get { return AlbumProviderKeys.Default; }
        }
    }
}

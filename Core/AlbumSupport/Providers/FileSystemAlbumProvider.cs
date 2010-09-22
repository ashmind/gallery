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
                   let album = GetAlbum(location, user)
                   where album != null
                   orderby album.Date descending
                   select album;
        }

        public Album GetAlbum(IEnumerable<ILocation> locations, string providerSpecificPath, User user) {
            return GetAlbum(this.fileSystem.GetLocation(providerSpecificPath), user);
        }

        public Album GetAlbum(ILocation location, User user, bool ensureNonEmpty = true) {
            if (!authorization.IsAuthorized(user, SecurableAction.View, location))
                return null;

            Func<AlbumItem[]> itemFactory = () => this.GetItemsAtLocation(location, user).ToArray();
            if (ensureNonEmpty) {
                var items = itemFactory();
                if (items.Length == 0)
                    return null;

                itemFactory = () => items;
            }

            return new Album(
                new AlbumDescriptor(this.ProviderKey, location.Path),
                location.Name, itemFactory,
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

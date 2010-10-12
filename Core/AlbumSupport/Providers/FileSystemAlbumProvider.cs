using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

using AshMind.Gallery.Core.IO;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core.Fixes;

namespace AshMind.Gallery.Core.AlbumSupport.Providers {
    public class FileSystemAlbumProvider : IAlbumProvider {
        private readonly IFileSystem fileSystem;
        private readonly AlbumItemFactory itemFactory;
        private readonly AuthorizationService authorization;
        private readonly ObjectCache cache;

        public FileSystemAlbumProvider(
            IFileSystem fileSystem,
            AlbumItemFactory itemFactory,
            AuthorizationService authorization,
            ObjectCache cache
        ) {
            this.fileSystem = fileSystem;
            this.itemFactory = itemFactory;
            this.authorization = authorization;
            this.cache = cache;
        }

        public IEnumerable<Album> GetAllAlbums(IEnumerable<ILocation> locations, IUser user) {
            return from location in locations
                   let album = GetAlbum(location, user)
                   where album != null
                   orderby album.Date descending
                   select album;
        }

        public Album GetAlbum(IEnumerable<ILocation> locations, string providerSpecificPath, IUser user) {
            return GetAlbum(this.fileSystem.GetLocation(providerSpecificPath), user);
        }

        public Album GetAlbum(ILocation location, IUser user, bool ensureNonEmpty = true) {
            if (!authorization.IsAuthorized(user, SecurableAction.View, location))
                return null;

            var cacheKey = "album:" + location.Path;
            var album = (Album)this.cache.Get(cacheKey);
            if (album != null)
                return album.AsWritable();

            Func<AlbumItem[]> itemFactory = () => this.GetItemsAtLocation(location, user).ToArray();
            if (ensureNonEmpty) {
                var items = itemFactory();
                if (items.Length == 0)
                    return null;

                itemFactory = () => items;
            }

            album = new Album(
                new AlbumDescriptor(this.ProviderKey, location.Path),
                location.Name, itemFactory,
                location
            );
            album.MakeReadOnly();
            this.cache.Set(cacheKey, album, new CacheItemPolicy {
                ChangeMonitors = { new FixedFileChangeMonitor(new[] { location.Path }) }
            });

            return album.AsWritable();
        }

        private IEnumerable<AlbumItem> GetItemsAtLocation(ILocation location, IUser user) {
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

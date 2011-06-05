using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

using AshMind.IO.Abstraction;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core.Fixes;
using AshMind.Gallery.Core.Security.Actions;

namespace AshMind.Gallery.Core.AlbumSupport.Providers {
    public class FileSystemAlbumProvider : IAlbumProvider {
        private readonly IFileSystem fileSystem;
        private readonly AlbumFactory albumFactory;
        private readonly AlbumItemFactory itemFactory;
        private readonly IAuthorizationService authorization;
        private readonly ObjectCache cache;

        public FileSystemAlbumProvider(
            IFileSystem fileSystem,
            AlbumFactory albumFactory,
            AlbumItemFactory itemFactory,
            IAuthorizationService authorization,
            ObjectCache cache
        ) {
            this.fileSystem = fileSystem;
            this.albumFactory = albumFactory;
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
            if (!authorization.IsAuthorized(user, SecurableActions.View(location)))
                return null;

            var cacheKey = "album:" + location.Path;
            var album = (Album)this.cache.Get(cacheKey);
            if (album != null)
                return album.AsWritable();

            Func<AlbumItem[]> itemsFactory = () => this.GetItemsAtLocation(location).ToArray();
            if (ensureNonEmpty) {
                var items = itemsFactory();
                if (items.Length == 0)
                    return null;

                itemsFactory = () => items;
            }

            album = this.albumFactory.Create(
                new AlbumDescriptor(this.ProviderKey, location.Path),
                location.Name, itemsFactory,
                location
            );
            album.MakeReadOnly();
            this.cache.Set(cacheKey, album, new CacheItemPolicy {
                ChangeMonitors = { new FixedFileChangeMonitor(new[] { location.Path }) }
            });

            return album.AsWritable();
        }

        private IEnumerable<AlbumItem> GetItemsAtLocation(ILocation location) {
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using AshMind.Extensions;
using AshMind.Gallery.Core.Albums;
using AshMind.Gallery.Core.Values;
using AshMind.IO.Abstraction;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core.Fixes;
using AshMind.Gallery.Core.Security.Actions;

namespace AshMind.Gallery.Core.AlbumSupport.Providers {
    public class FileSystemAlbumProvider : IAlbumProvider {
        private readonly IFileSystem fileSystem;
        private readonly AlbumFactory<FileSystemAlbum> albumFactory;
        private readonly AlbumItemFactory itemFactory;
        private readonly IAuthorizationService authorization;
        private readonly ObjectCache cache;

        public FileSystemAlbumProvider(
            IFileSystem fileSystem,
            AlbumFactory<FileSystemAlbum> albumFactory,
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
            var album = GetReadOnlyAlbum(location, ensureNonEmpty);
            if (album == null)
                return null;

            if (!authorization.IsAuthorized(user, SecurableActions.View(album)))
                return null;

            var writable = album.AsWritable();
            writable.Items = writable.Items.Change(
                items => items.RemoveWhere(i => !authorization.IsAuthorized(user, SecurableActions.View(i)))
            );

            return writable;
        }

        private Album GetReadOnlyAlbum(ILocation location, bool ensureNonEmpty = true) {
            var cacheKey = "album:" + location.Path;
            var album = (Album)this.cache.Get(cacheKey);
            if (album != null)
                return album;

            var itemsValue = To.Lazy(() => this.GetItemsAtLocation(location));
            if (ensureNonEmpty) {
                var items = itemsValue.Value.ToArray();
                if (items.Length == 0)
                    return null;

                itemsValue = To.Just(items);
            }

            var descriptor = new AlbumDescriptor(this.ProviderKey, location.Path);
            album = this.albumFactory.Process(new FileSystemAlbum(
                descriptor, this.albumFactory.GetAlbumName(location.Name, descriptor), location, itemsValue
            ));
            album.MakeReadOnly();
            this.cache.Set(cacheKey, album, new CacheItemPolicy {
                ChangeMonitors = { new FixedFileChangeMonitor(new[] { location.Path }) }
            });

            return album;
        }

        private IEnumerable<AlbumItem> GetItemsAtLocation(ILocation location) {
            return from file in location.GetFiles()
                   let itemType = GuessItemType.Of(file.Name)
                   where itemType == AlbumItemType.Image
                   select this.itemFactory.CreateFrom(file, itemType);
        }
        
        public string ProviderKey {
            get { return AlbumProviderKeys.Default; }
        }
    }
}

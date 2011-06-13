using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

using AshMind.IO.Abstraction;

using AshMind.Gallery.Core.Metadata;
using AshMind.Gallery.Core.Values;
using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Core.AlbumSupport {
    public class AlbumFacade : IAlbumFacade {
        private readonly IAlbumIDProvider idProvider;
        private readonly IDictionary<string, IAlbumProvider> albumProviders;
        private readonly IAlbumFilter[] albumFilters;
        private readonly IMetadataStore<AlbumItem>[] metadataStores;

        internal ILocation Root { private set; get; }

        public AlbumFacade(
            ILocation root,
            IAlbumIDProvider idProvider,
            IEnumerable<IAlbumProvider> albumProviders,
            IAlbumFilter[] albumFilters,
            IMetadataStore<AlbumItem>[] metadataStores
        ) {
            this.Root = root;

            this.idProvider = idProvider;
            this.albumProviders = albumProviders.ToDictionary(p => p.ProviderKey);
            this.albumFilters = albumFilters;
            this.metadataStores = metadataStores;
        }

        public IEnumerable<Album> GetAlbums(string providerKey, IUser user) {
            Argument.VerifyNotNullOrEmpty("providerKey", providerKey);
            Argument.VerifyNotNull("user", user);

            var locations = GetAlbumLocations();
            return this.albumProviders[providerKey].GetAllAlbums(locations, user);
        }

        private IEnumerable<ILocation> GetAlbumLocations() {
            return this.Root.GetLocations(true)
                            .Where(location => !this.albumFilters.Any(a => a.ShouldSkip(location)));
        }

        public string GetAlbumID(Album album) {
            Argument.VerifyNotNull("album", album);

            return this.idProvider.GetAlbumID(album.Name, album.Descriptor);
        }

        public IValue<AlbumItem> GetItem(string albumID, string itemName, IUser user) {
            Argument.VerifyNotNullOrEmpty("itemName", itemName);

            var album = GetAlbum(albumID, user);
            return album.Items.FirstOrDefault(item => item.Name == itemName);
        }

        public Album GetAlbum(string albumID, IUser user) {
            Argument.VerifyNotNullOrEmpty("albumID", albumID);
            Argument.VerifyNotNull("user", user);

            var descriptor = this.idProvider.GetAlbumDescriptor(albumID);
            var provider = this.albumProviders[descriptor.ProviderKey];

            return provider.GetAlbum(GetAlbumLocations(), descriptor.ProviderSpecificPath, user);
        }

        public void SaveItem(AlbumItem item) {
            Argument.VerifyNotNull("item", item);

            this.metadataStores.ForEach(p => p.SaveMetadata(item));
        }
    }
}
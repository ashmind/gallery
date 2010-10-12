using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using AshMind.Extensions;

using AshMind.Gallery.Core.AlbumSupport;
using AshMind.Gallery.Core.Commenting;
using AshMind.Gallery.Core.ImageProcessing;
using AshMind.Gallery.Core.Internal;
using AshMind.Gallery.Core.IO;
using AshMind.Gallery.Core.Metadata;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core.Integration;

namespace AshMind.Gallery.Core {
    public class AlbumFacade {
        private readonly IAlbumIDProvider idProvider;
        private readonly IDictionary<string, IAlbumProvider> albumProviders;
        private readonly IAlbumFilter[] albumFilters;
        private readonly IAlbumItemMetadataProvider[] metadataProviders;

        internal ILocation Root { private set; get; }

        public AlbumFacade(
            ILocation root,
            IAlbumIDProvider idProvider,
            IAlbumProvider[] albumProviders,
            IAlbumFilter[] albumFilters,
            IAlbumItemMetadataProvider[] metadataProviders
        ) {
            this.Root = root;

            this.idProvider = idProvider;
            this.albumProviders = albumProviders.ToDictionary(p => p.ProviderKey);
            this.albumFilters = albumFilters;
            this.metadataProviders = metadataProviders;
        }

        public IEnumerable<Album> GetAlbums(string providerKey, IUser user) {
            var locations = GetAlbumLocations();
            return this.albumProviders[providerKey].GetAllAlbums(locations, user);
        }

        private IEnumerable<ILocation> GetAlbumLocations() {
            return this.Root.GetLocations(true)
                            .Where(location => !this.albumFilters.Any(a => a.ShouldSkip(location)));
        }

        public string GetAlbumID(Album album) {
            return this.idProvider.GetAlbumID(album.Name, album.Descriptor);
        }

        public AlbumItem GetItem(string albumID, string itemName, IUser user) {
            var album = GetAlbum(albumID, user);
            return album.Items.FirstOrDefault(item => item.Name == itemName);
        }

        public Album GetAlbum(string albumID, IUser user) {
            var descriptor = this.idProvider.GetAlbumDescriptor(albumID);
            var provider = this.albumProviders[descriptor.ProviderKey];

            return provider.GetAlbum(GetAlbumLocations(), descriptor.ProviderSpecificPath, user);
        }

        public void SaveItem(AlbumItem item) {
            this.metadataProviders.ForEach(p => p.SaveMetadata(item));
        }
    }
}
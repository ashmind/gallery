using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.AlbumSupport;
using AshMind.Web.Gallery.Core.Commenting;
using AshMind.Web.Gallery.Core.ImageProcessing;
using AshMind.Web.Gallery.Core.Internal;
using AshMind.Web.Gallery.Core.IO;
using AshMind.Web.Gallery.Core.Metadata;
using AshMind.Web.Gallery.Core.Security;
using AshMind.Web.Gallery.Core.Integration;

namespace AshMind.Web.Gallery.Core {
    public class AlbumFacade {
        private readonly IAlbumIDProvider idProvider;
        private readonly IDictionary<string, IAlbumProvider> albumProviders;
        private readonly IAlbumFilter[] albumFilters;

        internal ILocation Root { private set; get; }

        internal AlbumFacade(
            ILocation root,
            IAlbumIDProvider idProvider,
            IAlbumProvider[] albumProviders,
            IAlbumFilter[] albumFilters
        ) {
            this.Root = root;

            this.idProvider = idProvider;
            this.albumProviders = albumProviders.ToDictionary(p => p.ProviderKey);
            this.albumFilters = albumFilters;
        }

        public IEnumerable<Album> GetAlbums(string providerKey, User user) {
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

        public AlbumItem GetItem(string albumID, string itemName, User user) {
            var album = GetAlbum(albumID, user);
            return album.Items.FirstOrDefault(item => item.Name == itemName);
        }

        public Album GetAlbum(string albumID, User user) {
            var descriptor = this.idProvider.GetAlbumDescriptor(albumID);
            var provider = this.albumProviders[descriptor.ProviderKey];

            return provider.GetAlbum(GetAlbumLocations(), descriptor.ProviderSpecificPath, user);
        }

        public string GetAlbumToken(string albumID) {
            return this.idProvider.GetAlbumDescriptor(albumID).ProviderSpecificPath;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

using AshMind.Web.Gallery.Core.Integration;
using AshMind.Web.Gallery.Core.IO;
using AshMind.Web.Gallery.Core.Security;

namespace AshMind.Web.Gallery.Core.AlbumSupport.Providers {
    public class PersonAlbumProvider : IAlbumProvider {
        private readonly AlbumItemFactory itemFactory;
        private readonly IFaceProvider[] faceProviders;
        private readonly AuthorizationService authorization;
        private readonly ObjectCache faceCache;

        public PersonAlbumProvider(
            AlbumItemFactory itemFactory,
            IFaceProvider[] faceProviders,
            AuthorizationService authorization,
            ObjectCache faceCache
        ) {
            this.itemFactory = itemFactory;
            this.faceProviders = faceProviders;
            this.authorization = authorization;
            this.faceCache = faceCache;
        }

        public IEnumerable<Album> GetAllAlbums(IEnumerable<ILocation> locations, User user) {
            var albumsCacheKey = "faces:all_albums";
            var cached = this.faceCache.Get(albumsCacheKey);
            if (cached != null)
                return (Album[])cached;

            var albums = (
                from location in locations
                from face in GetFaces(location)
                group face by face.Person into personFaces
                let uniqueKey = personFaces.Key.Email ?? personFaces.Key.Name
                select new Album(
                    new AlbumDescriptor(this.ProviderKey, uniqueKey),
                    personFaces.Key.Name,
                    (
                        from face in personFaces
                        let itemType = GuessItemType.Of(face.File.Name)
                        where itemType == AlbumItemType.Image
                        select this.itemFactory.CreateFrom(face.File, itemType)
                    ).ToList(),
                    new SecurableUniqueKey(uniqueKey)
                )
            ).ToArray();

            this.faceCache.Set(albumsCacheKey, albums, new CacheItemPolicy {
                AbsoluteExpiration = DateTimeOffset.Now.AddDays(3)
            });

            return FilterByAuthorization(albums, user);
        }

        public Album GetAlbum(IEnumerable<ILocation> locations, string providerSpecificPath, User user) {
            return this.GetAllAlbums(locations, user).Single(a => a.Descriptor.ProviderSpecificPath == providerSpecificPath);
        }

        private IEnumerable<Album> FilterByAuthorization(IEnumerable<Album> albums, User user) {
            return albums.Where(
                album => album.Descriptor.ProviderSpecificPath == user.Email
                      || IsAuthorizedTo(album, user)
            );
        }

        private bool IsAuthorizedTo(Album album, User user) {
            return this.authorization.IsAuthorized(
                user, SecurableAction.View,
                new SecurableUniqueKey(album.Descriptor.ProviderSpecificPath)
            );
        }

        private IEnumerable<Face> GetFaces(ILocation location) {
            var cacheKey = "faces:" + location.Path;
            var cached = this.faceCache.Get(cacheKey);
            if (cached != null)
                return (Face[])cached;

            var faces = (
                from provider in this.faceProviders
                from face in provider.GetFaces(location)
                select face
            ).ToArray();
            
            // https://connect.microsoft.com/VisualStudio/feedback/details/565313/breaking-problem-with-datetimeoffset-that-affects-multiple-framework-classes?wa=wsignin1.0
            // this.faceCache.Set(cacheKey, faces, new CacheItemPolicy {
            //    ChangeMonitors = { new HostFileChangeMonitor(new[] { location.Path }) }
            // });

            this.faceCache.Set(cacheKey, faces, new CacheItemPolicy {
                AbsoluteExpiration = DateTimeOffset.Now.AddDays(3)
            });

            return faces;
        }

        public string ProviderKey {
            get { return AlbumProviderKeys.People; }
        }
    }
}

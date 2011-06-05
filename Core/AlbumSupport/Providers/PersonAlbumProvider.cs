using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

using AshMind.Extensions;

using AshMind.Gallery.Core.Integration;
using AshMind.IO.Abstraction;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core.Security.Actions;

namespace AshMind.Gallery.Core.AlbumSupport.Providers {
    public class PersonAlbumProvider : IAlbumProvider {
        private readonly AlbumFactory albumFactory;
        private readonly AlbumItemFactory itemFactory;
        private readonly IFaceProvider[] faceProviders;
        private readonly FileSystemAlbumProvider primaryAlbumProvider;
        private readonly IAuthorizationService authorization;
        private readonly ObjectCache faceCache;

        public PersonAlbumProvider(
            AlbumFactory albumFactory,
            AlbumItemFactory itemFactory,
            FileSystemAlbumProvider primaryAlbumProvider,
            IFaceProvider[] faceProviders,
            IAuthorizationService authorization,
            ObjectCache faceCache
        ) {
            this.albumFactory = albumFactory;
            this.itemFactory = itemFactory;
            this.primaryAlbumProvider = primaryAlbumProvider;
            this.faceProviders = faceProviders;
            this.authorization = authorization;
            this.faceCache = faceCache;
        }

        public IEnumerable<Album> GetAllAlbums(IEnumerable<ILocation> locations, IUser user) {
            const string albumsCacheKey = "faces:all_albums";
            var cached = this.faceCache.Get(albumsCacheKey);
            if (cached != null)
                return CorrectAlbums((Album[])cached, user);

            var albums = (
                from location in locations
                from face in GetFaces(location)
                group face by face.Person into personFaces
                let uniqueKey = personFaces.Key.Emails.ElementAtOrDefault(0) ?? personFaces.Key.Name
                select this.albumFactory.Create(
                    new AlbumDescriptor(this.ProviderKey, uniqueKey),
                    personFaces.Key.Name,
                    () => (
                        from face in personFaces
                        let itemType = GuessItemType.Of(face.File.Name)
                        where itemType == AlbumItemType.Image
                        select CreateAlbumItem(face, itemType)
                    ).ToList(),
                    new SecurableUniqueKey(uniqueKey)
                )
            ).ToArray();

            albums.ForEach(a => a.MakeReadOnly());
            this.faceCache.Set(albumsCacheKey, albums, new CacheItemPolicy {
                AbsoluteExpiration = DateTimeOffset.Now.AddDays(3)
            });

            return CorrectAlbums(albums, user);
        }

        private AlbumItem CreateAlbumItem(Face face, AlbumItemType itemType) {
            var item = this.itemFactory.CreateFrom(face.File, itemType);
            item.LazyPrimaryAlbum = new Lazy<Album>(
                () => this.primaryAlbumProvider.GetAlbum(face.File.Location, KnownUser.System, ensureNonEmpty: false)
            );

            return item;
        }

        public Album GetAlbum(IEnumerable<ILocation> locations, string providerSpecificPath, IUser user) {
            return this.GetAllAlbums(locations, user).Single(a => a.Descriptor.ProviderSpecificPath == providerSpecificPath);
        }

        private IEnumerable<Album> CorrectAlbums(IEnumerable<Album> albums, IUser user) {
            return FilterByAuthorization(albums, user).Select(a => a.AsWritable());
        }

        private IEnumerable<Album> FilterByAuthorization(IEnumerable<Album> albums, IUser user) {
            var realUser = user as KnownUser;            
            return albums.Where(
                album => (realUser != null && album.Descriptor.ProviderSpecificPath == realUser.Email)
                      || IsAuthorizedTo(album, user)
            );
        }

        private bool IsAuthorizedTo(Album album, IUser user) {
            return this.authorization.IsAuthorized(
                user, SecurableActions.View(new SecurableUniqueKey(album.Descriptor.ProviderSpecificPath))
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

using AshMind.Extensions;
using AshMind.Gallery.Core.Albums;
using AshMind.Gallery.Integration.Faces;
using AshMind.IO.Abstraction;

using AshMind.Gallery.Core.Values;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core.Security.Actions;
using AshMind.Gallery.Integration;

namespace AshMind.Gallery.Core.AlbumSupport.Providers {
    public class PersonAlbumProvider : IAlbumProvider {
        private readonly AlbumFactory<PersonAlbum> albumFactory;
        private readonly AlbumItemFactory itemFactory;
        private readonly IFaceProvider[] faceProviders;
        private readonly FileSystemAlbumProvider primaryAlbumProvider;
        private readonly IAuthorizationService authorization;
        private readonly ObjectCache faceCache;

        public PersonAlbumProvider(
            AlbumFactory<PersonAlbum> albumFactory,
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
                return CorrectAlbums((PersonAlbum[])cached, user);

            var albums = (
                from location in locations
                from face in GetFaces(location)
                group face by face.Person into facesOfPerson
                let uniqueKey = facesOfPerson.Key.Emails.ElementAtOrDefault(0) ?? facesOfPerson.Key.Name
                let descriptor = new AlbumDescriptor(this.ProviderKey, uniqueKey)
                let name = this.albumFactory.GetAlbumName(facesOfPerson.Key.Name, descriptor)
                select this.albumFactory.Process(new PersonAlbum(
                    descriptor, name, facesOfPerson.Key,
                    To.Lazy(() => this.CreateAlbumItems(facesOfPerson, user))
                ))
            ).ToArray();

            albums.ForEach(a => a.MakeReadOnly());
            this.faceCache.Set(albumsCacheKey, albums, new CacheItemPolicy {
                AbsoluteExpiration = DateTimeOffset.Now.AddDays(3)
            });

            return CorrectAlbums(albums, user);
        }

        private IEnumerable<AlbumItem> CreateAlbumItems(IEnumerable<Face> faces, IUser user) {
            return from face in faces
                   let itemType = GuessItemType.Of(face.File.Name)
                   where itemType == AlbumItemType.Image
                   let item = CreateAlbumItem(face, itemType)
                   select item;
        }

        private AlbumItem CreateAlbumItem(Face face, AlbumItemType itemType) {
            var item = this.itemFactory.CreateFrom(face.File, itemType);
            item.PrimaryAlbum = To.Lazy(
                () => this.primaryAlbumProvider.GetAlbum(face.File.Location, KnownUser.System, ensureNonEmpty: false)
            );

            return item;
        }

        public Album GetAlbum(IEnumerable<ILocation> locations, string providerSpecificPath, IUser user) {
            return this.GetAllAlbums(locations, user).Single(a => a.Descriptor.ProviderSpecificPath == providerSpecificPath);
        }

        private IEnumerable<PersonAlbum> CorrectAlbums(IEnumerable<PersonAlbum> albums, IUser user) {
            var filtered = FilterByAuthorization(albums, user).Select(a => a.AsWritable());
            foreach (var album in filtered) {
                album.Items = album.Items.Change(
                    v => v.RemoveWhere(item => !this.authorization.IsAuthorized(user, SecurableActions.View(item)))
                );

                yield return album;
            }
        }

        private IEnumerable<PersonAlbum> FilterByAuthorization(IEnumerable<PersonAlbum> albums, IUser user) {
            var realUser = user as KnownUser;            
            return albums.Where(
                album => (realUser != null && album.IsOf(realUser))
                      || this.authorization.IsAuthorized(user, SecurableActions.View(album))
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

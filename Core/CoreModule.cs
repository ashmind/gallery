using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography;

using Autofac;
using Autofac.Builder;

using AshMind.Extensions;

using AshMind.Gallery.Core.AlbumSupport;
using AshMind.Gallery.Core.Commenting;
using AshMind.Gallery.Core.ImageProcessing;
using AshMind.Gallery.Core.Integration;
using AshMind.Gallery.Core.Integration.Picasa;
using AshMind.Gallery.Core.IO;
using AshMind.Gallery.Core.IO.Implementation;
using AshMind.Gallery.Core.Metadata;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core.Security.Internal;

namespace AshMind.Gallery.Core {
    public class CoreModule : Module {
        private readonly IFileSystem fileSystem;
        private readonly ILocation albumLocation;
        private readonly ILocation storageLocation;
        private readonly IFile picasaContactsXmlFile;

        private readonly Func<ObjectCache> cacheFactory;

        public CoreModule(
            string albumPath,
            string storagePath,
            string picasaContactsXmlPath,
            Func<ObjectCache> cacheFactory
        ) {
            this.fileSystem = new FileSystem();

            this.albumLocation = fileSystem.GetLocation(albumPath);
            this.storageLocation = fileSystem.GetLocation(storagePath);

            if (picasaContactsXmlPath != null)
                this.picasaContactsXmlFile = fileSystem.GetFile(picasaContactsXmlPath);

            this.cacheFactory = cacheFactory;
        }

        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);

            var types = this.GetType().Assembly.GetTypes();
            

            builder.Register(c => cacheFactory())
                   .As<ObjectCache>()
                   .FactoryScoped();

            builder.Register(c => new JsonSecurityRepository(
                        this.storageLocation.GetFile("users.jsdb", false)
                    ))
                   .As<IRepository<User>, IRepository<UserGroup>, IRepository<IUserGroup>>()
                   .SingletonScoped();

            builder.Register<JsonCommentRepository>()
                   .As<ICommentRepository>()
                   .SingletonScoped();

            builder.Register(this.fileSystem)
                   .As<IFileSystem>()
                   .SingletonScoped();

            var cacheRoot = this.storageLocation.GetLocation("images", ActionIfMissing.CreateNew);
            builder.Register(c => new ImageCache(cacheRoot, ImageCacheFormat.Jpeg, c.Resolve<ICacheDependencyProvider[]>()))
                   .SingletonScoped();

            RegisterSecurity(builder);

            RegisterAlbumSupport(builder, types);

            builder.Register<PreviewFacade>().SingletonScoped();
            
            RegisterPicasaIntegration(builder);
                        
            RegisterAllImplementationsOf<ILocationMetadataProvider>(builder, types, InstanceScope.Singleton);            
            RegisterAllImplementationsOf<IOrientationProvider>(builder, types,  InstanceScope.Singleton);
            RegisterAllImplementationsOf<ICacheDependencyProvider>(builder, types, InstanceScope.Singleton);            
        }

        private void RegisterSecurity(ContainerBuilder builder) {
            builder.Register<AuthorizationService>().As<IAuthorizationService>().SingletonScoped();
            builder.Register<JsonKeyPermissionProvider>()
                   .As<IPermissionProvider>()
                   .WithArguments(new TypedParameter(typeof(IFile), this.storageLocation.GetFile("permissions.jsdb", false)))
                   .SingletonScoped();

            builder.Register<JsonLocationPermissionProvider>()
                   .As<IPermissionProvider>()
                   .SingletonScoped();

            // because my current gallery has a lot of stuff with permissions set through this:
            builder.Register<ObsoleteJsonLocationPermissionProvider>()
                   .As<IPermissionProvider>()
                   .SingletonScoped();

            builder.Register<MD5UserGroupSecureReferenceStrategy>()
                   .As<IUserGroupSecureReferenceStrategy>()
                   .ContainerScoped();

            builder.Register(c => (Func<IUserGroupSecureReferenceStrategy>)(() => c.Resolve<IUserGroupSecureReferenceStrategy>()));
        }

        private void RegisterAlbumSupport(ContainerBuilder builder, IEnumerable<Type> types) {
            builder.Register(c => new AlbumIDProvider(this.storageLocation, this.fileSystem))
                   .As<IAlbumIDProvider>();

            builder.Register<AlbumFactory>();
            builder.Register<AlbumItemFactory>();
            
            builder.Register<AlbumFacade>()
                   .As<IAlbumFacade>()
                   .WithArguments(new TypedParameter(typeof(ILocation), this.albumLocation))
                   .SingletonScoped();

            RegisterAllImplementationsOf<IAlbumItemMetadataProvider>(builder, types, InstanceScope.Singleton);
            RegisterAllImplementationsOf<IAlbumProvider>(builder, types, InstanceScope.Singleton);
            RegisterAllImplementationsOf<IAlbumFilter>(builder, types, InstanceScope.Singleton);
        }

        private void RegisterPicasaIntegration(ContainerBuilder builder) {
            builder.Register<PicasaIniFileFinder>();
            builder.Register<PicasaIniParser>();

            if (this.picasaContactsXmlFile == null)
                return;

            builder.Register(c => new PicasaDatabase(this.picasaContactsXmlFile));

            builder.Register<PicasaFaceProvider>()
                   .As<IFaceProvider>()
                   .SingletonScoped();
        }

        private void RegisterAllImplementationsOf<TService>(ContainerBuilder builder, IEnumerable<Type> types, InstanceScope scope) {
            var service = typeof(TService);
            types.Where(type => type.GetInterfaces().Contains(service))
                 .ForEach(
                    type => builder.Register(type).As(service, type).WithScope(scope)
                 );
        }
    }
}

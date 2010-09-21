using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;

using Autofac;
using Autofac.Builder;

using AshMind.Extensions;

using AshMind.Web.Gallery.Core.AlbumSupport;
using AshMind.Web.Gallery.Core.Commenting;
using AshMind.Web.Gallery.Core.ImageProcessing;
using AshMind.Web.Gallery.Core.Integration;
using AshMind.Web.Gallery.Core.Integration.Picasa;
using AshMind.Web.Gallery.Core.IO;
using AshMind.Web.Gallery.Core.IO.Implementation;
using AshMind.Web.Gallery.Core.Metadata;
using AshMind.Web.Gallery.Core.Security;
using AshMind.Web.Gallery.Core.Security.Internal;

namespace AshMind.Web.Gallery.Core {
    public class CoreModule : Module {
        private readonly IFileSystem fileSystem;
        private readonly ILocation albumLocation;
        private readonly ILocation storageLocation;
        private readonly IFile picasaContactsXmlFile;

        public CoreModule(
            string albumPath,
            string storagePath,
            string picasaContactsXmlPath
        ) {
            this.fileSystem = new FileSystem();

            this.albumLocation = fileSystem.GetLocation(albumPath);
            this.storageLocation = fileSystem.GetLocation(storagePath);

            if (picasaContactsXmlPath != null)
                this.picasaContactsXmlFile = fileSystem.GetFile(picasaContactsXmlPath);
        }

        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);

            var types = this.GetType().Assembly.GetTypes();
            

            builder.Register(c => new MemoryCache("memory"))
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

            builder.Register<AuthorizationService>().SingletonScoped();
            builder.Register<JsonKeyPermissionProvider>()
                   .As<IPermissionProvider>()
                   .WithArguments(new TypedParameter(typeof(IFile), this.storageLocation.GetFile("permissions.jsdb", false)))
                   .SingletonScoped();

            builder.Register<JsonLocationPermissionProvider>()
                   .As<IPermissionProvider>()
                   .SingletonScoped();

            RegisterAlbumSupport(builder);

            builder.Register<PreviewFacade>().SingletonScoped();

            RegisterPicasaIntegration(builder);

            RegisterAllImplementationsOf<IAlbumProvider>(builder, types,        InstanceScope.Singleton);
            RegisterAllImplementationsOf<IOrientationProvider>(builder, types,  InstanceScope.Singleton);
            RegisterAllImplementationsOf<ICacheDependencyProvider>(builder, types, InstanceScope.Singleton);
            RegisterAllImplementationsOf<IAlbumFilter>(builder, types,          InstanceScope.Singleton);
        }

        private void RegisterAlbumSupport(ContainerBuilder builder) {
            builder.Register(c => new AlbumIDProvider(this.storageLocation, this.fileSystem))
                   .As<IAlbumIDProvider>();

            builder.Register<AlbumItemFactory>();
            
            builder.Register<AlbumFacade>()
                   .WithArguments(new TypedParameter(typeof(ILocation), this.albumLocation))
                   .SingletonScoped();
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

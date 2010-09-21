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
        private readonly string albumPath;
        private readonly string storagePath;
        private readonly string localAppDataPath;

        public CoreModule(
            string albumPath,
            string storagePath,
            string localAppDataPath
        ) {
            this.albumPath = albumPath;
            this.storagePath = storagePath;
            this.localAppDataPath = localAppDataPath;
        }

        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);

            var types = this.GetType().Assembly.GetTypes();

            builder.Register(c => new MemoryCache("memory"))
                   .As<ObjectCache>()
                   .FactoryScoped();

            builder.Register(new JsonSecurityRepository(Path.Combine(storagePath, "users.jsdb")))
                   .As<IRepository<User>, IRepository<UserGroup>>()
                   .SingletonScoped();

            builder.Register<JsonCommentRepository>()
                   .As<ICommentRepository>()
                   .SingletonScoped();

            builder.Register<FileSystem>()
                   .As<IFileSystem>()
                   .SingletonScoped();

            var cacheRootPath = Path.Combine(this.storagePath, "images");
            if (!Directory.Exists(cacheRootPath))
                Directory.CreateDirectory(cacheRootPath);
            builder.Register(c => new ImageCache(new Location(cacheRootPath), ImageCacheFormat.Jpeg, c.Resolve<ICacheDependencyProvider[]>()))
                   .SingletonScoped();

            builder.Register<AuthorizationService>();

            RegisterAlbumSupport(builder);

            builder.Register<PreviewFacade>().SingletonScoped();

            RegisterPicasaIntegration(builder);

            RegisterAllImplementationsOf<IAlbumProvider>(builder, types,        InstanceScope.Singleton);
            RegisterAllImplementationsOf<IPermissionProvider>(builder, types,   InstanceScope.Singleton);
            RegisterAllImplementationsOf<IOrientationProvider>(builder, types,  InstanceScope.Singleton);
            RegisterAllImplementationsOf<ICacheDependencyProvider>(builder, types, InstanceScope.Singleton);
            RegisterAllImplementationsOf<IAlbumFilter>(builder, types,          InstanceScope.Singleton);
        }

        private void RegisterAlbumSupport(ContainerBuilder builder) {
            builder.Register(c => new AlbumIDProvider(c.Resolve<IFileSystem>().GetLocation(this.storagePath), c.Resolve<IFileSystem>()))
                   .As<IAlbumIDProvider>();

            builder.Register<AlbumItemFactory>();
            
            builder.Register(c => new AlbumFacade(
                        c.Resolve<IFileSystem>().GetLocation(this.albumPath),
                        c.Resolve<IAlbumIDProvider>(),
                        c.Resolve<IAlbumProvider[]>(),
                        c.Resolve<IAlbumFilter[]>()
                    ))
                   .SingletonScoped();
        }

        private void RegisterPicasaIntegration(ContainerBuilder builder) {
            builder.Register<PicasaIniFileFinder>();
            builder.Register<PicasaIniParser>();

            if (this.localAppDataPath == null)
                return;

            builder.Register(c => new PicasaDatabase(c.Resolve<IFileSystem>().GetLocation(this.localAppDataPath)));

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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Autofac.Builder;

using AshMind.Extensions;

using AshMind.Web.Gallery.Core.AlbumSupport;
using AshMind.Web.Gallery.Core.Commenting;
using AshMind.Web.Gallery.Core.ImageProcessing;
using AshMind.Web.Gallery.Core.IO;
using AshMind.Web.Gallery.Core.Metadata;
using AshMind.Web.Gallery.Core.Security;
using AshMind.Web.Gallery.Core.Security.Internal;

namespace AshMind.Web.Gallery.Core {
    public class CoreModule : Module {
        private readonly string albumPath;
        private readonly string storagePath;

        public CoreModule(string albumPath, string storagePath) {
            this.albumPath = albumPath;
            this.storagePath = storagePath;
        }

        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);

            var types = this.GetType().Assembly.GetTypes();

            builder.Register(new JsonSecurityRepository(Path.Combine(storagePath, "users.jsdb")))
                   .As<IRepository<User>, IRepository<UserGroup>>()
                   .SingletonScoped();

            builder.Register<JsonCommentRepository>()
                   .As<ICommentRepository>()
                   .SingletonScoped();

            builder.Register<FileSystem>()
                   .As<IFileSystem>()
                   .SingletonScoped();

            builder.Register(new ImageCache(Path.Combine(this.storagePath, "images"), ImageCacheFormat.Jpeg));

            builder.Register(c => new AlbumIDProvider(this.storagePath)).As<IAlbumIDProvider>();

            builder.Register<AuthorizationService>();

            builder.Register(c => new AlbumFacade(
                        this.albumPath,
                        c.Resolve<PreviewFacade>(),
                        c.Resolve<IFileSystem>(),
                        c.Resolve<IAlbumIDProvider>(),
                        c.Resolve<AuthorizationService>(),
                        c.Resolve<ICommentRepository>(),
                        c.Resolve<IAlbumFilter[]>(),
                        c.Resolve<ITagProvider[]>()
                    ))
                   .SingletonScoped();

            builder.Register<PreviewFacade>().SingletonScoped();

            RegisterAllImplementationsOf<ITagProvider>(builder, types);
            RegisterAllImplementationsOf<IAlbumFilter>(builder, types);
        }

        private void RegisterAllImplementationsOf<TService>(ContainerBuilder builder, IEnumerable<Type> types) {
            var service = typeof(TService);
            types.Where(type => type.GetInterfaces().Contains(service))
                 .ForEach(
                    type => builder.Register(type).As(service, type)
                 );
        }
    }
}

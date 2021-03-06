﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

using Autofac;
using Autofac.Builder;

using AshMind.Extensions;

using AshMind.IO.Abstraction;
using AshMind.IO.Abstraction.DefaultImplementation;

using AshMind.Gallery.Core.AlbumSupport;
using AshMind.Gallery.Core.ImageProcessing;
using AshMind.Gallery.Core.Metadata;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core.Security.Internal;
using AshMind.Gallery.Imaging;

namespace AshMind.Gallery.Core {
    public class CoreModule : Module {
        private readonly ILocation albumLocation;
        private readonly ILocation storageLocation;

        private readonly Func<ObjectCache> cacheFactory;

        public CoreModule(
            ILocation albumLocation,
            ILocation storageLocation,
            Func<ObjectCache> cacheFactory
        ) {
            this.albumLocation = albumLocation;
            this.storageLocation = storageLocation;

            this.cacheFactory = cacheFactory;
        }

        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);

            var types = this.GetType().Assembly.GetTypes();
            

            builder.Register(c => cacheFactory())
                   .As<ObjectCache>()
                   .InstancePerDependency();

            builder.Register(c => new JsonSecurityRepository(
                        this.storageLocation.GetFile("users.jsdb", ActionIfMissing.ReturnAsIs)
                    ))
                   .As<IRepository<KnownUser>, IRepository<UserGroup>, IRepository<IUserGroup>>()
                   .SingleInstance();
            
            builder.RegisterType<FileSystem>()
                   .As<IFileSystem>()
                   .SingleInstance();

            var cacheRoot = this.storageLocation.GetLocation("images", ActionIfMissing.CreateNew);
            builder.Register(c => new ImageCache(
                        cacheRoot,
                        c.Resolve<IImageFormat[]>().Single(f => f.FileExtensions.Contains("jpeg")),
                        c.Resolve<IImageLoader>(),
                        c.Resolve<ICacheDependencyProvider[]>()
                   ))
                   .SingleInstance();
            
            RegisterAlbumSupport(builder, types);

            builder.RegisterType<PreviewFacade>().SingleInstance();
                        
            RegisterAllImplementationsOf<ILocationMetadataProvider>(builder, types, x => x.SingleInstance());
            RegisterAllImplementationsOf<IOrientationProvider>(builder, types, x => x.SingleInstance());
            RegisterAllImplementationsOf<ICacheDependencyProvider>(builder, types, x => x.SingleInstance());            
        }

        private void RegisterAlbumSupport(ContainerBuilder builder, IList<Type> types) {
            builder.Register(c => new AlbumIDProvider(this.storageLocation))
                   .As<IAlbumIDProvider>();

            builder.RegisterGeneric(typeof(AlbumFactory<>));
            builder.RegisterType<AlbumItemFactory>();

            builder.RegisterType<AlbumFacade>()
                   .As<IAlbumFacade>()
                   .WithParameter(new TypedParameter(typeof(ILocation), this.albumLocation))
                   .SingleInstance();

            RegisterAllImplementationsOf<IMetadataStore<AlbumItem>>(builder, types, x => x.SingleInstance());
            RegisterAllImplementationsOf<IMetadataStore<Album>>(builder, types, x => x.SingleInstance());

            builder.RegisterType<MetadataStoreUntypedAdapter<AlbumItem>>()
                   .As<IMetadataStore<object>>()
                   .SingleInstance();

            builder.RegisterType<MetadataStoreUntypedAdapter<Album>>()
               .As<IMetadataStore<object>>()
               .SingleInstance();

            RegisterAllImplementationsOf<IAlbumProvider>(builder, types, x => x.SingleInstance());
            RegisterAllImplementationsOf<IAlbumFilter>(builder, types, x => x.SingleInstance());
        }

        private void RegisterAllImplementationsOf<TService>(
            ContainerBuilder builder,
            IEnumerable<Type> types,
            Action<IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>> scope
        ) {
            var service = typeof(TService);
            types.Where(type => type.GetInterfaces().Contains(service))
                 .ForEach(
                    type => scope(builder.RegisterType(type).As(service, type))
                 );
        }
    }
}

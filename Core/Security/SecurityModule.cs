using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.Gallery.Core.Security.Rules;
using Autofac;

using AshMind.IO.Abstraction;
using AshMind.Gallery.Core.Security.Internal;

namespace AshMind.Gallery.Core.Security {
    public class SecurityModule : Module {
        private readonly ILocation storageLocation;

        public SecurityModule(ILocation storageLocation) {
            this.storageLocation = storageLocation;
        }

        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);

            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().SingleInstance();
            builder.RegisterAssemblyTypes(typeof(IAuthorizationRule).Assembly)
                   .As<IAuthorizationRule>()
                   .SingleInstance();

            builder.RegisterType<JsonKeyPermissionProvider>()
                   .As<IPermissionProvider>()
                   .WithParameter(new TypedParameter(typeof(IFile), this.storageLocation.GetFile("permissions.jsdb", false)))
                   .SingleInstance();

            builder.RegisterType<JsonLocationPermissionProvider>()
                   .As<IPermissionProvider>()
                   .SingleInstance();

            // because my current gallery has a lot of stuff with permissions set through this:
            builder.RegisterType<ObsoleteJsonLocationPermissionProvider>()
                   .As<IPermissionProvider>()
                   .SingleInstance();

            builder.RegisterType<MD5UserGroupSecureReferenceStrategy>()
                   .As<IUserGroupSecureReferenceStrategy>()
                   .InstancePerLifetimeScope();
        }
    }
}

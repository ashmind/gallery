using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;

namespace AshMind.Gallery.Imaging.Raw {
    public class RawImagingModule : Module {
        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);

            builder.RegisterType<RawImageLoader>()
                   .As<IImageLoader>()
                   .SingleInstance();
        }
    }
}

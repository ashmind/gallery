using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

using Autofac;

namespace AshMind.Gallery.Imaging.Wpf {
    public class WpfImagingModule : Module {
        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);

            builder.RegisterType<BitmapDecoderAdapter>()
                   .As<IImageReader>()
                   .SingleInstance();

            builder.RegisterType<BitmapEncoderAdapter>()
                   .As<IImageWriter>()
                   .SingleInstance();

            builder.RegisterType<JpegBitmapEncoder>()
                   .As<BitmapEncoder>()
                   .InstancePerDependency();
        }
    }
}

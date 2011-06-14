using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;

using Autofac;

namespace AshMind.Gallery.Imaging.GdiPlus {
    public class GdiImagingModule : Module {
        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);

            builder.RegisterType<ImageReader>()
                   .As<IImageReader>()
                   .SingleInstance();

            builder.RegisterType<ImageWriter>()
                   .As<IImageWriter>()
                   .SingleInstance();

            var jpegCodecInfo = ImageCodecInfo.GetImageEncoders().Single(c => c.FormatID == ImageFormat.Jpeg.Guid);
            builder.RegisterInstance(jpegCodecInfo)
                   .As<ImageCodecInfo>()
                   .SingleInstance();
        }
    }
}

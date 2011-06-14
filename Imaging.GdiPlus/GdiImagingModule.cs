using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;

using Autofac;

namespace AshMind.Gallery.Imaging.GdiPlus {
    public class GdiImagingModule : Module {
        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);

            builder.RegisterType<ImageLoader>()
                   .As<IImageLoader>()
                   .SingleInstance();
            
            builder.RegisterAdapter<ImageCodecInfo, ImageCodecAdapter>(c => new ImageCodecAdapter(c))
                   .As<IImageFormat>()
                   .SingleInstance();

            var codecs = ImageCodecInfo.GetImageEncoders();
            foreach (var codec in codecs) {
                builder.RegisterInstance(codec);
            }
        }
    }
}

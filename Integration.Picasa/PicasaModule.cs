using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.Gallery.Integration.Faces;
using Autofac;

using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Integration.Picasa {
    public class PicasaModule : Module {
        private readonly IFile picasaContactsXmlFile;

        public PicasaModule(IFile picasaContactsXmlFile) {
            this.picasaContactsXmlFile = picasaContactsXmlFile;
        }

        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);

            builder.RegisterType<PicasaIniFileFinder>();
            builder.RegisterType<PicasaIniParser>();

            if (this.picasaContactsXmlFile == null)
                return;

            builder.Register(c => new PicasaDatabase(this.picasaContactsXmlFile));

            builder.RegisterType<PicasaFaceProvider>()
                   .As<IFaceProvider>()
                   .SingleInstance();
        }
    }
}

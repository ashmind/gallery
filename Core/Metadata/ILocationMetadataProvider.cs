using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Gallery.Core.IO;

namespace AshMind.Gallery.Core.Metadata {
    public interface ILocationMetadataProvider {
        T GetMetadata<T>(ILocation location, string metadataKey)
            where T : class;

        void ApplyMetadata<T>(ILocation location, string metadataKey, T metadata)
            where T : class;
    }
}

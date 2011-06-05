using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Core.Metadata {
    public interface ILocationMetadataProvider {
        T GetMetadata<T>(ILocation location, string metadataKey)
            where T : class;

        void ApplyMetadata<T>(ILocation location, string metadataKey, T metadata)
            where T : class;
    }
}

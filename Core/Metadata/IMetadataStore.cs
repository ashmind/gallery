using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Metadata {
    public interface IMetadataStore<in T> {
        void LoadMetadataTo(T target);
        void SaveMetadata(T target);
    }
}

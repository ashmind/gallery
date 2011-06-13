using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

namespace AshMind.Gallery.Core.Metadata {
    public class MetadataStoreUntypedAdapter<T> : IMetadataStore<object> 
        where T : class
    {
        private readonly IMetadataStore<T>[] stores;

        public MetadataStoreUntypedAdapter(IMetadataStore<T>[] stores) {
            this.stores = stores;
        }

        public void LoadMetadataTo(object target) {
            var typed = target as T;
            if (typed == null)
                return;

            this.stores.ForEach(s => s.LoadMetadataTo(typed));
        }

        public void SaveMetadata(object target) {
            var typed = target as T;
            if (typed == null)
                return;

            this.stores.ForEach(s => s.SaveMetadata(typed));
        }
    }
}

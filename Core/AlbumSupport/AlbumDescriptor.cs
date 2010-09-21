using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.AlbumSupport {
    public class AlbumDescriptor {
        public AlbumDescriptor(string providerKey, string providerSpecificPath) {
            this.ProviderKey = providerKey;
            this.ProviderSpecificPath = providerSpecificPath;
        }

        public string ProviderKey { get; private set; }
        public string ProviderSpecificPath { get; private set; }

        public override bool Equals(object obj) {
            if (obj.GetType() != typeof(AlbumDescriptor))
                return false;

            var descriptor = obj as AlbumDescriptor;
            return this.ProviderKey == descriptor.ProviderKey
                && this.ProviderSpecificPath == descriptor.ProviderSpecificPath;
        }

        public override int GetHashCode() {
            return this.ProviderKey.GetHashCode() ^ this.ProviderSpecificPath.GetHashCode();
        }

        public override string ToString() {
            return new { this.ProviderKey, this.ProviderSpecificPath }.ToString();
        }
    }
}

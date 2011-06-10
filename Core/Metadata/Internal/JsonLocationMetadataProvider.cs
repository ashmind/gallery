using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

using Newtonsoft.Json;

using AshMind.Extensions;

using AshMind.IO.Abstraction;
using AshMind.Gallery.Core.Fixes;

namespace AshMind.Gallery.Core.Metadata.Internal {
    public class JsonLocationMetadataProvider : ILocationMetadataProvider {
        private readonly ObjectCache cache;

        public JsonLocationMetadataProvider(ObjectCache cache) {
            this.cache = cache;
        }

        public T GetMetadata<T>(ILocation location, string metadataKey)
            where T : class
        {
            Argument.VerifyNotNull("location", location);
            Argument.VerifyNotNullOrEmpty("metadataKey", metadataKey);

            var metadataRoot = this.GetCachedMetadata(location);
            var untypedMetadata = metadataRoot.GetValueOrDefault(metadataKey);
            if (untypedMetadata == null)
                return null;

            return JsonConvert.DeserializeObject<T>(
                JsonConvert.SerializeObject(untypedMetadata)
            );
        }

        public void ApplyMetadata<T>(ILocation location, string metadataKey, T metadata) 
            where T : class
        {
            Argument.VerifyNotNull("location", location);
            Argument.VerifyNotNullOrEmpty("metadataKey", metadataKey);
            Argument.VerifyNotNull("metadata", metadata);

            var file = GetMetadataFile(location, false);

            var metadataRoot = this.GetCachedMetadata(location);
            metadataRoot[metadataKey] = metadata;

            // http://social.msdn.microsoft.com/Forums/en-US/netfxbcl/thread/8d7aa093-5038-4b4a-a63e-bf949f0d0d13
            if (file.Exists)
                file.SetHidden(false);

            file.WriteAllText(
                JsonConvert.SerializeObject(metadataRoot, Formatting.Indented)
            );
            file.SetHidden(true);
        }

        private IDictionary<string, object> GetCachedMetadata(ILocation location) {
            var cacheKey = "metadata:" + location.Path;
            var metadata = (IDictionary<string, object>)this.cache.Get(cacheKey);
            if (metadata != null)
                return metadata;

            var file = GetMetadataFile(location, true);
            if (file == null) {
                metadata = new Dictionary<string, object>();
                this.cache.Add(cacheKey, metadata, new CacheItemPolicy {
                    ChangeMonitors = { new FixedFileChangeMonitor(new[] { location.Path }) }
                });
                return metadata;
            }

            metadata = JsonConvert.DeserializeObject<Dictionary<string, object>>(file.ReadAllText());
            this.cache.Add(cacheKey, metadata, new CacheItemPolicy {
                ChangeMonitors = { new FixedFileChangeMonitor(new[] { file.Path }) }
            });
            return metadata;
        }

        private IFile GetMetadataFile(ILocation location, bool nullUnlessExists) {
            return location.GetFile(".gallery.info", nullUnlessExists ? ActionIfMissing.ReturnNull : ActionIfMissing.ReturnAsIs);
        }
    }
}

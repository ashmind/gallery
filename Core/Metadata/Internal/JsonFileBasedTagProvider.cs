using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.Metadata.Internal {
    internal class JsonFileBasedTagProvider : ITagProvider {
        private class JsonAlbumMetadata {
            public string[] Tags { get; set; }
        }

        private readonly IFileSystem fileSystem;

        public JsonFileBasedTagProvider(IFileSystem fileSystem) {
            this.fileSystem = fileSystem;
        }

        public IEnumerable<string> GetTags(string path) {
            if (!fileSystem.IsLocation(path))
                return Enumerable.Empty<string>();

            var db = fileSystem.BuildPath(path, "gallery.json");
            if (!fileSystem.FileExists(db))
                return Enumerable.Empty<string>();

            var json = fileSystem.ReadAllText(db);
            var metadata = JsonConvert.DeserializeObject<JsonAlbumMetadata>(json);

            return metadata.Tags;
        }
    }
}

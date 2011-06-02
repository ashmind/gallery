using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;

using AshMind.Gallery.Core.IO;

namespace AshMind.Gallery.Core.AlbumSupport {
    internal class AlbumIDProvider : IAlbumIDProvider {
        private readonly IFile idMapFile;
        private readonly ConcurrentDictionary<string, AlbumDescriptor> idMap;

        public AlbumIDProvider(ILocation dataRoot) {
            this.idMapFile = dataRoot.GetFile("idmap", false);
            this.idMap = new ConcurrentDictionary<string, AlbumDescriptor>(this.LoadMap());
        }

        private IEnumerable<KeyValuePair<string, AlbumDescriptor>> LoadMap() {
            if (!this.idMapFile.Exists)
                return Enumerable.Empty<KeyValuePair<string, AlbumDescriptor>>();

            var lines = this.idMapFile.ReadAllLines();
            return lines.Select(line => line.Split('\t'))
                        .Select(parts => new KeyValuePair<string, AlbumDescriptor>(
                            parts[0],
                            new AlbumDescriptor(
                                parts[1],
                                parts[2]
                            )
                        ));
        }

        public string GetAlbumID(string name, AlbumDescriptor descriptor) {            
            var pair = this.idMap.Select(p => (KeyValuePair<string, AlbumDescriptor>?)p)
                                 .SingleOrDefault(p => p.Value.Equals(descriptor));

            if (pair != null)
                return pair.Value.Key;

            var id = Regex.Replace(name, @"(:?[\s'""]|:)", "_");
            if (this.idMap.TryAdd(id, descriptor))
                this.idMapFile.AppendAllText(id + "\t" + descriptor.ProviderKey + "\t" + descriptor.ProviderSpecificPath + Environment.NewLine);

            return id;
        }

        public AlbumDescriptor GetAlbumDescriptor(string albumID) {
            return idMap[albumID];
        }
    }
}

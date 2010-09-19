using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;

using File = System.IO.File;

namespace AshMind.Web.Gallery.Core.AlbumSupport {
    internal class AlbumIDProvider : IAlbumIDProvider {
        private readonly IFile idMapFile;
        private readonly ConcurrentDictionary<string, string> idMap;
        private readonly IFileSystem albumFileSystem;

        public AlbumIDProvider(ILocation dataRoot, IFileSystem albumFileSystem) {
            this.idMapFile = dataRoot.GetFile("idmap", false);
            this.idMap = new ConcurrentDictionary<string, string>(this.LoadMap());

            this.albumFileSystem = albumFileSystem;
        }

        private IEnumerable<KeyValuePair<string, string>> LoadMap() {
            if (!this.idMapFile.Exists)
                return Enumerable.Empty<KeyValuePair<string, string>>();

            var lines = this.idMapFile.ReadAllLines();
            return lines.Select(line => line.Split('\t'))
                        .Select(parts => new KeyValuePair<string, string>(parts[0], parts[1]));
        }

        public string GetAlbumID(ILocation location) {
            var pair = this.idMap.Select(p => (KeyValuePair<string, string>?)p)
                                 .SingleOrDefault(p => p.Value.Value == location.Path);

            if (pair != null)
                return pair.Value.Key;

            var id = location.Name.Replace(" ", "_");
            if (this.idMap.TryAdd(id, location.Path))
                this.idMapFile.AppendAllText(id + "\t" + location.Path + Environment.NewLine);

            return id;
        }

        public ILocation GetAlbumLocation(string albumID) {
            return this.albumFileSystem.GetLocation(idMap[albumID]);
        }
    }
}

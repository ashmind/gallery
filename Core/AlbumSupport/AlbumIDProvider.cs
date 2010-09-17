using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;

using File = System.IO.File;

namespace AshMind.Web.Gallery.Core.AlbumSupport
{
    internal class AlbumIDProvider : IAlbumIDProvider {
        private readonly string idMapFilePath;
        private readonly ConcurrentDictionary<string, string> idMap;
        private readonly IFileSystem fileSystem;

        public AlbumIDProvider(string dataRoot, IFileSystem fileSystem) {
            this.idMapFilePath = Path.Combine(dataRoot, "idmap");
            this.idMap = new ConcurrentDictionary<string,string>(this.LoadMap());

            this.fileSystem = fileSystem;
        }

        private IEnumerable<KeyValuePair<string, string>> LoadMap() {
            if (!File.Exists(this.idMapFilePath))
                return Enumerable.Empty<KeyValuePair<string, string>>();

            var lines = File.ReadAllLines(this.idMapFilePath);
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
                File.AppendAllText(this.idMapFilePath, id + "\t" + location + Environment.NewLine);

            return id;
        }

        public ILocation GetAlbumLocation(string albumID) {
            return this.fileSystem.GetLocation(idMap[albumID]);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.AlbumSupport
{
    internal class AlbumIDProvider : IAlbumIDProvider {
        private readonly string idMapFilePath;
        private readonly ConcurrentDictionary<string, string> idMap;

        public AlbumIDProvider(string dataRoot) {
            this.idMapFilePath = Path.Combine(dataRoot, "idmap");
            this.idMap = new ConcurrentDictionary<string,string>(this.LoadMap());
        }

        private IEnumerable<KeyValuePair<string, string>> LoadMap() {
            if (!File.Exists(this.idMapFilePath))
                return Enumerable.Empty<KeyValuePair<string, string>>();

            var lines = File.ReadAllLines(this.idMapFilePath);
            return lines.Select(line => line.Split('\t'))
                        .Select(parts => new KeyValuePair<string, string>(parts[0], parts[1]));
        }

        public string GetAlbumID(string location) {
            var pair = this.idMap.Select(p => (KeyValuePair<string, string>?)p)
                                 .SingleOrDefault(p => p.Value.Value == location);

            if (pair != null)
                return pair.Value.Key;

            var id = Path.GetFileName(location).Replace(" ", "_");
            if (this.idMap.TryAdd(id, location))
                File.AppendAllText(this.idMapFilePath, id + "\t" + location + Environment.NewLine);

            return id;
        }

        public string GetAlbumLocation(string albumID) {
            return idMap[albumID];
        }
    }
}

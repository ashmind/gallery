using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Gallery.Core.IO;
using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Core {
    public interface IAlbumProvider {
        IEnumerable<Album> GetAllAlbums(IEnumerable<ILocation> locations, IUser user);
        Album GetAlbum(IEnumerable<ILocation> locations, string providerSpecificPath, IUser user);

        string ProviderKey { get; }
    }
}

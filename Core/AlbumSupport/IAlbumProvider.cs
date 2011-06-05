using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.IO.Abstraction;
using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Core.AlbumSupport {
    public interface IAlbumProvider {
        IEnumerable<Album> GetAllAlbums(IEnumerable<ILocation> locations, IUser user);
        Album GetAlbum(IEnumerable<ILocation> locations, string providerSpecificPath, IUser user);

        string ProviderKey { get; }
    }
}

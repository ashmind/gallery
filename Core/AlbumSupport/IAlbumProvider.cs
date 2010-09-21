using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;
using AshMind.Web.Gallery.Core.Security;

namespace AshMind.Web.Gallery.Core {
    public interface IAlbumProvider {
        IEnumerable<Album> GetAllAlbums(IEnumerable<ILocation> locations, User user);
        Album GetAlbum(IEnumerable<ILocation> locations, string providerSpecificPath, User user);

        string ProviderKey { get; }
    }
}

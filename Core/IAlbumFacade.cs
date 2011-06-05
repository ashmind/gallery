using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core.Values;

namespace AshMind.Gallery.Core {
    public interface IAlbumFacade {
        IEnumerable<Album> GetAlbums(string providerKey, IUser user);
        Album GetAlbum(string albumID, IUser user);
        IValue<AlbumItem> GetItem(string albumID, string itemName, IUser user);
        string GetAlbumID(Album album);

        void SaveItem(AlbumItem item);        
    }
}

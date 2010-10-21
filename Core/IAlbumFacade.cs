using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Core {
    public interface IAlbumFacade {
        IEnumerable<Album> GetAlbums(string providerKey, IUser user);
        Album GetAlbum(string albumID, IUser user);
        AlbumItem GetItem(string albumID, string itemName, IUser user);
        string GetAlbumID(Album album);

        void SaveItem(AlbumItem item);        
    }
}

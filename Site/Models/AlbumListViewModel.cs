using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

using AshMind.Extensions;

using AshMind.Web.Gallery.Core;

namespace AshMind.Web.Gallery.Site.Models {
    public class AlbumListViewModel : PagedListViewModel<Album> {
        public AlbumListViewModel(
            IList<Album> albums,
            int start,
            int? yearBeforeStart
        ) : base(albums, start) {
            this.YearBeforeStart = yearBeforeStart;
        }

        public int? YearBeforeStart { get; private set; }
    }
}
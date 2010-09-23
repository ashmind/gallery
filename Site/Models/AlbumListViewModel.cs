using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

using AshMind.Extensions;

using AshMind.Gallery.Core;

namespace AshMind.Gallery.Site.Models {
    public class AlbumListViewModel : PagedListViewModel<AlbumViewModel> {
        public AlbumListViewModel(
            IList<AlbumViewModel> albums,
            int start,
            int? yearBeforeStart
        ) : base(albums, start) {
            this.YearBeforeStart = yearBeforeStart;
        }

        public int? YearBeforeStart { get; private set; }
    }
}
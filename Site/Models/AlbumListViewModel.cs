using System;
using System.Collections.Generic;
using System.Linq;

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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

using AshMind.Extensions;

using AshMind.Gallery.Core;

namespace AshMind.Gallery.Site.Models {
    public class PagedListViewModel<T> {
        public PagedListViewModel(
            IList<T> items, int start
        ) {
            this.List = items.AsReadOnly();
            this.Start = start;
        }

        public ReadOnlyCollection<T> List { get; private set; }
        public int Start { get; private set; }
        public int Count {
            get { return this.List.Count; }
        }
    }
}
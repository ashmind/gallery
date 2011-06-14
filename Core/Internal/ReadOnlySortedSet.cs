using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Internal {
    public class ReadOnlySortedSet<T> : ReadOnlySet<T> {
        public ReadOnlySortedSet(SortedSet<T> inner) : base(inner) {
        }
    }
}

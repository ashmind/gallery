using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Values {
    public static class ValueEnumerableExtensions {
        public static IValue<TSource> FirstOrDefault<TSource>(this IValue<IEnumerable<TSource>> source, Func<TSource, bool> predicate) {
            return source.Get(v => v.FirstOrDefault(predicate));
        }

        public static IValue<TResult> Min<TSource, TResult>(this IValue<IEnumerable<TSource>> source, Func<TSource, TResult> selector) {
            return source.Get(v => v.Min(selector));
        }
    }
}

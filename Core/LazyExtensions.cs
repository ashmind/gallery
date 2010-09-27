using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Gallery.Core {
    public static class LazyExtensions {
        public static Lazy<T> Apply<T>(this Lazy<T> lazy, Action<T> action) {
            return lazy.Apply(value => {
                action(value);
                return value;
            });
        }

        public static Lazy<T> Apply<T>(this Lazy<T> lazy, Func<T, T> function) {
            return new Lazy<T>(() => function(lazy.Value));
        }
    }
}

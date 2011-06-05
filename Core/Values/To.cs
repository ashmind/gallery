using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Values {
    public static class To {
        public static IValue<T> Null<T>() {
            return new NullValue<T>();
        }

        public static IValue<T> Just<T>(T value) {
            return new JustValue<T>(value);
        }

        public static IValue<T> Lazy<T>(Func<T> valueFactory) {
            return new LazyValue<T>(valueFactory);
        }
    }
}

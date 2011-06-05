using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Values {
    public class LazyValue<T> : IValue<T> {
        private readonly Lazy<T> lazy;

        public LazyValue(Func<T> valueFactory) {
            this.lazy = new Lazy<T>(valueFactory, true);
        }

        public IValue<TResult> Get<TResult>(Func<T, TResult> function) {
            return new LazyValue<TResult>(() => function(this.lazy.Value));
        }

        public IValue<T> Change(Action<T> action) {
            return this.Get(value => {
                action(value);
                return value;
            });
        }

        public T Value {
            get { return this.lazy.Value; }
        }
    }
}

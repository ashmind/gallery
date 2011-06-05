using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Values {
    public class JustValue<T> : IValue<T> {
        public JustValue(T value) {
            this.Value = value;
        }

        public IValue<TResult> Get<TResult>(Func<T, TResult> function) {
            return new JustValue<TResult>(function(this.Value));
        }

        public IValue<T> Change(Action<T> action) {
            action(this.Value);
            return this;
        }

        public T Value { get; private set; }
    }
}

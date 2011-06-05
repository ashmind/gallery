using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Values {
    public class NullValue<T> : IValue<T> {
        public virtual IValue<TResult> Get<TResult>(Func<T, TResult> function) {
            return new NullValue<TResult>();
        }

        public IValue<T> Change(Action<T> action) {
            return this;
        }

        public T Value {
            get { return (T)(object)null; }
        }
    }
}

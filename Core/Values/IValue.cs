using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Values {
    public interface IValue<out T> {
        IValue<TResult> Get<TResult>(Func<T, TResult> function);
        IValue<T> Change(Action<T> action);

        T Value { get; }
    }
}

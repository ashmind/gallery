using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core {
    public interface IRepository<T> {
        IQueryable<T> Query();

        object GetKey(T entity);
        T Load(object key);
    }
}

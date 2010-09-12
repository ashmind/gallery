using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core {
    public interface IRepository<T> {
        IQueryable<T> Query();
    }
}

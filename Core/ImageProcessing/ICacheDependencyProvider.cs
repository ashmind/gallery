using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.ImageProcessing {
    public interface ICacheDependencyProvider {
        IEnumerable<DateTimeOffset> GetRelatedChanges(IFile primary);
    }
}

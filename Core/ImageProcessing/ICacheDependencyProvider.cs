using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Gallery.Core.IO;

namespace AshMind.Gallery.Core.ImageProcessing {
    public interface ICacheDependencyProvider {
        IEnumerable<DateTimeOffset> GetRelatedChanges(IFile primary);
    }
}

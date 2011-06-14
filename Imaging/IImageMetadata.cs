using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Imaging {
    public interface IImageMetadata {
        T GetValue<T>(string key);
    }
}

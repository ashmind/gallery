using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Gallery.Core.IO {
    public enum FileLockMode {
        None,
        Read,
        Write,
        ReadWrite
    }
}

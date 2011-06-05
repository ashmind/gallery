using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.IO.Abstraction {
    public enum FileLockMode {
        None,
        Read,
        Write,
        ReadWrite
    }
}

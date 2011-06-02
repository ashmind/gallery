using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Security {
    public class SecurableUniqueKey {
        public SecurableUniqueKey(string value) {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}

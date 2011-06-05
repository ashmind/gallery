using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Security.Actions {
    public abstract class SecurableActionOn<T> : ISecurableAction {
        public T Target { get; private set; }

        protected SecurableActionOn(T target) {
            this.Target = target;
        }
    }
}

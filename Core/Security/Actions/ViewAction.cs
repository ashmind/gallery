using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Security.Actions {
    public class ViewAction<T> : SecurableActionOn<T> {
        public ViewAction(T target) : base(target) {
        }
    }
}

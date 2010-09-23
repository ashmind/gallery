using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Core {
    public class Comment {
        public Comment(User author, DateTimeOffset date, string text) {
            this.Author = author;
            this.Date = date;
            this.Text = text;
        }

        public User Author          { get; private set; }
        public DateTimeOffset Date  { get; private set; }
        public string Text          { get; private set; }
    }
}

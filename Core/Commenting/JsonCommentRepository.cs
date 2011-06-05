using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json;

using AshMind.Extensions;

using AshMind.Gallery.Core.IO;
using AshMind.Gallery.Core.Internal;
using AshMind.Gallery.Core.Security;
using System.IO;

namespace AshMind.Gallery.Core.Commenting {
    public class JsonCommentRepository : ICommentRepository {
        private class CommentStore {
            public CommentStore() {
                this.Comments = new List<RawComment>();
            }

            public IList<RawComment> Comments { get; private set; }
        }

        private class RawComment {
            [JsonProperty("Author")]
            public string AuthorEmailHash   { get; set; }
            public DateTimeOffset Date      { get; set; }
            public string Text              { get; set; }
        }

        private readonly IFileSystem fileSystem;
        private readonly IRepository<KnownUser> userRepository;

        public JsonCommentRepository(IFileSystem fileSystem, IRepository<KnownUser> userRepository) {
            this.fileSystem = fileSystem;
            this.userRepository = userRepository;
        }

        public IList<Comment> LoadCommentsOf(string itemPath) {
            var jsonFile = fileSystem.GetFile(itemPath + ".comments");
            if (jsonFile == null)
                return new List<Comment>();

            var commentStore = (CommentStore)null;
            using (var stream = jsonFile.Read(FileLockMode.Write)) {
                commentStore = LoadCommentStore(stream);
            }

            var raw = commentStore.Comments;
            var emailHashes = raw.Select(r => r.AuthorEmailHash).ToSet();

            using (var md5 = MD5.Create()) {
                var authors = this.userRepository.Query()
                    .AsEnumerable()
                    .Select(a => new { Author = a, EmailHash = md5.ComputeHashAsString(Encoding.UTF8.GetBytes(a.Email)) })
                    .Where(x => emailHashes.Contains(x.EmailHash))
                    .ToDictionary(x => x.EmailHash, x => x.Author);

                return raw.Select(c => new Comment(
                    authors[c.AuthorEmailHash],
                    c.Date, c.Text
                )).ToList();
            }
        }

        private CommentStore LoadCommentStore(Stream stream) {
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader)) {
                return new JsonSerializer().Deserialize<CommentStore>(jsonReader);
            }
        }

        public void SaveComment(string itemPath, Comment comment) {
            var jsonFile = this.fileSystem.GetFile(itemPath + ".comments", nullUnlessExists : false);
            var authorEmailHash = (string)null;
            using (var md5 = MD5.Create()) {
                authorEmailHash = md5.ComputeHashAsString(Encoding.UTF8.GetBytes(comment.Author.Email));
            }

            var raw = new RawComment {
                AuthorEmailHash = authorEmailHash,
                Date = comment.Date,
                Text = comment.Text
            };

            using (var stream = jsonFile.Open(FileLockMode.ReadWrite, FileOpenMode.ReadOrWrite)) {
                var store = stream.Length > 0 ? LoadCommentStore(stream) : new CommentStore();
                store.Comments.Add(raw);

                stream.Seek(0, SeekOrigin.Begin);
                using (var writer = new StreamWriter(stream))
                using (var jsonWriter = new JsonTextWriter(writer)) {
                    new JsonSerializer().Serialize(jsonWriter, store);
                }
            }
        }
    }
}

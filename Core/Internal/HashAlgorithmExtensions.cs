using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AshMind.Web.Gallery.Core.Internal {
    public static class HashAlgorithmExtensions {
        public static string ComputeHashAsString(this HashAlgorithm hashAlgorithm, byte[] buffer) {
            var hashBytes = hashAlgorithm.ComputeHash(buffer);
            var builder = new StringBuilder();
            foreach (var @byte in hashBytes) {
                builder.Append(@byte.ToString("x"));
            }

            return builder.ToString();
        }
    }
}

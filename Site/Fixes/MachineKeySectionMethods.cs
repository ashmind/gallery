using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;

using AshMind.Extensions;

// This is a hack -- this should be done using MachineKey.
// But I need IVType.Hash to get consistent URLs from encrypted data.
// No idea if this is insecure (it probably is, I should improve it later).

// Pull requests will be very appreciated.

namespace AshMind.Gallery.Site.Fixes {
    using EncryptOrDecryptDataFunc = Func<bool, byte[], byte[], int, int, bool, bool, IVType, bool, byte[]>;

    public static class MachineKeySectionMethods {
        private static readonly EncryptOrDecryptDataFunc EncryptOrDecryptDataFunc
            = (EncryptOrDecryptDataFunc)Delegate.CreateDelegate(
                typeof(EncryptOrDecryptDataFunc),
                typeof(MachineKeySection).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                    .Where(m => m.Name == "EncryptOrDecryptData")
                    .HavingMax(m => m.GetParameters().Length)
                    .Single()
            );

        public static byte[] EncryptOrDecryptData(bool fEncrypt, byte[] buf, byte[] modifier, int start, int length, bool useValidationSymAlgo, bool useLegacyMode, IVType ivType, bool signData) {
            return EncryptOrDecryptDataFunc(fEncrypt, buf, modifier, start, length, useValidationSymAlgo, useLegacyMode, ivType, signData);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using AshMind.Extensions;

using AshMind.IO.Abstraction;

using AshMind.Gallery.Integration.Picasa.IniParts;

namespace AshMind.Gallery.Integration.Picasa {
    public class PicasaIniParser {
        public PicasaIni Parse(IFile file) {
            Argument.VerifyNotNull("file", file);
            
            var lines = file.ReadAllLines();
            return Parse(lines);
        }

        private PicasaIni Parse(IEnumerable<string> lines) {
            var result = new PicasaIni();
            var linesIterator = lines.GetEnumerator();

            var finished = !linesIterator.MoveNext();
            while (!finished) {
                var line = linesIterator.Current;
                if (line == "[Contacts]") {
                    MoveOverContacts(result, linesIterator, out finished);
                    continue;
                }

                if (line.StartsWith("[") && line.Contains(".")) {
                    MoveOverItem(result, line, linesIterator, out finished);
                    continue;
                }

                finished = !linesIterator.MoveNext();
            }

            return result;
        }

        // example format: 8f7eee7d92388080=ashmind_lh,108
        private void MoveOverContacts(PicasaIni ini, IEnumerator<string> linesIterator, out bool linesIteratorEnded) {
            MoveOver(linesIterator, line => {
                var parsed = Regex.Match(line, "^(?<hash>[^=]+)=(?<user>[^,]+),(?<id>[^,]+)$");
                if (!parsed.Success)
                    return;

                ini.Contacts.Add(new PicasaIniContact(
                    parsed.Groups["hash"].Value,
                    parsed.Groups["user"].Value,
                    parsed.Groups["id"].Value
                ));
            }, out linesIteratorEnded);
        }


        private void MoveOverItem(PicasaIni ini, string firstLine, IEnumerator<string> linesIterator, out bool linesIteratorEnded) {
            var fileName = firstLine.RemoveStart("[").RemoveEnd("]");
            var metadata = ini.Items.GetValueOrDefault(fileName);
            if (metadata == null) {
                metadata = new PicasaIniMetadata();
                ini.Items.Add(fileName, metadata);
            }

            MoveOver(linesIterator, line => {
                if (line.StartsWith("rotate=")) {
                    var rotate = int.Parse(Regex.Match(line, @"rotate\(([\d+])\)").Groups[1].Value);
                    metadata.Rotate = rotate;
                }
                else if (line.StartsWith("faces=")) {
                    var faces = line.SubstringAfter("faces=").Split(';');
                    metadata.Faces.Clear();
                    foreach (var face in faces) {
                        metadata.Faces.Add(new PicasaIniFace(face.SubstringAfter(",")));
                    }
                }
            }, out linesIteratorEnded);            
        }

        private void MoveOver(IEnumerator<string> linesIterator, Action<string> action, out bool linesIteratorEnded) {
            linesIteratorEnded = !linesIterator.MoveNext();
            var line = linesIterator.Current;
            while (!linesIteratorEnded && !line.StartsWith("[")) {
                action(line);

                linesIteratorEnded = !linesIterator.MoveNext();
                line = linesIterator.Current;
            }
        }
    }
}

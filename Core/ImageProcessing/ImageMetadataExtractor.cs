using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using ExifLibrary;

namespace AshMind.Web.Gallery.Core.ImageProcessing {
    internal static class ImageMetadataExtractor {
        private static IDictionary<Orientation, RotateFlipType> orientations = new Dictionary<Orientation, RotateFlipType> {
            { Orientation.Normal,                            RotateFlipType.RotateNoneFlipNone },
            { Orientation.RotatedLeft,                       RotateFlipType.Rotate90FlipNone   },
            { Orientation.RotatedRight,                      RotateFlipType.Rotate270FlipNone  },
            { Orientation.RotatedRightAndMirroredVertically, RotateFlipType.Rotate270FlipY     },
            { Orientation.Rotated180,                        RotateFlipType.Rotate180FlipNone  },
        };

        private static Dictionary<byte[], Func<BinaryReader, ImageMetadata>> imageFormatDecoders = new Dictionary<byte[], Func<BinaryReader, ImageMetadata>>()
        {
            { new byte[] { 0x42, 0x4D },                                        DecodeBitmap},
            { new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 },                DecodeGif },
            { new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 },                DecodeGif },
            { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A },    DecodePng },
            { new byte[] { 0xff, 0xd8 },                                        DecodeJfif },
        };

        public static ImageMetadata ReadMetadata(string path) {
            using (var binaryReader = new BinaryReader(File.OpenRead(path))) {
                return ReadMetadata(binaryReader);
            }
        }

        public static ImageMetadata ReadMetadata(BinaryReader binaryReader) {
            var maxMagicBytesLength = imageFormatDecoders.Keys.Max(x => x.Length);

            var possibleDecoders = imageFormatDecoders.ToList();
            var byteIndex = 0;
            while (possibleDecoders.Count > 0) {
                var @byte = binaryReader.ReadByte();
                for (var i = possibleDecoders.Count - 1; i > 0; i--) {
                    if (possibleDecoders[i].Key[byteIndex] != @byte) {
                        possibleDecoders.RemoveAt(i);
                        continue;
                    }

                    if (byteIndex == possibleDecoders[i].Key.Length - 1)
                        return possibleDecoders[i].Value(binaryReader);
                }

                byteIndex += 1;
            }

            throw new NotSupportedException("File format is not supported.");
        }
        
        private static short ReadInt16LittleEndian(BinaryReader binaryReader) {
            var bytes = new byte[sizeof(short)];
            for (int i = 0; i < sizeof(short); i += 1) {
                bytes[sizeof(short) - 1 - i] = binaryReader.ReadByte();
            }
            return BitConverter.ToInt16(bytes, 0);
        }

        private static int ReadInt32LittleEndian(BinaryReader binaryReader) {
            var bytes = new byte[sizeof(int)];
            for (int i = 0; i < sizeof(int); i += 1) {
                bytes[sizeof(int) - 1 - i] = binaryReader.ReadByte();
            }
            return BitConverter.ToInt32(bytes, 0);
        }

        private static ImageMetadata DecodeBitmap(BinaryReader binaryReader) {
            binaryReader.ReadBytes(16);
            return new ImageMetadata(new Size(
                binaryReader.ReadInt32(),
                binaryReader.ReadInt32()
            ));
        }

        private static ImageMetadata DecodeGif(BinaryReader binaryReader) {
            return new ImageMetadata(new Size(
                binaryReader.ReadInt16(),
                binaryReader.ReadInt16()
            ));
        }

        private static ImageMetadata DecodePng(BinaryReader binaryReader) {
            binaryReader.ReadBytes(8);
            return new ImageMetadata(new Size(
                ReadInt32LittleEndian(binaryReader),
                ReadInt32LittleEndian(binaryReader)
            ));
        }

        private static ImageMetadata DecodeJfif(BinaryReader binaryReader) {
            while (binaryReader.ReadByte() == 0xff) {
                var marker = binaryReader.ReadByte();
                var chunkLength = ReadInt16LittleEndian(binaryReader);

                if (marker == 0xc0) {
                    binaryReader.ReadByte();

                    var height = ReadInt16LittleEndian(binaryReader);
                    var width = ReadInt16LittleEndian(binaryReader);
                    return new ImageMetadata(
                        new Size(width, height),
                        GetJfifOrientation(binaryReader)
                    );
                }

                binaryReader.ReadBytes(
                    chunkLength > 0
                        ? chunkLength - 2
                        : ((ushort)chunkLength) - 2
                );
            }

            throw new FormatException("JFIF format was not recognized.");
        }

        private static RotateFlipType? GetJfifOrientation(BinaryReader binaryReader) {
            binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);
            var exif = ExifFile.Read(binaryReader.BaseStream);

            if (!exif.Properties.ContainsKey(ExifTag.Orientation))
                return null;

            return orientations[(Orientation)exif.Properties[ExifTag.Orientation].Value];
        }
    }
}

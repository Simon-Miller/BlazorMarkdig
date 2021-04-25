using System;
using System.Collections.Generic;

namespace BlazorMarkdig.Shared.Models
{
    public class ImageFile
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// how this file will be requested.  Name of file without domain.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// original file extension, which should be equivalent of the Mime type.
        /// </summary>
        public string Extension { get; set; }

        public string MimeType { get; set; }

        public byte[] RawData { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int Size => RawData?.Length ?? 0;

        public byte[] Serialize()
        {
            var data = new List<byte>();

            encodeString(data, this.Id.ToString());
            encodeString(data, this.FileName);
            encodeString(data, this.Extension);
            encodeString(data, this.MimeType);
            encodeBytes(data, this.RawData);
            data.AddRange(toBytes(this.Width));
            data.AddRange(toBytes(this.Height));

            return data.ToArray();
        }

        private byte[] toBytes(string source)
        {
            return System.Text.ASCIIEncoding.UTF8.GetBytes(source);
        }
        private byte[] toBytes(UInt16 num)
        {
            var upper = num / 256;
            var lower = num - (upper * 256);

            return new byte[] { (byte)lower, (byte)upper };
        }
        private byte[] toBytes(int num)
        {
            var byte0 = num & 0x000000ff;
            var byte1 = (num >> 8) & 0x000000ff;
            var byte2 = (num >> 16) & 0x000000ff;
            var byte3 = (num >> 24);

            return new byte[] { (byte)byte0, (byte)byte1, (byte)byte2, (byte)byte3 };
        }

        private void encodeString(List<byte> output, string data)
        {
            var rawData = toBytes(data);
            var lenBytes = toBytes((UInt16)rawData.Length);

            output.AddRange(lenBytes);
            output.AddRange(rawData);
        }
        private void encodeBytes(List<byte> output, byte[] data)
        {
            var lenData = toBytes(data.Length);
            
            output.AddRange(lenData);
            output.AddRange(data);
        }


        public static ImageFile Deserialize(byte[] source)
        {            
            int offset = 0;

            return new ImageFile()
            {
                Id = Guid.Parse(decodeString(source, ref offset)),
                FileName = decodeString(source, ref offset),
                Extension = decodeString(source, ref offset),
                MimeType = decodeString(source, ref offset),
                RawData = decodeBytes(source, ref offset),
                Width = toInt(source, ref offset),
                Height = toInt(source, ref offset)
            };
        }

        private static UInt16 toUInt16(byte[] source, ref int offset)
        {
            var data = (UInt16)( (source[offset+1] << 8) | source[offset] );
            offset += 2;
            return data;
        }
        private static int toInt(byte[] source, ref int offset)
        {
            byte byte0 = source[offset++];
            byte byte1 = source[offset++];
            byte byte2 = source[offset++];
            byte byte3 = source[offset++];

            var calc = (byte3 << 24) | (byte2 << 16) | (byte1 << 8) | byte0;
            return (int)calc;
        }
        private static string toString(byte[] source, ref int offset, int length)
        {
            var stringData = subArray(source, offset, length);
            var result = System.Text.ASCIIEncoding.UTF8.GetString(stringData);
            offset += length;
            return result;
        }
        private static byte[] subArray(byte[] source, int offset, int length)
        {
            var destinationArray = new byte[length];
            for (int i = 0; i < length; i++)
                destinationArray[i] = source[offset + i];

            return destinationArray;
        }
        private static string decodeString(byte[] source, ref int offset)
        {
            var length = toUInt16(source, ref offset);
            return toString(source, ref offset, length);
        }
        private static byte[] decodeBytes(byte[] source, ref int offset)
        {
            var len = toInt(source, ref offset);
            var data = subArray(source, offset, len);
            offset += len;
            return data;
        }
    }
}

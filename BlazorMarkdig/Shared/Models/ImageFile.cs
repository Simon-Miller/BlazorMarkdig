using System;
using System.Collections.Generic;
using System.IO;

namespace BlazorMarkdig.Shared.Models
{
    /// <summary>
    /// the idea being we use this type right up to storing it.
    /// It'll parse as JSON unquestioningly.  
    /// We'll just have the trouble of getting to original byte[] back from the encoded RawData.  We could have some level of control over that if we wish,
    /// as a Base64 encoded string.
    /// </summary>
    public class ImageFileDTO
    {        
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; }
        public string RawData { get; set; } // to be deserialized 
        public int Width { get; set; }
        public int Height { get; set; }

        public static ImageFileDTO From(ImageFile source)
        {
            return new ImageFileDTO
            {
                Id = source.Id,
                FileName = source.FileName,
                Extension = source.Extension,
                MimeType = source.MimeType,
                Width = source.Width,
                Height = source.Height,
                RawData = Convert.ToBase64String(source.RawData)
            };
        }
    }

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

        // this will be a son-of-a-bitch piece of base64 encoded JSON.  Not really very good for an image of hundreds of K!  But hey, the cost of being lazy...
        public byte[] RawData { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int Size => RawData?.Length ?? 0;

        /// <summary>
        /// encodes this instance into a byte[], which can later be deserialised. see <seealso cref="Deserialize(Stream)"/>
        /// </summary>
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

        /// <summary>
        /// decode a byte stream into an instance of <see cref="ImageFile"/>.
        /// </summary>
        public static ImageFile Deserialize(Stream stream)
        {
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Flush(); // do I really need this to read the data?

            return new ImageFile()
            {
                Id = Guid.Parse(decodeString(stream)),
                FileName = decodeString(stream),
                Extension = decodeString(stream),
                MimeType = decodeString(stream),
                RawData = decodeBytes(stream),
                Width = toInt(stream),
                Height = toInt(stream)
            };
        }

        private static string decodeString(Stream stream)
        {
            var length = toUInt16(stream);
            return toString(stream, length);
        }
        private static UInt16 toUInt16(Stream stream)
        {
            var byte0 = (byte)stream.ReadByte();
            var byte1 = (byte)stream.ReadByte();

            var data = (UInt16)((byte1 << 8) | byte0);

            return data;
        }
        private static string toString(Stream stream, int length)
        {
            var stringData = subArray(stream, length);
            var result = System.Text.ASCIIEncoding.UTF8.GetString(stringData);

            return result;
        }
        private static byte[] subArray(Stream stream, int length)
        {
            var destinationArray = new byte[length];
            for (int i = 0; i < length; i++)
                destinationArray[i] = (byte)stream.ReadByte();

            return destinationArray;
        }
        private static byte[] decodeBytes(Stream stream)
        {
            var len = toInt(stream);
            var data = subArray(stream, len);

            return data;
        }
        private static int toInt(Stream stream)
        {
            var byte0 = (byte)stream.ReadByte();
            var byte1 = (byte)stream.ReadByte();
            var byte2 = (byte)stream.ReadByte();
            var byte3 = (byte)stream.ReadByte();

            var calc = (byte3 << 24) | (byte2 << 16) | (byte1 << 8) | byte0;
            return (int)calc;
        }

        public static ImageFile From(ImageFileDTO source)
        {
            return new ImageFile 
            {
                Id = source.Id,
                FileName = source.FileName,
                Extension = source.Extension,
                MimeType = source.MimeType,
                Width = source.Width,
                Height = source.Height,
                RawData = Convert.FromBase64String(source.RawData)
            };
        }
    }
}

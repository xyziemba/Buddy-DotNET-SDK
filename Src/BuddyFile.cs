using System;
using System.IO;

namespace BuddySDK
{
    public class BuddyFile
    {
        public Stream Data {get; private set;}

        private byte[] _bytes;
        internal byte[] Bytes
        {
            get
            {
                if (Data != null && _bytes == null)
                {
                    _bytes = new byte[Data.Length];
                    long pos = Data.Position;

                    if (Data.CanSeek) {
                        Data.Seek (0, SeekOrigin.Begin);
                    }
                    Data.Read(_bytes, 0, _bytes.Length);
                    if (Data.CanSeek) {
                        Data.Seek (pos, SeekOrigin.Begin);
                    }

                }
                return _bytes;
            }
        }
        public string Name {get; private set;}

        public string ContentType {get; private set;}

       
        public BuddyFile(Stream stream, string name = null, string contentType = "application/octet-stream") {
            Data = stream;
            ContentType = contentType;
            Name = name;
        }
    }
}


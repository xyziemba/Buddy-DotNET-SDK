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
                    long pos = 0;

                    if (Data.CanSeek) {
                        pos = Data.Position;
                        Data.Seek(0, SeekOrigin.Begin);
                    }

                    GetBytesFromStream();
                   
                    if (Data.CanSeek) {
                        Data.Seek (pos, SeekOrigin.Begin);
                    }
                }

                return _bytes;
            }
        }

        private void GetBytesFromStream()
        {
            if (Data is MemoryStream)
            {
                _bytes = ((MemoryStream)Data).ToArray();
            }
            else
            {
                using (var ms = new MemoryStream())
                {
                    Data.CopyTo(ms);

                    _bytes = ms.ToArray();
                }
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


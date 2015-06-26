using System;
using System.IO;

namespace YamlSharp
{
    public class YamlReader : IDisposable
    {
        private const int MaxBufferSize = 1024;

        private readonly Stream stream;
        //private readonly Encoding encoding;
        private readonly byte[] buffer = new byte[MaxBufferSize];

        private int position = 0;
        private int length = 0;

        public YamlReader(string fileName)
            : this(new FileStream(fileName, FileMode.Open))
        { }

        public YamlReader(Stream stream)
        {
            this.stream = stream;
        }

        void IDisposable.Dispose()
        {
            if (stream != null)
                stream.Dispose();
        }

        public void Peek(int numChars = 1)
        {

        }

        private void UpdateBuffer()
        {
            var numValues = length - position;
            for (var i = 0; i < numValues; i++)
                buffer[i] = buffer[position + i];

            var temp = new byte[MaxBufferSize - numValues];
            var count = stream.Read(temp, 0, temp.Length);
            if (count == 0)
                return;

            Array.Copy(temp, 0, buffer, numValues, count);
        }
    }
}
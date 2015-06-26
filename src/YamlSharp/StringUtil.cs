using System.IO;
using System.Text;

namespace YamlSharp
{
    public static class StringUtil
    {
        private static Encoding GetFileEncoding(Stream fileStream)
        {
            var buffer = new byte[5];

            fileStream.Read(buffer, 0, 5);

            if (buffer[0] == 0x00 && buffer[1] == 0x00 && buffer[2] == 0xFE && buffer[3] == 0xFF)
                return new UTF32Encoding(true, true, true);
            if (buffer[0] == 0x00 && buffer[1] == 0x00 && buffer[2] == 0x00)
                return new UTF32Encoding(true, false, true);
            if (buffer[0] == 0xFF && buffer[1] == 0xFE && buffer[2] == 0x00 && buffer[3] == 0x00)
                return new UTF32Encoding(false, true, true);
            if (buffer[1] == 0x00 && buffer[2] == 0x00 && buffer[3] == 0x00)
                return new UTF32Encoding(false, false, true);
            if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                return new UnicodeEncoding(true, true, true);
            if (buffer[0] == 0x00)
                return new UnicodeEncoding(true, false, true);
            if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                return new UnicodeEncoding(false, true, true);
            if (buffer[1] == 0x00)
                return new UnicodeEncoding(false, false, true);
            if (buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
                return new UTF8Encoding(true, true);
            return new UTF8Encoding(false, true);
        }

        public static Encoding GetFileEncoding(string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Open))
                return GetFileEncoding(fileStream);
        }
    }
}
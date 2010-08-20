using System.Collections.Generic;
using System.IO;

namespace YamlSharp
{
    public class YamlReader
    {
        private readonly TextReader reader;

        public YamlReader(string fileName)
        {
            reader = new StreamReader(fileName, StringUtil.GetFileEncoding(fileName));
        }

        public IEnumerable<YamlDocument> Read()
        {
            var line = reader.ReadLine();
            while (line != null)
            {
                yield return null;
                line = reader.ReadLine();
            }
        }
    }
}
using System.IO;
using System.Linq;
using NUnit.Framework;
using System.Text;

namespace YamlSharp.Test
{
    [TestFixture]
    public class StringUtilTest
    {
        [Test]
        public void GetFileEncodingTest()
        {
            var testFiles = new[]
            {
                "utf8.yml",
                "utf8-explicit-bom.yml",
                "utf16be.yml",
                "utf16be-explicit-bom.yml",
                "utf16le.yml",
                "utf16le-explicit-bom.yml",
                "utf32be.yml",
                "utf32be-explicit-bom.yml",
                "utf32le.yml",
                "utf32le-explicit-bom.yml"
            };

            foreach (var fileName in testFiles.Select(f => string.Format(Path.Combine("..", "..", "TestData", f))))
                using (TextReader reader = new StreamReader(fileName, StringUtil.GetFileEncoding(fileName)))
                    Assert.AreEqual("test", reader.ReadLine());
        }

        [Test]
        public void Katse()
        {
            using (var s = File.OpenRead(Path.Combine("..", "..", "TestData", "utf32le-explicit-bom.yml")))
            {
                s.ReadByte(); s.ReadByte(); s.ReadByte(); s.ReadByte();
                s.ReadByte(); s.ReadByte(); s.ReadByte(); s.ReadByte();

                using (TextReader r = new StreamReader(s, new UTF32Encoding(false, true, true)))
                    Assert.AreEqual("est", r.ReadLine());
            }
        }
    }
}
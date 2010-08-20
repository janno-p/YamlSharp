using System.IO;
using NUnit.Framework;

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
                "utf32le-explicit-bom.yml",
            };

            foreach (var testFile in testFiles)
            {
                var fileName = string.Format("../../TestData/{0}", testFile);
                using (TextReader reader = new StreamReader(fileName, StringUtil.GetFileEncoding(fileName)))
                    Assert.AreEqual("test", reader.ReadLine());
            }
        }
    }
}
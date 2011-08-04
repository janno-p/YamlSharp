using NUnit.Framework;
using System.IO;

namespace YamlSharp.Test
{
	[TestFixture]
	public class ScannerTest
	{
		[Test]
		public void ReadDocumentPrefix()
		{
			using (var reader = new StreamReader(string.Format("TestData{0}example-9.1_document-prefix.yml", Path.DirectorySeparatorChar)))
			{
				var scanner = new Scanner(reader);
				foreach (var token in scanner.ReadTokens())
					;
				Assert.AreEqual(scanner.CurrentLine, "Document");
			}
		}
	}
}
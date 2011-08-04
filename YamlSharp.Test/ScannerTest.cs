using NUnit.Framework;
using System.IO;
using System.Linq;
using YamlSharp.Tokens;

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

        [Test]
        public void ReadReservedDirective()
        {
            using (var reader = new StreamReader(string.Format("TestData{0}example-6.13_reserved-directives.yml", Path.DirectorySeparatorChar)))
            {
                var scanner = new Scanner(reader);
                var list = scanner.ReadTokens().ToList();

                Assert.That(list.Count, Is.EqualTo(5));
                Assert.That(list[0], Is.InstanceOf(typeof(StreamStartToken)));
                Assert.That(list[1], Is.InstanceOf(typeof(DirectivesStartToken)));
                Assert.That(list[2], Is.InstanceOf(typeof(DirectiveToken)));
                Assert.That(list[3], Is.InstanceOf(typeof(DirectivesEndToken)));
                Assert.That(list[4], Is.InstanceOf(typeof(StreamEndToken)));
                Assert.That(scanner.CurrentLine, Is.EqualTo(" \"foo\""));

                var directiveToken = list[2] as DirectiveToken;
                Assert.That(directiveToken.Name, Is.EqualTo("FOO"));

                var directiveParameters = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters.Count, Is.EqualTo(2));
                Assert.That(directiveParameters[0], Is.EqualTo("bar"));
                Assert.That(directiveParameters[1], Is.EqualTo("baz"));
            }
        }

        [Test]
        public void ReadYamlDirective()
        {
            using (var reader = new StreamReader(string.Format("TestData{0}example-6.14_yaml-directive.yml", Path.DirectorySeparatorChar)))
            {
                var scanner = new Scanner(reader);
                var list = scanner.ReadTokens().ToList();

                Assert.That(list.Count, Is.EqualTo(5));
                Assert.That(list[0], Is.InstanceOf(typeof(StreamStartToken)));
                Assert.That(list[1], Is.InstanceOf(typeof(DirectivesStartToken)));
                Assert.That(list[2], Is.InstanceOf(typeof(DirectiveToken)));
                Assert.That(list[3], Is.InstanceOf(typeof(DirectivesEndToken)));
                Assert.That(list[4], Is.InstanceOf(typeof(StreamEndToken)));
                Assert.That(scanner.CurrentLine, Is.EqualTo("\"foo\""));

                var directiveToken = list[2] as DirectiveToken;
                Assert.That(directiveToken.Name, Is.EqualTo("YAML"));

                var directiveParameters = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters.Count, Is.EqualTo(1));
                Assert.That(directiveParameters[0], Is.EqualTo("1.3"));
            }
        }

        [Test]
        public void ReadInvalidRepeatedYamlDirective()
        {
            using (var reader = new StreamReader(string.Format("TestData{0}example-6.15_invalid-repeated-yaml-directive.yml", Path.DirectorySeparatorChar)))
            {
                var scanner = new Scanner(reader);
                var list = scanner.ReadTokens().ToList();

                Assert.That(list.Count, Is.EqualTo(6));
                Assert.That(list[0], Is.InstanceOf(typeof(StreamStartToken)));
                Assert.That(list[1], Is.InstanceOf(typeof(DirectivesStartToken)));
                Assert.That(list[2], Is.InstanceOf(typeof(DirectiveToken)));
                Assert.That(list[3], Is.InstanceOf(typeof(DirectiveToken)));
                Assert.That(list[4], Is.InstanceOf(typeof(DirectivesEndToken)));
                Assert.That(list[5], Is.InstanceOf(typeof(StreamEndToken)));
                Assert.That(scanner.CurrentLine, Is.EqualTo("foo"));

                var directiveToken = list[2] as DirectiveToken;
                Assert.That(directiveToken.Name, Is.EqualTo("YAML"));

                var directiveToken2 = list[3] as DirectiveToken;
                Assert.That(directiveToken.Name, Is.EqualTo("YAML"));

                var directiveParameters = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters.Count, Is.EqualTo(1));
                Assert.That(directiveParameters[0], Is.EqualTo("1.2"));

                var directiveParameters2 = directiveToken2.Parameters.ToList();
                Assert.That(directiveParameters2.Count, Is.EqualTo(1));
                Assert.That(directiveParameters2[0], Is.EqualTo("1.1"));
            }
        }

        // TODO : Tag directives
	}
}
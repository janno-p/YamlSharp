using NUnit.Framework;
using System.IO;
using System.Linq;
using YamlSharp.Tokens;
using System;

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
				foreach (var token in scanner.ReadTokens()) {}
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
                Assert.IsNotNull(directiveToken);
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
                Assert.IsNotNull(directiveToken);
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
                Assert.IsNotNull(directiveToken);
                Assert.That(directiveToken.Name, Is.EqualTo("YAML"));

                var directiveToken2 = list[3] as DirectiveToken;
                Assert.IsNotNull(directiveToken2);
                Assert.That(directiveToken2.Name, Is.EqualTo("YAML"));

                var directiveParameters = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters.Count, Is.EqualTo(1));
                Assert.That(directiveParameters[0], Is.EqualTo("1.2"));

                var directiveParameters2 = directiveToken2.Parameters.ToList();
                Assert.That(directiveParameters2.Count, Is.EqualTo(1));
                Assert.That(directiveParameters2[0], Is.EqualTo("1.1"));
            }
        }

        [Test]
        public void ReadTagDirective()
        {
            using (var reader = new StreamReader(string.Format("TestData{0}example-6.16_tag-directive.yml", Path.DirectorySeparatorChar)))
            {
                var scanner = new Scanner(reader);
                var list = scanner.ReadTokens().ToList();

                Assert.That(list.Count, Is.EqualTo(5));
                Assert.That(list[0], Is.InstanceOf(typeof(StreamStartToken)));
                Assert.That(list[1], Is.InstanceOf(typeof(DirectivesStartToken)));
                Assert.That(list[2], Is.InstanceOf(typeof(DirectiveToken)));
                Assert.That(list[3], Is.InstanceOf(typeof(DirectivesEndToken)));
                Assert.That(list[4], Is.InstanceOf(typeof(StreamEndToken)));
                Assert.That(scanner.CurrentLine, Is.EqualTo("!yaml!str \"foo\""));

                var directiveToken = list[2] as DirectiveToken;
                Assert.IsNotNull(directiveToken);
                Assert.That(directiveToken.Name, Is.EqualTo("TAG"));

                var directiveParameters = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters.Count, Is.EqualTo(2));
                Assert.That(directiveParameters[0], Is.EqualTo("!yaml!"));
                Assert.That(directiveParameters[1], Is.EqualTo("tag:yaml.org,2002:"));
            }
        }

        [Test]
        public void ReadInvalidRepeatedTagDirective()
        {
            using (var reader = new StreamReader(string.Format("TestData{0}example-6.17_invalid-repeated-tag-directive.yml", Path.DirectorySeparatorChar)))
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
                Assert.That(scanner.CurrentLine, Is.EqualTo("bar"));

                var directiveToken = list[2] as DirectiveToken;
                Assert.IsNotNull(directiveToken);
                Assert.That(directiveToken.Name, Is.EqualTo("TAG"));

                var directiveToken2 = list[2] as DirectiveToken;
                Assert.IsNotNull(directiveToken2);
                Assert.That(directiveToken2.Name, Is.EqualTo("TAG"));

                var directiveParameters = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters.Count, Is.EqualTo(2));
                Assert.That(directiveParameters[0], Is.EqualTo("!"));
                Assert.That(directiveParameters[1], Is.EqualTo("!foo"));

                var directiveParameters2 = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters2.Count, Is.EqualTo(2));
                Assert.That(directiveParameters2[0], Is.EqualTo("!"));
                Assert.That(directiveParameters2[1], Is.EqualTo("!foo"));
            }
        }

        [Test]
        public void ReadSecondaryTagHandle()
        {
            using (var reader = new StreamReader(string.Format("TestData{0}example-6.19_secondary-tag-handle.yml", Path.DirectorySeparatorChar)))
            {
                var scanner = new Scanner(reader);
                var list = scanner.ReadTokens().ToList();

                Assert.That(list.Count, Is.EqualTo(5));
                Assert.That(list[0], Is.InstanceOf(typeof(StreamStartToken)));
                Assert.That(list[1], Is.InstanceOf(typeof(DirectivesStartToken)));
                Assert.That(list[2], Is.InstanceOf(typeof(DirectiveToken)));
                Assert.That(list[3], Is.InstanceOf(typeof(DirectivesEndToken)));
                Assert.That(list[4], Is.InstanceOf(typeof(StreamEndToken)));
                Assert.That(scanner.CurrentLine, Is.EqualTo("!!int 1 - 3 # Interval, not integer"));

                var directiveToken = list[2] as DirectiveToken;
                Assert.IsNotNull(directiveToken);
                Assert.That(directiveToken.Name, Is.EqualTo("TAG"));

                var directiveParameters = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters.Count, Is.EqualTo(2));
                Assert.That(directiveParameters[0], Is.EqualTo("!!"));
                Assert.That(directiveParameters[1], Is.EqualTo("tag:example.com,2000:app/"));
            }
        }

        [Test]
        public void ReadTagHandles()
        {
            using (var reader = new StreamReader(string.Format("TestData{0}example-6.20_tag-handles.yml", Path.DirectorySeparatorChar)))
            {
                var scanner = new Scanner(reader);
                var list = scanner.ReadTokens().ToList();

                Assert.That(list.Count, Is.EqualTo(5));
                Assert.That(list[0], Is.InstanceOf(typeof(StreamStartToken)));
                Assert.That(list[1], Is.InstanceOf(typeof(DirectivesStartToken)));
                Assert.That(list[2], Is.InstanceOf(typeof(DirectiveToken)));
                Assert.That(list[3], Is.InstanceOf(typeof(DirectivesEndToken)));
                Assert.That(list[4], Is.InstanceOf(typeof(StreamEndToken)));
                Assert.That(scanner.CurrentLine, Is.EqualTo("!e!foo \"bar\""));

                var directiveToken = list[2] as DirectiveToken;
                Assert.IsNotNull(directiveToken);
                Assert.That(directiveToken.Name, Is.EqualTo("TAG"));

                var directiveParameters = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters.Count, Is.EqualTo(2));
                Assert.That(directiveParameters[0], Is.EqualTo("!e!"));
                Assert.That(directiveParameters[1], Is.EqualTo("tag:example.com,2000:app/"));
            }
        }

        [Test, Ignore("Can not handle multiple documents, yet!")]
        public void ReadLocalTagPrefix()
        {
            using (var reader = new StreamReader(string.Format("TestData{0}example-6.21_local-tag-prefix.yml", Path.DirectorySeparatorChar)))
            {
                var scanner = new Scanner(reader);
                var list = scanner.ReadTokens().ToList();

                Assert.That(list.Count, Is.EqualTo(5));
                Assert.That(list[0], Is.InstanceOf(typeof(StreamStartToken)));
                Assert.That(list[1], Is.InstanceOf(typeof(DirectivesStartToken)));
                Assert.That(list[2], Is.InstanceOf(typeof(DirectiveToken)));
                Assert.That(list[3], Is.InstanceOf(typeof(DirectivesEndToken)));
                Assert.That(list[4], Is.InstanceOf(typeof(StreamEndToken)));
                Assert.That(scanner.CurrentLine, Is.EqualTo("!m!light fluorescent"));

                var directiveToken = list[2] as DirectiveToken;
                Assert.IsNotNull(directiveToken);
                Assert.That(directiveToken.Name, Is.EqualTo("TAG"));

                var directiveParameters = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters.Count, Is.EqualTo(2));
                Assert.That(directiveParameters[0], Is.EqualTo("!m!"));
                Assert.That(directiveParameters[1], Is.EqualTo("!my-"));
            }
        }

        [Test]
        public void ReadGlobalTagPrefix()
        {
            using (var reader = new StreamReader(string.Format("TestData{0}example-6.22_global-tag-prefix.yml", Path.DirectorySeparatorChar)))
            {
                var scanner = new Scanner(reader);
                var list = scanner.ReadTokens().ToList();

                Assert.That(list.Count, Is.EqualTo(5));
                Assert.That(list[0], Is.InstanceOf(typeof(StreamStartToken)));
                Assert.That(list[1], Is.InstanceOf(typeof(DirectivesStartToken)));
                Assert.That(list[2], Is.InstanceOf(typeof(DirectiveToken)));
                Assert.That(list[3], Is.InstanceOf(typeof(DirectivesEndToken)));
                Assert.That(list[4], Is.InstanceOf(typeof(StreamEndToken)));
                Assert.That(scanner.CurrentLine, Is.EqualTo("- !e!foo \"bar\""));

                var directiveToken = list[2] as DirectiveToken;
                Assert.IsNotNull(directiveToken);
                Assert.That(directiveToken.Name, Is.EqualTo("TAG"));

                var directiveParameters = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters.Count, Is.EqualTo(2));
                Assert.That(directiveParameters[0], Is.EqualTo("!e!"));
                Assert.That(directiveParameters[1], Is.EqualTo("tag:example.com,2000:app/"));
            }
        }

        [Test, ExpectedException(typeof(Exception), ExpectedMessage = "Directives end marker missing")]
        public void ReadDirectivesEndTagMissing()
        {
            using (var reader = new StreamReader(string.Format("TestData{0}directives-end-tag-missing.yml", Path.DirectorySeparatorChar)))
            {
                new Scanner(reader).ReadTokens().ToList();

                Assert.Fail("This test should fail!");
            }
        }
	}
}
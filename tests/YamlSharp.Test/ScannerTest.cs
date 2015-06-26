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
            using (var reader = new StreamReader(Path.Combine("TestData", "example-9.1_document-prefix.yml")))
            {
                var scanner = new Scanner(reader);
                var tokens = scanner.ReadTokens().ToList();

                Assert.AreEqual(7, tokens.Count);

                var documentContentToken = tokens.Find(t => t is DocumentContentToken) as DocumentContentToken;

                Assert.IsNotNull(documentContentToken);
                Assert.AreEqual("Document", documentContentToken.Content.TrimEnd());
            }
        }

        [Test]
        public void ReadExplicitDocument()
        {
            using (var reader = new StreamReader(Path.Combine("TestData", "example-9.4_explicit-document.yml")))
            {
                var scanner = new Scanner(reader);
                var tokens = scanner.ReadTokens().ToList();

                Assert.AreEqual(12, tokens.Count);

                var documentContentToken = tokens.FindLast(t => t is DocumentContentToken) as DocumentContentToken;

                Assert.IsNotNull(documentContentToken);
                Assert.That(documentContentToken.Content.TrimEnd(), Is.Empty);
            }
        }

        [Test]
        public void ReadBareDocuments()
        {
            using (var reader = new StreamReader(Path.Combine("TestData", "example-9.3_bare-documents.yml")))
            {
                var scanner = new Scanner(reader);
                var tokens = scanner.ReadTokens().ToList();

                Assert.That(tokens.Count, Is.EqualTo(12));
                Assert.That(tokens[0], Is.InstanceOf(typeof(StreamStartToken)));
                Assert.That(tokens[1], Is.InstanceOf(typeof(DirectivesStartToken)));
                Assert.That(tokens[2], Is.InstanceOf(typeof(DirectivesEndToken)));
                Assert.That(tokens[3], Is.InstanceOf(typeof(DocumentStartToken)));
                Assert.That(tokens[4], Is.InstanceOf(typeof(DocumentContentToken)));
                Assert.That(tokens[5], Is.InstanceOf(typeof(DocumentEndToken)));
                Assert.That(tokens[6], Is.InstanceOf(typeof(DirectivesStartToken)));
                Assert.That(tokens[7], Is.InstanceOf(typeof(DirectivesEndToken)));
                Assert.That(tokens[8], Is.InstanceOf(typeof(DocumentStartToken)));
                Assert.That(tokens[9], Is.InstanceOf(typeof(DocumentContentToken)));
                Assert.That(tokens[10], Is.InstanceOf(typeof(DocumentEndToken)));
                Assert.That(tokens[11], Is.InstanceOf(typeof(StreamEndToken)));

                Assert.That(((DocumentContentToken)tokens[4]).Content, Is.Not.Empty);
                Assert.That(((DocumentContentToken)tokens[9]).Content, Is.Not.Empty);
            }
        }

        [Test]
        public void ReadReservedDirective()
        {
            using (var reader = new StreamReader(Path.Combine("TestData", "example-6.13_reserved-directives.yml")))
            {
                var scanner = new Scanner(reader);
                var tokens = scanner.ReadTokens().ToList();

                Assert.That(tokens.Count, Is.EqualTo(8));
                Assert.That(tokens[0], Is.InstanceOf(typeof(StreamStartToken)));
                Assert.That(tokens[1], Is.InstanceOf(typeof(DirectivesStartToken)));
                Assert.That(tokens[2], Is.InstanceOf(typeof(DirectiveToken)));
                Assert.That(tokens[3], Is.InstanceOf(typeof(DirectivesEndToken)));
                Assert.That(tokens[4], Is.InstanceOf(typeof(DocumentStartToken)));
                Assert.That(tokens[5], Is.InstanceOf(typeof(DocumentContentToken)));
                Assert.That(tokens[6], Is.InstanceOf(typeof(DocumentEndToken)));
                Assert.That(tokens[7], Is.InstanceOf(typeof(StreamEndToken)));

                var directiveToken = tokens[2] as DirectiveToken;
                Assert.IsNotNull(directiveToken);
                Assert.That(directiveToken.Name, Is.EqualTo("FOO"));

                var directiveParameters = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters.Count, Is.EqualTo(2));
                Assert.That(directiveParameters[0], Is.EqualTo("bar"));
                Assert.That(directiveParameters[1], Is.EqualTo("baz"));

                var documentContentToken = tokens.Find(t => t is DocumentContentToken) as DocumentContentToken;

                Assert.IsNotNull(documentContentToken);
                Assert.AreEqual(" \"foo\"", documentContentToken.Content.TrimEnd());
            }
        }

        [Test]
        public void ReadYamlDirective()
        {
            using (var reader = new StreamReader(Path.Combine("TestData", "example-6.14_yaml-directive.yml")))
            {
                var scanner = new Scanner(reader);
                var tokens = scanner.ReadTokens().ToList();

                Assert.That(tokens.Count, Is.EqualTo(8));

                var directiveToken = tokens.Find(t => t is DirectiveToken) as DirectiveToken;
                Assert.IsNotNull(directiveToken);
                Assert.That(directiveToken.Name, Is.EqualTo("YAML"));

                var directiveParameters = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters.Count, Is.EqualTo(1));
                Assert.That(directiveParameters[0], Is.EqualTo("1.3"));

                var documentContentToken = tokens.Find(t => t is DocumentContentToken) as DocumentContentToken;

                Assert.IsNotNull(documentContentToken);
                Assert.AreEqual("\"foo\"", documentContentToken.Content.TrimEnd());
            }
        }

        [Test]
        public void ReadInvalidRepeatedYamlDirective()
        {
            using (var reader = new StreamReader(Path.Combine("TestData", "example-6.15_invalid-repeated-yaml-directive.yml")))
            {
                var scanner = new Scanner(reader);
                var tokens = scanner.ReadTokens().ToList();

                Assert.That(tokens.Count, Is.EqualTo(9));

                var directiveTokens = tokens.FindAll(t => t is DirectiveToken).Select(t => t as DirectiveToken).ToList();
                Assert.AreEqual(2, directiveTokens.Count);

                Assert.IsNotNull(directiveTokens[0]);
                Assert.That(directiveTokens[0].Name, Is.EqualTo("YAML"));

                Assert.IsNotNull(directiveTokens[1]);
                Assert.That(directiveTokens[1].Name, Is.EqualTo("YAML"));

                var directiveParameters0 = directiveTokens[0].Parameters.ToList();
                Assert.That(directiveParameters0.Count, Is.EqualTo(1));
                Assert.That(directiveParameters0[0], Is.EqualTo("1.2"));

                var directiveParameters1 = directiveTokens[1].Parameters.ToList();
                Assert.That(directiveParameters1.Count, Is.EqualTo(1));
                Assert.That(directiveParameters1[0], Is.EqualTo("1.1"));

                var documentContentToken = tokens.Find(t => t is DocumentContentToken) as DocumentContentToken;

                Assert.IsNotNull(documentContentToken);
                Assert.AreEqual("foo", documentContentToken.Content.TrimEnd());
            }
        }

        [Test]
        public void ReadTagDirective()
        {
            using (var reader = new StreamReader(Path.Combine("TestData", "example-6.16_tag-directive.yml")))
            {
                var scanner = new Scanner(reader);
                var tokens = scanner.ReadTokens().ToList();

                Assert.That(tokens.Count, Is.EqualTo(8));

                var directiveToken = tokens.Find(t => t is DirectiveToken) as DirectiveToken;
                Assert.IsNotNull(directiveToken);
                Assert.That(directiveToken.Name, Is.EqualTo("TAG"));

                var directiveParameters = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters.Count, Is.EqualTo(2));
                Assert.That(directiveParameters[0], Is.EqualTo("!yaml!"));
                Assert.That(directiveParameters[1], Is.EqualTo("tag:yaml.org,2002:"));

                var documentContentToken = tokens.Find(t => t is DocumentContentToken) as DocumentContentToken;

                Assert.IsNotNull(documentContentToken);
                Assert.AreEqual("!yaml!str \"foo\"", documentContentToken.Content.TrimEnd());
            }
        }

        [Test]
        public void ReadInvalidRepeatedTagDirective()
        {
            using (var reader = new StreamReader(Path.Combine("TestData", "example-6.17_invalid-repeated-tag-directive.yml")))
            {
                var scanner = new Scanner(reader);
                var tokens = scanner.ReadTokens().ToList();

                Assert.That(tokens.Count, Is.EqualTo(9));

                var directiveTokens = tokens.FindAll(t => t is DirectiveToken).Select(t => t as DirectiveToken).ToList();
                Assert.AreEqual(2, directiveTokens.Count);

                Assert.IsNotNull(directiveTokens[0]);
                Assert.That(directiveTokens[0].Name, Is.EqualTo("TAG"));

                Assert.IsNotNull(directiveTokens[1]);
                Assert.That(directiveTokens[1].Name, Is.EqualTo("TAG"));

                var directiveParameters0 = directiveTokens[0].Parameters.ToList();
                Assert.That(directiveParameters0.Count, Is.EqualTo(2));
                Assert.That(directiveParameters0[0], Is.EqualTo("!"));
                Assert.That(directiveParameters0[1], Is.EqualTo("!foo"));

                var directiveParameters1 = directiveTokens[1].Parameters.ToList();
                Assert.That(directiveParameters1.Count, Is.EqualTo(2));
                Assert.That(directiveParameters1[0], Is.EqualTo("!"));
                Assert.That(directiveParameters1[1], Is.EqualTo("!foo"));

                var documentContentToken = tokens.Find(t => t is DocumentContentToken) as DocumentContentToken;

                Assert.IsNotNull(documentContentToken);
                Assert.AreEqual("bar", documentContentToken.Content.TrimEnd());
            }
        }

        [Test]
        public void ReadSecondaryTagHandle()
        {
            using (var reader = new StreamReader(Path.Combine("TestData", "example-6.19_secondary-tag-handle.yml")))
            {
                var scanner = new Scanner(reader);
                var tokens = scanner.ReadTokens().ToList();

                Assert.That(tokens.Count, Is.EqualTo(8));

                var directiveToken = tokens.Find(t => t is DirectiveToken) as DirectiveToken;
                Assert.IsNotNull(directiveToken);
                Assert.That(directiveToken.Name, Is.EqualTo("TAG"));

                var directiveParameters = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters.Count, Is.EqualTo(2));
                Assert.That(directiveParameters[0], Is.EqualTo("!!"));
                Assert.That(directiveParameters[1], Is.EqualTo("tag:example.com,2000:app/"));

                var documentContentToken = tokens.Find(t => t is DocumentContentToken) as DocumentContentToken;

                Assert.IsNotNull(documentContentToken);
                Assert.AreEqual("!!int 1 - 3 # Interval, not integer", documentContentToken.Content.TrimEnd());
            }
        }

        [Test]
        public void ReadTagHandles()
        {
            using (var reader = new StreamReader(Path.Combine("TestData", "example-6.20_tag-handles.yml")))
            {
                var scanner = new Scanner(reader);
                var tokens = scanner.ReadTokens().ToList();

                Assert.That(tokens.Count, Is.EqualTo(8));

                var directiveToken = tokens.Find(t => t is DirectiveToken) as DirectiveToken;
                Assert.IsNotNull(directiveToken);
                Assert.That(directiveToken.Name, Is.EqualTo("TAG"));

                var directiveParameters = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters.Count, Is.EqualTo(2));
                Assert.That(directiveParameters[0], Is.EqualTo("!e!"));
                Assert.That(directiveParameters[1], Is.EqualTo("tag:example.com,2000:app/"));

                var documentContentToken = tokens.Find(t => t is DocumentContentToken) as DocumentContentToken;

                Assert.IsNotNull(documentContentToken);
                Assert.AreEqual("!e!foo \"bar\"", documentContentToken.Content.TrimEnd());
            }
        }

        [Test]
        public void ReadLocalTagPrefix()
        {
            using (var reader = new StreamReader(Path.Combine("TestData", "example-6.21_local-tag-prefix.yml")))
            {
                var scanner = new Scanner(reader);
                var tokens = scanner.ReadTokens().ToList();

                Assert.That(tokens.Count, Is.EqualTo(14));
                Assert.IsInstanceOf(typeof(StreamStartToken), tokens[0]);
                Assert.IsInstanceOf(typeof(StreamEndToken), tokens[13]);

                var doc1Tokens = tokens.Where((_, i) => i >= 1 && i < 7).ToList();
                var doc2Tokens = tokens.Where((_, i) => i >= 7 && i < 13).ToList();

                var doc1DirectiveToken = doc1Tokens.First(t => t is DirectiveToken) as DirectiveToken;
                Assert.IsNotNull(doc1DirectiveToken);
                Assert.That(doc1DirectiveToken.Name, Is.EqualTo("TAG"));

                var doc2DirectiveToken = doc2Tokens.First(t => t is DirectiveToken) as DirectiveToken;
                Assert.IsNotNull(doc2DirectiveToken);
                Assert.That(doc2DirectiveToken.Name, Is.EqualTo("TAG"));

                var doc1DirectiveParameters = doc1DirectiveToken.Parameters.ToList();
                Assert.That(doc1DirectiveParameters.Count, Is.EqualTo(2));
                Assert.That(doc1DirectiveParameters[0], Is.EqualTo("!m!"));
                Assert.That(doc1DirectiveParameters[1], Is.EqualTo("!my-"));

                var doc2DirectiveParameters = doc2DirectiveToken.Parameters.ToList();
                Assert.That(doc2DirectiveParameters.Count, Is.EqualTo(2));
                Assert.That(doc2DirectiveParameters[0], Is.EqualTo("!m!"));
                Assert.That(doc2DirectiveParameters[1], Is.EqualTo("!my-"));

                var doc1ContentToken = doc1Tokens.First(t => t is DocumentContentToken) as DocumentContentToken;
                Assert.IsNotNull(doc1ContentToken);
                Assert.AreEqual("!m!light fluorescent", doc1ContentToken.Content.TrimEnd());

                var doc2ContentToken = doc2Tokens.First(t => t is DocumentContentToken) as DocumentContentToken;
                Assert.IsNotNull(doc2ContentToken);
                Assert.AreEqual("!m!light green", doc2ContentToken.Content.TrimEnd());
            }
        }

        [Test]
        public void ReadGlobalTagPrefix()
        {
            using (var reader = new StreamReader(Path.Combine("TestData", "example-6.22_global-tag-prefix.yml")))
            {
                var scanner = new Scanner(reader);
                var tokens = scanner.ReadTokens().ToList();

                Assert.That(tokens.Count, Is.EqualTo(8));

                var directiveToken = tokens.Find(t => t is DirectiveToken) as DirectiveToken;
                Assert.IsNotNull(directiveToken);
                Assert.That(directiveToken.Name, Is.EqualTo("TAG"));

                var directiveParameters = directiveToken.Parameters.ToList();
                Assert.That(directiveParameters.Count, Is.EqualTo(2));
                Assert.That(directiveParameters[0], Is.EqualTo("!e!"));
                Assert.That(directiveParameters[1], Is.EqualTo("tag:example.com,2000:app/"));

                var documentContentToken = tokens.Find(t => t is DocumentContentToken) as DocumentContentToken;

                Assert.IsNotNull(documentContentToken);
                Assert.AreEqual("- !e!foo \"bar\"", documentContentToken.Content.TrimEnd());
            }
        }

        [Test, ExpectedException(typeof(Exception), ExpectedMessage = "Directives end marker missing")]
        public void ReadDirectivesEndTagMissing()
        {
            using (var reader = new StreamReader(Path.Combine("TestData", "directives-end-tag-missing.yml")))
            {
                var tokens = new Scanner(reader).ReadTokens().ToList();

                Assert.IsNotNull(tokens);
                Assert.Fail("This test should fail!");
            }
        }
    }
}
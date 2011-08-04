using System.Collections.Generic;
using System.IO;
using YamlSharp.Tokens;
using System;

namespace YamlSharp
{
	public class Scanner
	{
		private readonly StreamReader reader;

		public string CurrentLine { get; private set; }
		public int Indent { get; private set; }

		public Scanner(StreamReader reader)
		{
			this.reader = reader;
		}

		public IEnumerable<Token> ReadTokens()
		{
			// [211] l-yaml-stream ::= l-document-prefix* l-any-document? ( l-document-suffix+ l-document-prefix* l-any-document? | l-document-prefix* l-explicit-document? )*

			yield return new StreamStartToken(0, 0);

			while (!reader.EndOfStream)
			{
				ReadDocumentPrefix();
				foreach (var token in ReadDocument())
					yield return token;
			}

			yield return new StreamEndToken(0, 0);
		}

		private void ReadDocumentPrefix()
		{
			// [202] l-document-prefix ::= c-byte-order-mark? l-comment*
			// [78] l-comment ::= s-separate-in-line c-nb-comment-text? b-comment
			// [66] s-separate-in-line ::= s-white+ | /* Start of line */
			// [31] s-space ::= #x20 /* SP */
			// [32] s-tab ::= #x9 /* TAB */
			// [33] s-white ::= s-space | s-tab
			// [75] c-nb-comment-text ::= “#” nb-char*
			// [76] b-comment ::= b-non-content | /* End of file */
			// [30] b-non-content ::= b-break
			// [28] b-break ::= ( b-carriage-return b-line-feed ) | b-carriage-return | b-line-feed
			// [27] nb-char ::= c-printable - b-char - c-byte-order-mark
			// [24] b-line-feed ::= #xA /* LF */
			// [25] b-carriage-return ::= #xD /* CR */
			// [26] b-char ::= b-line-feed | b-carriage-return
			// [1] c-printable ::= #x9 | #xA | #xD | [#x20-#x7E] /* 8 bit */ | #x85 | [#xA0-#xD7FF] | [#xE000-#xFFFD] /* 16 bit */ | [#x10000-#x10FFFF] /* 32 bit */
			// [2] nb-json ::= #x9 | [#x20-#x10FFFF] (inside quoted scalars)
			// [3] c-byte-order-mark ::= #xFEFF

			// TODO : BOM - maybe custom stream reader for yaml which recognizes byte order marks inside stream

			string line;
			while ((line = reader.ReadLine()) != null)
			{
				var position = 0;
				CurrentLine = line;

				// s-separate-in-line
				while (position < line.Length && (line[position] == ' ' || line[position] == '\t'))
					position++;
				if (position >= line.Length)
					continue;

				if (line[position] != '#')
					break;
			}
		}

		private IEnumerable<Token> ReadDocument()
		{
			// [210] l-any-document ::= l-directive-document | l-explicit-document | l-bare-document

			// [209] l-directive-document ::= l-directive+ l-explicit-document
			// [208] l-explicit-document ::= c-directives-end ( l-bare-document | ( e-node s-l-comments ) )
			// [207] l-bare-document ::= s-l+block-node(-1,block-in) /* Excluding c-forbidden content */

			// [203] c-directives-end ::= “-” “-” “-”
			// [204] c-document-end ::= “.” “.” “.”
			// [205] l-document-suffix ::= c-document-end s-l-comments
			// [206] c-forbidden ::= /* Start of line */ ( c-directives-end | c-document-end ) ( b-char | s-white | /* End of file */ )



			yield break;
		}
	}
}
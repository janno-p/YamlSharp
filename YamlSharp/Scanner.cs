using System.Collections.Generic;
using System.IO;
using YamlSharp.Tokens;
using System;
using System.Text.RegularExpressions;

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

            yield return new DirectivesStartToken(0, 0);

            foreach (var directiveToken in ReadDirectives())
                yield return directiveToken;

            // Directives may end with:
            //  * explicit directives end marker
            //  * start of block node

            yield return new DirectivesEndToken(0, 0);



			// [209] l-directive-document ::= l-directive+ l-explicit-document
			// [208] l-explicit-document ::= c-directives-end ( l-bare-document | ( e-node s-l-comments ) )
			// [207] l-bare-document ::= s-l+block-node(-1,block-in) /* Excluding c-forbidden content */

			// [203] c-directives-end ::= “-” “-” “-”
			// [204] c-document-end ::= “.” “.” “.”
			// [205] l-document-suffix ::= c-document-end s-l-comments
			// [206] c-forbidden ::= /* Start of line */ ( c-directives-end | c-document-end ) ( b-char | s-white | /* End of file */ )



			yield break;
		}

        private IEnumerable<Token> ReadDirectives()
        {
            var hasDirectives = false;
            while (CurrentLine[0] == '%')
            {
                yield return ReadDirective();
                hasDirectives = true;
            }

            if (CurrentLine.StartsWith("---"))
                CurrentLine = CurrentLine.Length > 3 ? CurrentLine.Substring(3) : reader.ReadLine();
            else if (hasDirectives)
                throw new Exception("Directives end marker missing");
        }

        private Token ReadDirective()
        {
            // [82] l-directive ::= “%” ( ns-yaml-directive | ns-tag-directive | ns-reserved-directive ) s-l-comments

            CurrentLine = CurrentLine.Substring(1);
            if (CurrentLine.Length == 0 || CurrentLine[0] == ' ' || CurrentLine[0] == '\t' || Char.IsControl(CurrentLine[0]))
                throw new Exception("Directive name expected");

            Token token;

            var splits = CurrentLine.Split(' ', '\t');
            if (splits[0].Equals("YAML"))
            {
                // [86] ns-yaml-directive ::= “Y” “A” “M” “L” s-separate-in-line ns-yaml-version
                // [87] ns-yaml-version ::= ns-dec-digit+ “.” ns-dec-digit+
                // [35] ns-dec-digit ::= [#x30-#x39] /* 0-9 */

                if (splits.Length == 1 || (splits.Length > 1 && !Regex.IsMatch(splits[1], @"^\d+\.\d+$")))
                    throw new Exception("YAML version expected");
                if (splits.Length > 2 && splits[2][0] != '#')
                    throw new Exception("Unexpected token");
                token = new DirectiveToken(0, 0, splits[0], splits[1]);
            }
            else if (splits[0].Equals("TAG"))
            {
                // [88] ns-tag-directive ::= “T” “A” “G” s-separate-in-line c-tag-handle s-separate-in-line ns-tag-prefix

                // [89] c-tag-handle ::= c-named-tag-handle | c-secondary-tag-handle | c-primary-tag-handle
                // [92] c-named-tag-handle ::= “!” ns-word-char+ “!”
                // [38] ns-word-char ::= ns-dec-digit | ns-ascii-letter | “-”
                // [37] ns-ascii-letter ::= [#x41-#x5A] /* A-Z */ | [#x61-#x7A] /* a-z */
                // [91] c-secondary-tag-handle ::= “!” “!”
                // [90] c-primary-tag-handle ::= “!”
                if (splits.Length == 1 || (splits.Length > 1 && !Regex.IsMatch(splits[1], @"^!(([A-Za-z0-9-]+!)|!)?$")))
                    throw new Exception("TAG handle expected");

                // [93] ns-tag-prefix ::= c-ns-local-tag-prefix | ns-global-tag-prefix

                // [94] c-ns-local-tag-prefix ::= “!” ns-uri-char*
                // [39] ns-uri-char ::= “%” ns-hex-digit ns-hex-digit | ns-word-char | “#” | “;” | “/” | “?” | “:” | “@” | “&” | “=” | “+” | “$” | “,” | “_” | “.” | “!” | “~” | “*” | “'” | “(” | “)” | “[” | “]”
                // [36] ns-hex-digit ::= ns-dec-digit | [#x41-#x46] /* A-F */ | [#x61-#x66] /* a-f */

                // [95] ns-global-tag-prefix ::= ns-tag-char ns-uri-char*
                // [40] ns-tag-char ::= ns-uri-char - “!” - c-flow-indicator
                // [23] c-flow-indicator ::= “,” | “[” | “]” | “{” | “}”
                if (splits.Length > 2 && !Regex.IsMatch(splits[2], @"^((!((%[0-9A-Fa-f]{2})|[A-Za-z0-9-#;/?:@&=+$,_.!\-*'()[\]])*)|(((%[0-9A-Fa-f]{2})|[A-Za-z0-9-#;/?:@&=+$_.\-*'()])((%[0-9A-Fa-f]{2})|[A-Za-z0-9-#;/?:@&=+$,_.!\-*'()[\]])*))$"))
                    throw new Exception("TAG prefix expected");
                if (splits.Length > 3 && splits[3][0] != '#')
                    throw new Exception("Unexpected token");

                token = new DirectiveToken(0, 0, splits[0], splits[1], splits[2]);
            }
            else
            {
                // [83] ns-reserved-directive ::= ns-directive-name ( s-separate-in-line ns-directive-parameter )*
                // [84] ns-directive-name ::= ns-char+
                // [85] ns-directive-parameter ::= ns-char+
                // [34] ns-char ::= nb-char - s-white

                var parameters = new List<string>();

                var i = 0;
                for ( ; i < splits.Length; i++)
                {
                    if (splits[i][0] == '#')
                        break;

                    foreach (var c in splits[i])
                        if (Char.IsControl(c))
                            throw new Exception(string.Format("Invalid character in directive {0}", i == 0 ? "name" : "parameter"));

                    if (i != 0)
                        parameters.Add(splits[i]);
                }

                token = new DirectiveToken(0, 0, splits[0], parameters.ToArray());
            }

            // [79] s-l-comments ::= ( s-b-comment | /* Start of line */ ) l-comment*
            // [77] s-b-comment ::= ( s-separate-in-line c-nb-comment-text? )? b-comment

            while ((CurrentLine = reader.ReadLine()) != null)
            {
                var position = 0;

                // s-separate-in-line
                while (position < CurrentLine.Length && (CurrentLine[position] == ' ' || CurrentLine[position] == '\t'))
                    position++;

                if (position >= CurrentLine.Length)
                    continue;

                if (CurrentLine[position] != '#')
                    break;
            }

            return token;
        }
	}
}
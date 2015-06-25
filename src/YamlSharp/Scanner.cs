using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlSharp.Tokens;
using System;
using System.Text.RegularExpressions;

namespace YamlSharp
{
    public class Scanner
    {
        private class Cursor
        {
            public int Row { get; set; }
            public int Column { get; set; }

            public void NextLine()
            {
                Row++;
                Column = 0;
            }

            public void Reset()
            {
                Row = 0;
                Column = 0;
            }
        }

        private readonly StreamReader reader;
        private readonly Cursor cursor = new Cursor();

        private string currentLine;

        private readonly IList<int> identLevels = new List<int> { -1 };

        public Scanner(StreamReader reader)
        {
            this.reader = reader;
        }

        public IEnumerable<Token> ReadTokens()
        {
            // [211] l-yaml-stream ::= l-document-prefix* l-any-document? ( l-document-suffix+ l-document-prefix* l-any-document? | l-document-prefix* l-explicit-document? )*

            currentLine = string.Empty;
            cursor.Reset();

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

            SkipWhitespaceAndComments();
        }

        private IEnumerable<Token> ReadDocument()
        {
            // [210] l-any-document ::= l-directive-document | l-explicit-document | l-bare-document

            yield return new DirectivesStartToken(0, 0);

            // [209] l-directive-document ::= l-directive+ l-explicit-document

            foreach (var directiveToken in ReadDirectives())
                yield return directiveToken;

            // Directives may end with:
            //  * explicit directives end marker
            //  * start of block node

            yield return new DirectivesEndToken(0, 0);

            yield return new DocumentStartToken(0, 0);

            // [208] l-explicit-document ::= c-directives-end ( l-bare-document | ( e-node s-l-comments ) )
            foreach (var documentToken in ReadDocumentContent())
                yield return documentToken;

            yield return new DocumentEndToken(0, 0);
        }

        private IEnumerable<Token> ReadDocumentContent()
        {
            var contentBuilder = new StringBuilder();

            // [208] l-explicit-document ::= c-directives-end ( l-bare-document | ( e-node s-l-comments ) )

            while (currentLine != null)
            {
                if (currentLine.StartsWith("---") && (currentLine.Length == 3 || (currentLine[3] == ' ' || currentLine[3] == '\t')))
                    break;

                // [204] c-document-end ::= “.” “.” “.”
                // [205] l-document-suffix ::= c-document-end s-l-comments
                // [206] c-forbidden ::= /* Start of line */ ( c-directives-end | c-document-end ) ( b-char | s-white | /* End of file */ )
                var documentEnd = false;
                while (currentLine != null && currentLine.StartsWith("...") && (currentLine.Length == 3 || (currentLine[3] == ' ' || currentLine[3] == '\t')))
                {
                    currentLine = currentLine.Substring(3);
                    if (!SkipWhitespaceAndComments())
                        throw new Exception("Unexpected token");
                    documentEnd = true;
                }

                if (documentEnd)
                    break;

                // Read block content
                // [207] l-bare-document ::= s-l+block-node(-1,block-in) /* Excluding c-forbidden content */
                // [196] s-l+block-node(n,c) ::= s-l+block-in-block(n,c) | s-l+flow-in-block(n)
                // [198] s-l+block-in-block(n,c) ::= s-l+block-scalar(n,c) | s-l+block-collection(n,c)
                // [201] seq-spaces(n,c) ::= c = block-out => n-1
                //                           c = block-in  => n
                // [80] s-separate(n,c) ::= c = block-out => s-separate-lines(n)
                //                          c = block-in  => s-separate-lines(n)
                //                          c = flow-out  => s-separate-lines(n)
                //                          c = flow-in   => s-separate-lines(n)
                //                          c = block-key => s-separate-in-line
                //                          c = flow-key  => s-separate-in-line
                // [81] s-separate-lines(n) ::= ( s-l-comments s-flow-line-prefix(n) ) | s-separate-in-line
                // [69] s-flow-line-prefix(n) ::= s-indent(n) s-separate-in-line?
                // [63] s-indent(n) ::= s-space &times; n
                // [197] s-l+flow-in-block(n) ::= s-separate(n+1,flow-out) ns-flow-node(n+1,flow-out) s-l-comments

                // [199] s-l+block-scalar(n,c) ::= s-separate(n+1,c) ( c-ns-properties(n+1,c) s-separate(n+1,c) )? ( c-l+literal(n) | c-l+folded(n) )
                // [200] s-l+block-collection(n,c) ::= ( s-separate(n+1,c) c-ns-properties(n+1,c) )? s-l-comments ( l+block-sequence(seq-spaces(n,c)) | l+block-mapping(n) )
                // [161] ns-flow-node(n,c) ::= c-ns-alias-node | ns-flow-content(n,c) | ( c-ns-properties(n,c) ( ( s-separate(n,c) ns-flow-content(n,c) ) | e-scalar ) )
                
                contentBuilder.AppendLine(currentLine);
                currentLine = reader.ReadLine();
            }

            yield return new DocumentContentToken(0, 0, contentBuilder.ToString());
        }

        private IEnumerable<Token> ReadDirectives()
        {
            var hasDirectives = false;
            while (currentLine[0] == '%')
            {
                yield return ReadDirective();
                hasDirectives = true;
            }

            // [203] c-directives-end ::= “-” “-” “-”

            if (currentLine.StartsWith("---") && (currentLine.Length == 3 || (currentLine.Length > 3 && currentLine[3] == ' ' || currentLine[3] == '\t')))
            {
                currentLine = currentLine.Substring(3);
                SkipWhitespaceAndComments();
            }
            else if (hasDirectives)
                throw new Exception("Directives end marker missing");
        }

        private Token ReadDirective()
        {
            // [82] l-directive ::= “%” ( ns-yaml-directive | ns-tag-directive | ns-reserved-directive ) s-l-comments

            currentLine = currentLine.Substring(1);
            if (currentLine.Length == 0 || currentLine[0] == ' ' || currentLine[0] == '\t' || Char.IsControl(currentLine[0]))
                throw new Exception("Directive name expected");

            Token token;

            var splits = currentLine.Split(' ', '\t');
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

                    if (splits[i].Any(Char.IsControl))
                        throw new Exception(string.Format("Invalid character in directive {0}", i == 0 ? "name" : "parameter"));

                    if (i != 0)
                        parameters.Add(splits[i]);
                }

                token = new DirectiveToken(0, 0, splits[0], parameters.ToArray());
            }

            // [79] s-l-comments ::= ( s-b-comment | /* Start of line */ ) l-comment*
            // [77] s-b-comment ::= ( s-separate-in-line c-nb-comment-text? )? b-comment

            currentLine = reader.ReadLine();
            SkipWhitespaceAndComments();

            return token;
        }

        private bool SkipWhitespaceAndComments()
        {
            var changedRow = false;
            while (currentLine != null)
            {
                var position = 0;
                while (position < currentLine.Length && (currentLine[position] == ' ' || currentLine[position] == '\t'))
                    position++;
                if (position < currentLine.Length && currentLine[position] != '#')
                    break;
                currentLine = reader.ReadLine();
                changedRow = true;
            }
            return changedRow;
        }
    }
}
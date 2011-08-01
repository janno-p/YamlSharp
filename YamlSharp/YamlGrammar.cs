using Irony.Parsing;

namespace YamlSharp
{
    [Language("YAML", "1.2", "Human friendly data serialization standard for all programming languages")]
    public class YamlGrammar : Grammar
    {
        public YamlGrammar()
        {
            var documentPrefix = new NonTerminal("l-document-prefix");
            // [202] l-document-prefix ::= c-byte-order-mark? l-comment*

            var directivesEnd = ToTerm("---", "c-directives-end"); // (203)
            var documentEnd = ToTerm("...", "c-document-end"); // (204)

            var documentSuffix = new NonTerminal("l-document-suffix");
            // [205] l-document-suffix ::= c-document-end s-l-comments

            var forbidden = new NonTerminal("c-forbidden");
            // [206] c-forbidden ::= /* Start of line */ ( c-directives-end | c-document-end ) ( b-char | s-white | /* End of file */ )

            var bareDocument = new NonTerminal("l-bare-document");
            // [207] l-bare-document ::= s-l+block-node(-1,block-in) /* Excluding c-forbidden content */

            var explicitDocument = new NonTerminal("l-explicit-document");
            // [208] l-explicit-document ::= c-directives-end ( l-bare-document | ( e-node s-l-comments ) )

            var directiveDocument = new NonTerminal("l-directive-document");
            // [209] l-directive-document ::= l-directive+ l-explicit-document

            var anyDocument = new NonTerminal("l-any-document");
            // [210] l-any-document ::= l-directive-document | l-explicit-document | l-bare-document

            var stream = new NonTerminal("l-yaml-stream");
            // [211] l-yaml-stream ::= l-document-prefix* l-any-document? ( l-document-suffix+ l-document-prefix* l-any-document? | l-document-prefix* l-explicit-document? )*

            var line = new NonTerminal("test");
            line.Rule = directivesEnd + NewLine + documentEnd + NewLine;

            stream.Rule = MakeStarRule(stream, line);

            Root = stream;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;

namespace YamlSharp
{
	public class Spec
	{
		// e-	A production matching no characters
		// c-	A production starting and ending with a special character
		// b-	A production matching single line break
		// nb-	A production starting and ending with a non-break character
		// s-	A production starting and ending with a white space character
		// ns-	A production starting and ending with a non-space character
		// l-	A production matching complete line(s)
		// X-Y-	A production string with an x- character and ending with a y- character, where x- and y- are any of the above prefixes
		// X+,X-Y+	A production as above, with the additional property that the matched content identation level is greater than the specified n parameter
		
		#region Character Set (1-2)
		
		/// <summary>
		/// [1] c-printable
		/// </summary>
		public bool IsPrintable(char c)
		{
			return c == 0x9 || c == 0xA || c == 0xD || (c >= 0x20 && c <= 0x7E) || /* 8 bit */
				c == 0x85 || (c >= 0xA0 && c <= 0xD7FF) || 	(c >= 0xE000 && c <= 0xFFFD) || /* 16 bit */
				(c >= 0x10000 && c <= 0x10FFFF); /* 32 bit */
		}
		
		/// <summary>
		/// [2] nb-json
		/// </summary>
		public bool IsJson(char c)
		{
			return c == 0x9 || (c >= 0x20 && c <= 0x10FFFF);
		}
		
		#endregion
		
		#region Character Encodings (3)
		
		/// <summary>
		/// [3] c-byte-order-mark
		/// </summary>
		public bool IsByteOrderMark(char c)
		{
			return c == 0xFEFF;
		}
		
		#endregion
		
		#region Indicator Characters (4-23)
		
		/// <summary>
		/// [4] c-sequence-entry
		/// </summary>
		public const char SequenceEntry = '-'; // 0x2D
		
		/// <summary>
		/// [5] c-mapping-key
		/// </summary>
		public const char MappingKey = '?'; // 0x3F
		
		/// <summary>
		/// [6] c-mapping-value
		/// </summary>
		public const char MappingValue = ':'; // 0x3A
		
		/// <summary>
		/// [7] c-collect-entry
		/// </summary>
		public const char CollectEntry = ','; // 0x2C
		
		/// <summary>
		/// [8] c-sequence-start
		/// </summary>
		public const char SequenceStart = '['; // 0x5B
		
		/// <summary>
		/// [9] c_sequence-end
		/// </summary>
		public const char SequenceEnd = ']'; // 0x5D
		
		/// <summary>
		/// [10] c-mapping-start
		/// </summary>
		public const char MappingStart = '{'; // 0x7B
		
		/// <summary>
		/// [11] c-mapping-end
		/// </summary>
		public const char MappingEnd = '}'; // 0x7D
		
		/// <summary>
		/// [12] c-comment
		/// </summary>
		public const char Comment = '#'; // 0x23
		
		/// <summary>
		/// [13] c-anchor
		/// </summary>
		public const char Anchor = '&'; // 0x26
		
		/// <summary>
		/// [14] c-alias
		/// </summary>
		public const char Alias = '*'; // 0x2A
		
		/// <summary>
		/// [15] c-tag
		/// </summary>
		public const char Tag = '!'; // 0x21
		
		/// <summary>
		/// [16] c-literal
		/// </summary>
		public const char Literal = '|'; // 0x7C
		
		/// <summary>
		/// [17] c-folded
		/// </summary>
		public const char Folded = '>'; // 0x3E
		
		/// <summary>
		/// [18] c-single-quote
		/// </summary>
		public const char SingleQuote = '\''; // 0x27
		
		/// <summary>
		/// [19] c-double-quote
		/// </summary>
		public const char DoubleQuote = '"'; // 0x22
		
		/// <summary>
		/// [20] c-directive
		/// </summary>
		public const char Directive = '%'; // 0x25
		
		/// <summary>
		/// [21] c-reserved
		/// </summary>
		public const string Reserved = "@`"; // 0x40 && 0x60
		
		/// <summary>
		/// [22] c-indicator
		/// </summary>
		public const string Indicator = "-?:,[]{}#&*!|>'\"%@`";
		
		/// <summary>
		/// [23] c-flow-indicator
		/// </summary>
		public const string FlowIndicator = ",[]{}";
		
		#endregion
		
		#region Line Break Characters (24-30)
		
		/// <summary>
		/// [24] b-line-feed
		/// </summary>
		public const char LineFeed = '\n'; // 0xA (LF)
		
		/// <summary>
		/// [25] b-carriage-return
		/// </summary>
		public const char CarriageReturn = '\r'; // 0xD (CR)
		
		/// <summary>
		/// [26] b-char
		/// </summary>
		public const string BreakChar = "\n\r";
		
		/// <summary>
		/// [27] nb-char
		/// </summary>
		public bool IsNonBreakChar(char c)
		{
			return IsPrintable(c) && BreakChar.IndexOf(c) < 0 && !IsByteOrderMark(c);
		}
		
		/// <summary>
		/// [28] b-break
		/// </summary>
		public bool IsLineBreak(string str)
		{
			return str.StartsWith(BreakChar) || str[0] == CarriageReturn || str[0] == LineFeed;
		}
		
		/// <summary>
		/// [29] b-as-line-feed
		/// </summary>
		public bool IsAsLineFeed(string str)
		{
			return IsLineBreak(str);
		}
		
		/// <summary>
		/// [30] b-non-content
		/// </summary>
		public bool IsNonContent(string str)
		{
			return IsLineBreak(str);
		}
		
		#endregion
		
		#region White Space Characters (31-34)
		
		/// <summary>
		/// [31] s-space
		/// </summary>
		public const char Space = ' '; // 0x20 (SP)
		
		/// <summary>
		/// [32] s-tab
		/// </summary>
		public const char Tab = '\t'; // 0x9
		
		/// <summary>
		/// [33] s-white
		/// </summary>
		public const string WhiteSpace = " \t";
		
		/// <summary>
		/// [34] ns-char
		/// </summary>
		public bool IsNonSpace(char c)
		{
			return IsNonBreakChar(c) && WhiteSpace.IndexOf(c) < 0;
		}
		
		#endregion
		
		#region Miscellaneous Characters (35-40)
		
		/// <summary>
		/// [35] ns-dec-digit
		/// </summary>
		public bool IsDecimalDigit(char c)
		{
			return c >= 0x30 && c <= 0x39;
		}
		
		/// <summary>
		/// [36] ns-hex-digit
		/// </summary>
		public bool IsHexadecimalDigit(char c)
		{
			return IsDecimalDigit(c) || (c >= 0x41 && c <= 0x46);
		}
		
		/// <summary>
		/// [37] ns-ascii-letter
		/// </summary>
		public bool IsAsciiLetter(char c)
		{
			return (c >= 0x41 && c <= 0x5A) || (c >= 0x61 && c <= 0x7A); // A-Za-z
		}
		
		/// <summary>
		/// [38] ns-word-char
		/// </summary>
		public bool IsWordChar(char c)
		{
			return IsDecimalDigit(c) || IsAsciiLetter(c) || c == '-';
		}
		
		/// <summary>
		/// [39] ns-uri-char
		/// </summary>
		public bool IsUriChar(string str)
		{
			if (str[0] == '%' && IsHexadecimalDigit(str[1]) && IsHexadecimalDigit(str[2]))
				return true;
			return IsWordChar(str[0]) || "#;/?:@&=+$,_.!~*'()[]".IndexOf(str[0]) >= 0;
		}
		
		/// <summary>
		/// [40] ns-tag-char
		/// </summary>
		public bool IsTagChar(string str)
		{
			return IsUriChar(str) && str[0] != '!' && FlowIndicator.IndexOf(str[0]) < 0;
		}
		
		#endregion
		
		#region Escaped Characters (41-62)
		
		/// <summary>
		/// [41] c-escape
		/// </summary>
		public const char Escape = '\\';
		
		/// <summary>
		/// [42] ns-esc-null
		/// </summary>
		public const char EscapeNull = '0';
		
		/// <summary>
		/// [43] ns-esc-bell
		/// </summary>
		public const char EscapeBell = 'a';
		
		/// <summary>
		/// [44] ns-esc-backspace
		/// </summary>
		public const char EscapeBackspace = 'b';
		
		/// <summary>
		/// [45] ns-esc-horizontal-tab
		/// </summary>
		public const char EscapeHorizontalTab = 't';
		
		/// <summary>
		/// [46] ns-esc-line-feed
		/// </summary>
		public const char EscapeLineFeed = 'n';
		
		/// <summary>
		/// [47] ns-esc-vertical-tab
		/// </summary>
		public const char EscapeVerticalTab = 'v';
		
		/// <summary>
		/// [48] ns-esc-form-feed
		/// </summary>
		public const char EscapeFormFeed = 'f';
		
		/// <summary>
		/// [49] ns-esc-carriage-return
		/// </summary>
		public const char EscapeCarriageReturn = 'r';
		
		/// <summary>
		/// [50] ns-esc-escape
		/// </summary>
		public const char EscapeEscape = 'e';
		
		/// <summary>
		/// [51] ns-esc-space
		/// </summary>
		public const char EscapeSpace = ' '; // 0x20
		
		/// <summary>
		/// [52] ns-esc-double-quote
		/// </summary>
		public const char EscapeDoubleQuote = '"';
		
		/// <summary>
		/// [53] ns-esc-slash
		/// </summary>
		public const char EscapeSlash = '/';
		
		/// <summary>
		/// [54] ns-esc-backslash
		/// </summary>
		public const char EscapeBackslash = '\\';
		
		/// <summary>
		/// [55] ns-esc-next-line
		/// </summary>
		public const char EscapeNextLine = 'N';
		
		/// <summary>
		/// [56] ns-esc-non-breaking-space
		/// </summary>
		public const char EscapeNonBreakingSpace = '_';
		
		/// <summary>
		/// [57] ns-esc-line-separator
		/// </summary>
		public const char EscapeLineSeparator = 'L';
		
		/// <summary>
		/// [58] ns-esc-paragraph-separator
		/// </summary>
		public const char EscapeParagrahpSeparator = 'P';
		
		/// <summary>
		/// [59] ns-esc-8-bit
		/// </summary>
		public const char Escape8Bit = 'x';
		
		/// <summary>
		/// [60] ns-esc-16-bit
		/// </summary>
		public const char Escape16Bit = 'u';
		
		/// <summary>
		/// [61] ns-esc-32-bit
		/// </summary>
		public const char Escape32Bit = 'U';
		
		/// <summary>
		/// [62] c-ns-esc-char
		/// </summary>
		public bool IsEscapedCharacter(string str)
		{
			return str[0] == Escape && "0abtnvfre \"/\\N_LPxuU".IndexOf(str[1]) >= 0;
		}
		
		#endregion
		
		#region 6. Basic Structures
		
		// [63] s-indent(n) ::= s-space * n
		// [64] s-indent(<n) ::= s-space * m /* Where m < n */
		// [65] s-indent(<=n) ::= s-space * m /* Where m <= n */
		// [66] s-separate-in-line ::= s-white+ | /* Start of line */
		// [67] s-line-prefix(n,c) ::= c = block-out => s-block-line-prefix(n)
		//                             c = block-in  => s-block-line-prefix(n)
		//                             c = flow-out  => s-flow-line-prefix(n)
		//                             c = flow-in   => s-flow-line-prefix(n)
		// [68] s-block-line-prefix(n) ::= s-indent(n)
		// [69] s-flow-line-prefix ::= s-indent(n) s-separate-in-line?
		// [70] l-empty(n,c) ::= ( s-line-prefix(n,c) | s-indent(<n) ) b-as-line-feed
		// [71] b-l-trimmed(n,c) ::= b-non-content l-empty(n,c)+
		// [72] b-as-space ::= b-break
		// [73] b-l-folded(n,c) ::= b-l-trimmed(n,c) | b-as-space
		// [74] s-flow-folded(n) ::= s-separate-in-line? b-l-folded(n,flow-in) s-flow-line-prefix(n)
		// [75] c-nb-comment-text ::= "#" nb-char*
		// [76] b-comment ::= b-non-content | /* End of file */
		// [77] s-b-comment ::= ( s-separate-in-line c-nb-comment-text? )? b-comment
		// [78] l-comment ::= s-separate-in-line c-nb-comment-text? b-comment
		// [79] s-l-comments ::= ( s-b-comment | /* Start of line */ ) l-comment*
		// [80] s-separate(n,c) ::= c = block-out => s-separate-lines(n)
		//                          c = block-in  => s-separate-lines(n)
		//                          c = flow-out  => s-separate-lines(n)
		//                          c = flow-in   => s-separate-lines(n)
		//                          c = block-key => s-separate-in-line
		//                          c = flow-key  => s-separate-in-line
		// [81] s-separate-lines(n) ::= ( s-l-comments s-flow-line-prefix(n) ) | s-separate-in-line
		// [82] l-directive ::= "%" ( ns-yaml-directive | ns-tag-directive | ns-reserved-directive ) s-l-comments
		// [83] ns-reserved-directive ::= ns-directive-name ( s-separate-in-line ns-directive-parameter )*
		// [84] ns-directive-name ::= ns-char+
		// [85] ns-directive-parameter ::= ns-char+
		// [86] ns-yaml-directive ::= "Y" "A" "M" "L" s-separate-in-line ns-yaml-version
		// [87] ns-yaml-version ::= ns-dec-digit+ "." ns-dec-digit+
		// [88] ns-tag-directive ::= "T" "A" "G" s-separate-in-line c-tag-handle s-separate-in-line ns-tag-prefix
		// [89] c-tag-handle ::= c-named-tag-handle | c-secondary-tag-handle | c-primary-tag-handle
		// [90] c-primary-tag-handle ::= "!"
		// [91] c-secondary-tag-handle ::= "!" "!"
		// [92] c-named-tag-handle ::= "!" ns-word-char+ "!"
		// [93] ns-tag-prefix ::= c-ns-local-tag-prefix | ns-global-tag-prefix
		// [94] c-ns-local-tag-prefix ::= "!" ns-uri-char*
		// [95] ns-global-tag-prefix ::= ns-tag-char ns-uri-char*
		// [96] c-ns-properties(n,c) ::= ( c-ns-tag-property ( s-separate(n,c) c-ns-anchor-property )? ) | ( c-ns-anchor-property ( s-separate(n,c) c-ns-tag-property )? )
		// [97] c-ns-tag-property ::= c-verbatim-tag | c-ns-shorthand-tag | c-non-specific-tag
		// [98] c-verbatim-tag ::= "!" "<" ns-uri-char+ ">"
		// [99] c-ns-shorthand-tag ::= c-tag-handle ns-tag-char+
		// [100] c-non-specific-tag ::= "!"
		// [101] c-ns-anchor-property ::= "&" ns-anchor-name
		// [102] ns-anchor-char ::= ns-char - c-flow-indicator
		// [103] ns-anchor-name ::= ns-anchor-char+
		
		#endregion
		
		#region 7. Flow Styles
		
		// [104] c-ns-alias-node ::= "*" ns-anchor-name
		// [105] e-scalar ::= /* Empty */
		// [106] e-node ::= e-scalar
		// [107] nb-double-char ::= c-ns-esc-char | ( nb-json - "\" - """ )
		// [108] ns-double-char ::= nb-double-char - s-white
		// [109] c-double-quoted(n,c) ::= """ nb-double-text(n,c) """
		// [110] nb-double-text(n,c) ::= c = flow-out  => nb-double-multi-line(n)
		//                               c = flow-in   => nb-double-multi-line(n)
		//                               c = block-key => nb-double-one-line
		//                               c = flow-key  => nb-double-one-line
		// [111] nb-double-one-line ::= nb-double-char*
		// [112] s-double-escaped(n) ::= s-white* "\" b-non-content l-empty(n,flow-in)* s-flow-line-prefix(n)
		// [113] s-double-break(n) ::= s-double-escaped(n) | s-flow-folded(n)
		// [114] nb-ns-double-in-line ::= ( s-white* ns-double-char )*
		// [115] s-double-next-line(n) ::= s-double-break(n) ( ns-double-char nb-ns-double-in-line ( s-double-next-line(n) | s-white* ) )?
		// [116] nb-double-multi-line(n) ::= nb-ns-double-in-line ( s-double-next-line(n) | s-white* )
		// [117] c-quoted-quote ::= "'" "'"
		// [118] nb-single-char ::= c-quoted-quote | ( nb-json - "'" )
		// [119] ns-single-char ::= nb-single-char - s-white
		// [120] c-single-quoted(n,c) ::= "'" nb-single-text(n,c) "'"
		// [121] nb-single-text(n,c) ::= c = flow-out  => nb-single-multi-line(n)
		//                               c = flow-in   => nb-single-multi-line(n)
		//                               c = block-key => nb-single-one-line
		//                               c = flow-key  => nb-single-one-line
		// [122] nb-single-one-line ::= nb-single-char*
		// [123] nb-ns-single-in-line ::= ( s-white* ns-single-char )*
		// [124] s-single-next-line(n) ::= s-flow-folded(n) ( ns-single-char nb-ns-single-in-line ( s-single-next-line(n) | s-white* ) )?
		// [125] nb-single-multi-line(n) ::= nb-ns-single-in-line ( s-single-next-line(n) | s-white* )
		// [126] ns-plain-first(c) ::= ( ns-char - c-indicator ) | ( ( "?" | ":" | "-" ) /* Followed by an ns-plain-safe(c) */ )
		// [127] ns-plain-safe(c) ::= c = flow-out  => ns-plain-safe-out
		//                            c = flow-in   => ns-plain-safe-in
		//                            c = block-key => ns-plain-safe-out
		//                            c = flow-key  => ns-plain-safe-in
		// [128] ns-plain-safe-out ::= ns-char
		// [129] ns-plain-safe-in ::= ns-char - c-flow-indicator
		// [130] ns-plain-char(c) ::= ( ns-plain-safe(c) - ":" - "#" ) | ( /* An ns-char preceding */ "#" ) | ( ":" /* Followed by an ns-plain-safe(c) */ )
		// [131] ns-plain(n,c) ::= c = flow-out  => ns-plain-multi-line(n,c)
		//                         c = flow-in   => ns-plain-multi-line(n,c)
		//                         c = block-key => ns-plain-one-line(c)
		//                         c = flow-key  => ns-plain-one-line(c)
		// [132] nb-ns-plain-in-line(c) ::= ( s-white* ns-plain-char(c) )*
		// [133] ns-plain-one-line(c) ::= ns-plain-first(c) nb-ns-plain-in-line(c)
		// [134] s-ns-plain-next-line(n,c) ::= s-flow-folded(n) ns-plain-char(c) nb-ns-plain-in-line(c)
		// [135] ns-plain-multi-line(n,c) ::= ns-plain-one-line(c) s-ns-plain-next-line(n,c)*
		// [136] in-flow(c) ::= c = flow-out  => flow-in
		//                      c = flow-in   => flow-in
		//                      c = block-key => flow-key
		//                      c = flow-key  => flow-key
		// [137] c-flow-sequence(n,c) ::= "[" s-separate(n,c)? ns-s-flow-seq-entries(n,in-flow(c))? "]"
		// [138] ns-s-flow-seq-entries(n,c) ::= ns-flow-seq-entry(n,c) s-separate(n,c)? ( "," s-separate(n,c)? ns-s-flow-seq-entries(n,c)? )?
		// [139] ns-flow-seq-entry(n,c) ::= ns-flow-pair(n,c) | ns-flow-node(n,c)
		// [140] c-flow-mapping(n,c) ::= "{" s-separate(n,c)? ns-s-flow-map-entries(n,in-flow(c))? "}"
		// [141] ns-s-flow-map-entries(n,c) ::= ns-flow-map-entry(n,c) s-separate(n,c)? ( "," s-separate(n,c)? ns-s-flow-map-entries(n,c)? )?
		// [142] ns-flow-map-entry(n,c) ::= ( "?" s-separate(n,c) ns-flow-map-explicit-entry(n,c) ) | ns-flow-map-implicit-entry(n,c)
		// [143] ns-flow-map-explicit-entry(n,c) ::= ns-flow-map-implicit-entry(n,c) | ( e-node /* Key */ e-node /* Value */ )
		// [144] ns-flow-map-implicit-entry(n,c) ::= ns-flow-map-yaml-key-entry(n,c) | c-ns-flow-map-empty-key-entry(n,c) | c-ns-flow-map-json-key-entry(n,c)
		// [145] ns-flow-map-yaml-key-entry(n,c) ::= ns-flow-yaml-node(n,c) ( ( s-separate(n,c)? c-ns-flow-map-separate-value(n,c) ) | e-node )
		// [146] c-ns-flow-map-empty-key-entry(n,c) ::= e-node /* Key */ c-ns-flow-map-separate-value(n,c)
		// [147] c-ns-flow-map-separate-value(n,c) ::= ":" /* Not followed by an ns-plain-safe(c) */ ( ( s-separate(n,c) ns-flow-node(n,c) ) | e-node /* Value */ )
		// [148] c-ns-flow-map-json-key-entry(n,c) ::= c-flow-json-node(n,c) ( ( s-separate(n,c)? c-ns-flow-map-adjacent-value(n,c) ) | e-node )
		// [149] c-ns-flow-map-adjacent-value(n,c) ::= ":" ( ( s-separate(n,c)? ns-flow-node(n,c) ) | e-node ) /* Value */
		// [150] ns-flow-pair(n,c) ::= ( "?" s-separate(n,c) ns-flow-map-explicit-entry(n,c) ) | ns-flow-pair-entry(n,c)
		// [151] ns-flow-pair-entry(n,c) ::= ns-flow-pair-yaml-key-entry(n,c) | c-ns-flow-map-empty-key-entry(n,c) | c-ns-flow-pair-json-key-entry(n,c)
		// [152] ns-flow-pair-yaml-key-entry(n,c) ::= ns-s-implicit-yaml-key(flow-key) c-ns-flow-map-separate-value(n,c)
		// [153] c-ns-flow-pair-json-key-entry(n,c) ::= c-s-implicit-json-key(flow-key) c-ns-flow-map-adjacent-value(n,c)
		// [154] ns-s-implicit-yaml-key(c) ::= ns-flow-yaml-node(n/a,c) s-separate-in-line? /* At most 1024 characters altogether */
		// [155] c-s-implicit-json-key(c) ::= c-flow-json-node(n/a,c) s-separate-in-line? /* At most 1024 characters altogether */
		// [156] ns-flow-yaml-content(n,c) ::= ns-plain(n,c)
		// [157] c-flow-json-content(n,c) ::= c-flow-sequence(n,c) | c-flow-mapping(n,c) | c-single-quoted(n,c) | c-double-quoted(n,c)
		// [158] ns-flow-content(n,c) ::= ns-flow-yaml-content(n,c) | c-flow-json-content(n,c)
		// [159] ns-flow-yaml-node(n,c) ::= c-ns-alias-node | ns-flow-yaml-content(n,c) | ( c-ns-properties(n,c) ( ( s-separate(n,c) ns-flow-yaml-content(n,c) ) | e-scalar ) )
		// [160] c-flow-json-node(n,c) ::= ( c-ns-properties(n,c) s-separate(n,c) )? c-flow-json-content(n,c)
		// [161] ns-flow-node(n,c) ::= c-ns-alias-node | ns-flow-content(n,c) | ( c-ns-properties(n,c) ( ( s-separate(n,c) ns-flow-content(n,c) ) | e-scalar ) )
		
		#endregion
		
		#region 8. Block Styles
		
		// [162] c-b-block-header(m,t) ::= ( ( c-indentation-indicator(m) c-chomping-indicator(t) ) | ( c-chomping-indicator(t) c-indentation-indicator(m) ) ) s-b-comment
		// [163] c-indentation-indicator(m) ::= ns-dec-digit => m = ns-dec-digit - #x30
		//                                      /* Empty */  => m = auto-detect()
		// [164] c-chomping-indicator(t) ::= "-"         => t = strip
		//                                   "+"         => t = keep
		//                                   /* Empty */ => t = clip
		// [165] b-chomped-last(t) ::= t = strip => b-non-content | /* End of file */
		//                             t = clip  => b-as-line-feed | /* End of file */
		//                             t = keep  => b-as-line-feed | /* End of file */
		// [166] l-chomped-empty(n,t) ::= t = strip => l-strip-empty(n)
		//                                t = clip  => l-strip-empty(n)
		//                                t = keep  => l-keep-empty(n)
		// [167] l-strip-empty(n) ::= ( s-indent(<=n) b-non-content )* l-trail-comments(n)?
		// [168] l-keep-empty(n) ::= l-empty(n,block-in)* l-trail-comments(n)?
		// [169] l-trail-comments(n) ::= s-indent(<n) c-nb-comment-text b-comment l-comment*
		// [170] c-l+literal(n) ::= "|" c-b-block-header(m,t) l-literal-content(n+m,t)
		// [171] l-nb-literal-text(n) ::= l-empty(n,block-in)* s-indent(n) nb-char+
		// [172] b-nb-literal-next(n) ::= b-as-line-feed l-nb-literal-text(n)
		// [173] l-literal-content(n,t) ::= ( l-nb-literal-text(n) b-nb-literal-next(n)* b-chomped-last(t) )? l-chomped-empty(n,t)
		// [174] c-l+folded(n) ::= ">" c-b-block-header(m,t) l-folded-content(n+m,t)
		// [175] s-nb-folded-text(n) ::= s-indent(n) ns-char nb-char*
		// [176] l-nb-folded-lines(n) ::= s-nb-folded-text(n) ( b-l-folded(n,block-in) s-nb-folded-text(n) )*
		// [177] s-nb-spaced-text(n) ::= s-indent(n) s-white nb-char*
		// [178] b-l-spaced(n) ::= b-as-line-feed l-empty(n,block-in)*
		// [179] l-nb-spaced-lines(n) ::= s-nb-spaced-text(n) ( b-l-spaced(n) s-nb-spaced-text(n) )*
		// [180] l-nb-same-lines(n) ::= l-empty(n,block-in)* ( l-nb-folded-lines(n) | l-nb-spaced-lines(n) )
		// [181] l-nb-diff-lines(n) ::= l-nb-same-lines(n) ( b-as-line-feed l-nb-same-lines(n) )*
		// [182] l-folded-content(n,t) ::= ( l-nb-diff-lines(n) b-chomped-last(t) )? l-chomped-empty(n,t)
		// [183] l+block-sequence(n) ::= ( s-indent(n+m) c-l-block-seq-entry(n+m) )+ /* For some fixed auto-detected m > 0 */
		// [184] c-l-block-seq-entry(n) ::= "-" /* Not followed by an ns-char */ s-l-block-indented(n,block-in)
		// [185] s-l+block-indented(n,c) ::= ( s-indent(m) ( ns-l-compact-sequence(n+1+m) | ns-l-compact-mapping(n+1+m) ) ) | s-l+block-node(n,c) | ( e-node s-l-comments )
		// [186] ns-l-compact-sequence(n) ::= c-l-block-seq-entry(n) ( s-indent(n) c-l-block-seq-entry(n) )*
		// [187] l+block-mapping(n) ::= ( s-indent(n+m) ns-l-block-map-entry(n+m) )+ /* For some fixed auto-detected m > 0 */
		// [188] ns-l-block-map-entry(n) ::= c-l-block-map-explicit-entry(n) | ns-l-block-map-implicit-entry(n)
		// [189] c-l-block-map-explicit-entry(n) ::= c-l-block-map-explicit-key(n) ( l-block-map-explicit-value(n) | e-node )
		// [190] c-l-block-map-explicit-key(n) ::= "?" s-l+block-indented(n,block-out)
		// [191] l-block-map-explicit-value(n) ::= s-indent(n) ":" s-l+block-indented(n,block-out)
		// [192] ns-l-block-map-implicit-entry(n) ::= ( ns-s-block-map-implicit-key | e-node ) c-l-block-map-implicit-value(n)
		// [193] ns-s-block-map-implicit-key ::= c-s-implicit-json-key(block-key) | ns-s-implicit-yaml-key(block-key)
		// [194] c-l-block-map-implicit-value(n) ::= ":" ( s-l+block-node(n,block-out) | ( e-node s-l-comments ) )
		// [195] ns-l-compact-mapping(n) ::= ns-l-block-map-entry(n) ( s-indent(n) ns-l-block-map-entry(n) )*
		// [196] s-l+block-node(n,c) ::= s-l+block-in-block(n,c) | s-l+flow-in-block(n)
		// [197] s-l+flow-in-block(n) ::= s-separate(n+1,flow-out) ns-flow-node(n+1,flow-out) s-l-comments
		// [198] s-l+block-in-block(n,c) ::= s-l+block-scalar(n,c) | s-l+block-collection(n,c)
		// [199] s-l+block-scalar(n,c) ::= s-separate(n+1,c) ( c-ns-properties(n+1,c) s-separate(n+1,c) )? ( c-l+literal(n) | c-l+folded(n) )		
		// [200] s-l+block-collection(n,c) ::= ( s-separate(n+1,c) c-ns-properties(n+1,c) )? s-l-comments ( l+block-sequence(seq-spaces(n,c)) | l+block-mapping(n) )
		// [201] seq-spaces(n,c) ::= c = block-out => n-1
		//                           c = block-in  => n
		
		#endregion
		
		#region 9. YAML Character Stream
		
		// [202] l-document-prefix ::= c-byte-order-mark? l-comment*
		// [203] c-directives-end ::= "-" "-" "-"
		// [204] c-document-end ::= "." "." "."
		// [205] l-document-suffix ::= c-document-end s-l-comments
		// [206] c-forbidden ::= /* Start of line */ ( c-directives-end | c-document-end ) ( b-char | s-white | /* End of file */ )
		// [207] l-bare-document ::= s-l+block-node(-1,block-in) /* Excluding c-forbidden content */
		// [208] l-explicit-document ::= c-directives-end ( l-bare-document | ( e-node s-l-comments ) )
		// [209] l-directive-document ::= l-directive+ l-explicit-document
		// [210] l-any-document ::= l-directive-document | l-explicit-document | l-bare-document
		// [211] l-yaml-stream ::= l-document-prefix* l-any-document? ( l-document-suffix+ l-document-prefix* l-any-document? | l-document-prefix* l-explicit-document? )*
		
		#endregion

        public static bool IsDocumentPrefix(Stream stream)
        {
            stream.Read( )
        }
	}
}

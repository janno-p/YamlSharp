using System;
using System.Collections.Generic;

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
		
		
		
		#endregion
	}
}

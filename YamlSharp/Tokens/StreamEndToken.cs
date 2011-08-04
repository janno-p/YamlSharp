namespace YamlSharp.Tokens
{
	public class StreamEndToken : Token
	{
		public StreamEndToken(int startMark, int endMark)
			: base(startMark, endMark)
		{}
	}
}
namespace YamlSharp.Tokens
{
	public class StreamStartToken : Token
	{
		public StreamStartToken(int startMark, int endMark)
			: base(startMark, endMark)
		{}
	}
}
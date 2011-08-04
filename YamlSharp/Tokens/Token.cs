namespace YamlSharp.Tokens
{
	public class Token
	{
		public int StartMark { get; private set; }
		public int EndMark { get; private set; }

		public Token(int startMark, int endMark)
		{
			StartMark = startMark;
			EndMark = endMark;
		}
	}
}
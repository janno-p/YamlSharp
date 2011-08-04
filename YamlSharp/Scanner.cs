using System.Collections.Generic;
using System.IO;

namespace YamlSharp
{
	public class Scanner
	{
		private readonly StreamReader reader;

		public Scanner(StreamReader reader)
		{
			this.reader = reader;
		}

		public IEnumerable<Token> ReadTokens()
		{
			yield break;
		}
	}
}
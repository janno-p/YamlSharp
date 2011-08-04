using System.Collections.Generic;

namespace YamlSharp.Tokens
{
    public class DirectiveToken : Token
    {
        private readonly string name;
        private readonly string[] parameters;

        public string Name { get { return name; } }
        public IEnumerable<string> Parameters { get { return parameters; } }

        public DirectiveToken(int startMark, int endMark, string name, params string[] parameters)
            : base(startMark, endMark)
        {
            this.name = name;
            this.parameters = parameters;
        }
    }
}
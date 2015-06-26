using System.Collections.Generic;

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

    public class StreamStartToken : Token
    {
        public StreamStartToken(int startMark, int endMark) : base(startMark, endMark)
        {}
    }

    public class StreamEndToken : Token
    {
        public StreamEndToken(int startMark, int endMark) : base(startMark, endMark)
        {}
    }

    public class DocumentStartToken : Token
    {
        public DocumentStartToken(int startMark, int endMark) : base(startMark, endMark)
        {}
    }

    public class DocumentEndToken : Token
    {
        public DocumentEndToken(int startMark, int endMark) : base(startMark, endMark)
        {}
    }

    public class DocumentContentToken : Token
    {
        private readonly string content;

        public string Content { get { return content; } }

        public DocumentContentToken(int startMark, int endMark, string content) : base(startMark, endMark)
        {
            this.content = content;
        }
    }

    public class DirectivesStartToken : Token
    {
        public DirectivesStartToken(int startMark, int endMark) : base(startMark, endMark)
        {}
    }

    public class DirectivesEndToken : Token
    {
        public DirectivesEndToken(int startMark, int endMark) : base(startMark, endMark)
        {}
    }

    public class DirectiveToken : Token
    {
        private readonly string name;
        private readonly string[] parameters;

        public string Name { get { return name; } }
        public IEnumerable<string> Parameters { get { return parameters; } }

        public DirectiveToken(int startMark, int endMark, string name, params string[] parameters) : base(startMark, endMark)
        {
            this.name = name;
            this.parameters = parameters;
        }
    }
}
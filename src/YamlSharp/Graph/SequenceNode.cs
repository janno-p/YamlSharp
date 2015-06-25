using System.Collections.Generic;

namespace YamlSharp.Graph
{
    public class SequenceNode : Node
    {
        private readonly IList<Node> childNodes = new List<Node>();

        public IList<Node> ChildNodes { get { return childNodes; } }
    }
}
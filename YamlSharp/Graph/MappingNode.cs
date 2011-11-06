using System.Collections.Generic;

namespace YamlSharp.Graph
{
    public class MappingNode : Node
    {
        private readonly IDictionary<Node, Node> childNodes = new Dictionary<Node, Node>();

        public IDictionary<Node, Node> ChildNodes { get { return childNodes; } }
    }
}
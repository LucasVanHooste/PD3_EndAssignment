using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

abstract class CompositeNode : INode
{
    protected INode[] _nodes;

    public CompositeNode(params INode[] nodes)
    {
        _nodes = nodes;
    }

    public abstract IEnumerator<NodeResult> Tick();
}

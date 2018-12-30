using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


    public class ConditionNode : INode
    {
    public delegate bool Condition();

    private readonly Condition _condition;

        public ConditionNode(Condition condition)
        {
            _condition = condition;
        }
        public IEnumerator<NodeResult> Tick()
        {
            if (_condition())
            {
                yield return NodeResult.Succes;
            }
            else
            {
                yield return NodeResult.Failure;
            }
        }
    }

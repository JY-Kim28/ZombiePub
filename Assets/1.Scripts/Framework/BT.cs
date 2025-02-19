using System;
using System.Collections.Generic;

public class BTRunner
{
    IBTNode _RootNode;

    public BTRunner(IBTNode rootNode)
    {
        _RootNode = rootNode;
    }

    public void Operate(bool running)
    {
        _RootNode.Evaluate(running);
    }
}


public interface IBTNode
{
    public enum BT_State
    {
        Running,
        Success,
        Failure
    }

    public BT_State Evaluate(bool running);
}

public sealed class BTActionNode : IBTNode
{
    Func<IBTNode.BT_State> _OnUpdate = null;

    public BTActionNode(Func<IBTNode.BT_State> onUpdate)
    {
        _OnUpdate = onUpdate;
    }   

    public IBTNode.BT_State Evaluate(bool running)
    {
        if (running)
            return _OnUpdate?.Invoke() ?? IBTNode.BT_State.Failure;
        else
            return IBTNode.BT_State.Failure;
    }
}

public sealed class BTSelectNode : IBTNode
{
    List<IBTNode> _Childs;

    public BTSelectNode(List<IBTNode> childs)
    {
        _Childs = childs;
    }

    public IBTNode.BT_State Evaluate(bool running)
    {
        if (_Childs == null || running == false) 
            return IBTNode.BT_State.Failure;

        foreach(var child in _Childs)
        {
            if (running == false)
                return IBTNode.BT_State.Failure;

            switch(child.Evaluate(running))
            {
                case IBTNode.BT_State.Running:
                    return IBTNode.BT_State.Running;
                case IBTNode.BT_State.Success:
                    return IBTNode.BT_State.Success;
            }
        }

        return IBTNode.BT_State.Failure;
    }
}

public sealed class BTSequenceNode : IBTNode
{
    List<IBTNode> _Childs;

    public BTSequenceNode(List<IBTNode> childs)
    {
        _Childs = childs;
    }

    public IBTNode.BT_State Evaluate(bool running)
    {
        if (_Childs == null || running == false)
            return IBTNode.BT_State.Failure;

        foreach (var child in _Childs)
        {
            if (running == false)
                return IBTNode.BT_State.Failure;

            switch (child.Evaluate(running))
            {
                case IBTNode.BT_State.Running:
                    return IBTNode.BT_State.Running;
                case IBTNode.BT_State.Success:
                    continue;
                case IBTNode.BT_State.Failure:
                    return IBTNode.BT_State.Failure;
            }
        }

        return IBTNode.BT_State.Success;
    }
}

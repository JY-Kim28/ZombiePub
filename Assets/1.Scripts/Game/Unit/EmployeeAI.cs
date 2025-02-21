using System.Collections.Generic;
using UnityEngine;

public class EmployeeAI
{
    Employee me;

    protected BTRunner _BT;
    protected Queue<BTActionNode> _BTActionQueue;

    ObjectBase machine = null;

    public EmployeeAI(Employee unit)
    {
        me = unit;

        _BTActionQueue = new Queue<BTActionNode>();

        _BT = new BTRunner(CreateBT());
    }

    public virtual IBTNode CreateBT()
    {
        return new BTSelectNode(new List<IBTNode>()
        {
            new BTActionNode(WalkToMachine),


            new BTSequenceNode(new List<IBTNode>()
            {
                new BTActionNode(HaveObjects),

                new BTSelectNode(new List<IBTNode>()
                {
                    new BTActionNode(HaveTrash),
                    new BTActionNode(HaveProduct),
                })
            }),

            new BTSequenceNode(new List<IBTNode>()
            {
                new BTActionNode(HaveMachine),

                new BTSelectNode(new List<IBTNode>()
                {
                    new BTActionNode(WorkTakeManufacture),
                    new BTActionNode(WalkToCounterProduct),
                    new BTActionNode(WorkCasher),
                    new BTActionNode(WorkProcesser),
                    new BTActionNode(WorkTakeProcesser),
                    new BTActionNode(WalkToProcessingInsertPoint)                    
                })
            }),

            new BTSelectNode(new List<IBTNode>()
            {
                new BTActionNode(FindTrash),
                new BTActionNode(FindSelling),
                new BTActionNode(FindProcessorProduct),
                new BTActionNode(FindProcessorWork),
                new BTActionNode(FindManufactureProduct),
            })
        });
    }

    public void Update(bool running)
    {
        _BT.Operate(running);
    }

    public virtual IBTNode.BT_State WalkToMachine()
    {
        if (machine == null)
            return IBTNode.BT_State.Failure;

        if (machine.readyWorker != me && machine.IsNeedWorker() == false)
        {
            machine = null;
            return IBTNode.BT_State.Failure;
        }

        float limit = 1;
        if(machine.objType == OBJ_TYPE.Selling)
        {
            limit = 0.5f;
        }

        if (me.naviAgent.remainingDistance > limit)
        {
            return IBTNode.BT_State.Running;
        }

        return IBTNode.BT_State.Failure;
    }

    #region Work
    public IBTNode.BT_State HaveMachine()
    {
        if (machine == null)
            return IBTNode.BT_State.Failure;

        return IBTNode.BT_State.Success;
    }

    public IBTNode.BT_State WorkTakeProcesser()
    {
        if (machine.objType == OBJ_TYPE.Processing)
        {
            if (Vector3.SqrMagnitude((machine as ProcessingMachine).ReleasePos() - me.naviAgent.destination) < 0.1f)
            {
                if (me.GetProducts().Count == me.Stat.capacity)
                {
                    machine = null;
                    return IBTNode.BT_State.Success;
                }
            }
        }

        return IBTNode.BT_State.Failure;
    }

    public IBTNode.BT_State WorkTakeManufacture()
    {
        if(machine.objType == OBJ_TYPE.Manufacture)
        {
            if (me.GetProducts().Count == me.Stat.capacity)
            {
                machine = null;
                return IBTNode.BT_State.Success;
            }
            else
            {
                return IBTNode.BT_State.Running;
            }
        }

        return IBTNode.BT_State.Failure;    
    }

    public IBTNode.BT_State WalkToCounterProduct()
    {
        if (machine.objType == OBJ_TYPE.Selling)
        {
            SellingMachine selling = machine as SellingMachine;
            if (Vector3.SqrMagnitude(selling.InsertPos() - me.naviAgent.destination) < 10)
            {
                machine = null;
                return IBTNode.BT_State.Success;
            }
        }

        return IBTNode.BT_State.Failure;
    }

    public IBTNode.BT_State WorkCasher()
    {
        if (machine.objType == OBJ_TYPE.Selling)
        {
            SellingMachine selling = machine as SellingMachine;
            if(selling.HaveCustomer() == false || selling.GetProductCount() == 0)
            {
                machine = null;
                return IBTNode.BT_State.Success;
            }

            return IBTNode.BT_State.Running;
        }

        return IBTNode.BT_State.Failure;
    }

    public IBTNode.BT_State WalkToProcessingInsertPoint()
    {
        if (machine.objType == OBJ_TYPE.Processing)
        {
            ProcessingMachine processingMachine = machine as ProcessingMachine;
            if (Vector3.SqrMagnitude(processingMachine.InsertPos() - me.naviAgent.destination) < 0.1f)
            {
                machine = null;
                return IBTNode.BT_State.Success;
            }
        }

        return IBTNode.BT_State.Failure;
    }

    public IBTNode.BT_State WorkProcesser()
    {
        if (machine.objType == OBJ_TYPE.Processing)
        {
            ProcessingMachine processingMachine = machine as ProcessingMachine;
            if (Vector3.SqrMagnitude((machine as ProcessingMachine).ProcessPos() - me.naviAgent.destination) < 0.1f)
            {
                if (processingMachine.IsNeedWorker() == false)
                {
                    machine = null;
                    return IBTNode.BT_State.Success;
                }
                return IBTNode.BT_State.Running;
            }
        }

        return IBTNode.BT_State.Failure;
    }
    #endregion Work

    #region Select Work
    public IBTNode.BT_State HaveObjects()
    {
        if (me.GetProductData() == null)
        {
            return IBTNode.BT_State.Failure;
        }

        return IBTNode.BT_State.Success;
    }

    public IBTNode.BT_State HaveTrash()
    {
        if (me.GetProductData() == Game.Stage.GetTrashData())
        {
            foreach (var trashBin in Game.Stage.trashBins)
            {
                machine = trashBin;
                me.SetDestination(trashBin.transform.position);

                return IBTNode.BT_State.Success;
            }
        }

        return IBTNode.BT_State.Failure;
    }

    public IBTNode.BT_State HaveProduct()
    {
        List<ObjectBase> objs = new List<ObjectBase>();

        foreach (var obj in Game.Stage.objectList)
        {
            if (obj.currLv == 0) continue;

            if (obj.objType == OBJ_TYPE.Selling)
            {
                if (me.GetProductData() == (obj as SellingMachine).productData)
                {
                    objs.Add(obj);
                }
            }
            else if (obj.objType == OBJ_TYPE.Processing)
            {
                if(me.GetProductData() == (obj as ProcessingMachine).insertProductData)
                {
                    objs.Add(obj);
                }
            }
        }

        int objsCount = objs.Count;
        if(objsCount != 0)
        {
            machine = objs[UnityEngine.Random.Range(0, objs.Count)];
            if(machine.objType == OBJ_TYPE.Selling)
            {
                me.SetDestination((machine as SellingMachine).InsertPos());
            }
            else
            {
                me.SetDestination((machine as ProcessingMachine).InsertPos());
            }

            return IBTNode.BT_State.Success;
        }

        return IBTNode.BT_State.Failure;
    }

    public IBTNode.BT_State FindTrash()
    {
        foreach(var usingMachine in Game.Stage.usingMachines)
        {
            if (usingMachine.currLv == 0) continue;

            if(usingMachine.IsNeedWorker())
            {
                machine = usingMachine;
                machine.readyWorker = me;
                me.SetDestination(usingMachine.transform.position);
                return IBTNode.BT_State.Success;
            }
        }
        return IBTNode.BT_State.Failure;
    }

    public IBTNode.BT_State FindSelling()
    {
        SellingMachine selling;
        foreach (var obj in Game.Stage.objectList)
        {
            selling = obj as SellingMachine;
            if(selling != null)
            {
                if(selling.IsNeedWorker())
                {
                    machine = selling;
                    machine.readyWorker = me;
                    me.SetDestination(selling.CasherPos());
                    return IBTNode.BT_State.Success;
                }
            }
        }

        return IBTNode.BT_State.Failure;
    }

    public IBTNode.BT_State FindProcessorProduct()
    {
        foreach(var obj in Game.Stage.objectList)
        {
            if(obj.objType == OBJ_TYPE.Processing)
            {
                if((obj as ProcessingMachine).HaveReleaseProduct())
                {
                    machine = obj;
                    me.SetDestination((obj as ProcessingMachine).ReleasePos());

                    return IBTNode.BT_State.Success;
                }
            }
        }
        return IBTNode.BT_State.Failure;
    }

    public IBTNode.BT_State FindProcessorWork()
    {
        foreach (var obj in Game.Stage.objectList)
        {
            if (obj.objType == OBJ_TYPE.Processing)
            {
                if ((obj as ProcessingMachine).IsNeedWorker())
                {
                    machine = obj;
                    machine.readyWorker = me;
                    me.SetDestination((obj as ProcessingMachine).ProcessPos());

                    return IBTNode.BT_State.Success;
                }
            }
        }
        return IBTNode.BT_State.Failure;
    }

    public IBTNode.BT_State FindManufactureProduct()
    {
        ManufactureMachine manufacture;
        foreach (var obj in Game.Stage.objectList)
        {
            manufacture = obj as ManufactureMachine;
            if (manufacture != null)
            {
                if (manufacture.IsNeedWorker())
                {
                    machine = manufacture;
                    me.SetDestination(manufacture.ReleasePos());
                    return IBTNode.BT_State.Success;
                }
            }
        }
        return IBTNode.BT_State.Failure;
    }

    #endregion Select Work

}
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Customer : Unit
{
    [SerializeField] protected ScriptBubble scriptBubble;

    public CUSTOMER_TYPE type;

    public int TargetCount { get; protected set; } = 1;
    public int Count { get; protected set; } = 1;

    Transform outTr;

    public enum STATE
    {
        GoToChasherMachine,
        BuyingReady,
        Buying,
        FindUsingMachine,
        GoToUisngMachine,
        UsingProduct,
        GoToOutPoint,

        Event
    }

    public STATE state = STATE.GoToChasherMachine;

    protected Coroutine stateCoroutine;

    public float originalUsingSpeed = 1;
    public float usingSpeed = 1;
    public ushort price;

    protected SellingMachine sellingMachine;

    public int targetPosIdx { get; protected set; }
    public Vector3 targetPos { get; protected set; }


    public virtual void Create()
    {
        TargetCount = UnityEngine.Random.Range(1, 4);
        Count = TargetCount;

        state = STATE.GoToChasherMachine;

        foreach (var m in Game.Stage.objectList)
        {
            if(m.objType == OBJ_TYPE.Selling)
            {
                sellingMachine = m as SellingMachine;
                if(sellingMachine != null)
                {
                    if (sellingMachine.InsertCustomer(this))
                        break;
                    else
                        sellingMachine = null;
                }
            }
        }

        DrawStack();

        scriptBubble.Hide();

        if (sellingMachine == null)
        {
            SetStateGoToOutPoint();
        }
        else
        {
           stateCoroutine = StartCoroutine(ReadyToFirstCustomer());
        }
    }

    protected virtual IEnumerator ReadyToFirstCustomer()
    {
        while (state == STATE.GoToChasherMachine)
        {
            yield return null;

            if (Vector3.SqrMagnitude(transform.position - targetPos) <= 0.002f)
            {
                transform.position = targetPos;

                if(targetPos == sellingMachine.GetFirstCustomerPosition())
                {
                    state = STATE.BuyingReady;

                    scriptBubble.ShowOrder($"Product{sellingMachine.productData.Idx}", Count);
                }
                else
                {
                    sellingMachine.ArrangementCustomerLine();
                }
            }
        }
    }

    public virtual void SetDestination(int posIdx, Vector3 pos)
    {
        targetPosIdx = posIdx;
        targetPos = pos;
    }

    public virtual void Take(Product product)
    {
        Count--;

        scriptBubble.DrawCount(Count);

        InputProduct(product);

        if (Count == 0)
        {
            scriptBubble.Hide();
        }
    }

    public virtual bool PlayEvent()
    {
        return false;
    }

    public void SetStateGoToOutPoint()
    {
        state = STATE.GoToOutPoint;

        SetDestination(0, outTr.position);
        
        stateCoroutine = StartCoroutine(CheckingArriveOutPoint());
    }

    private IEnumerator CheckingArriveOutPoint()
    {
        while (state == STATE.GoToOutPoint)
        {
            yield return null;

            if (Vector3.SqrMagnitude(transform.position - targetPos) <= 1)
            {
                break;
            }
        }

        Root.Resources.ReleaseCustomer(this);
    }

    public void SetOutPoint(Transform tr)
    {
        outTr = tr;
    }
}

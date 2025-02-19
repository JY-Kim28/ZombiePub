using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Customer : Unit
{
    [SerializeField] ScriptBubble scriptBubble;

    public int TargetCount { get; private set; } = 1;
    public int Count { get; private set; } = 1;

    public enum STATE
    {
        GoToChasherMachine,
        BuyingReady,
        Buying,
        FindUsingMachine,
        GoToUisngMachine,
        UsingProduct,
        GoToOutPoint
    }

    public STATE state = STATE.GoToChasherMachine;

    NavMeshAgent naviAgent;

    Coroutine stateCoroutine;

    public float originalUsingSpeed = 1;
    public float usingSpeed = 1;
    public ushort price;

    SellingMachine sellingMachine;
    UsingMachine usingMachine;

    public void Create()
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

    private IEnumerator ReadyToFirstCustomer()
    {
        while (state == STATE.GoToChasherMachine)
        {
            yield return new WaitForSeconds(1);

            if(Vector3.SqrMagnitude(transform.position - sellingMachine.GetFirstCustomerPosition()) <= 1)
            {
                state = STATE.BuyingReady;

                scriptBubble.ShowOrder($"Product{sellingMachine.productData.Idx}", Count);
            }
        }
    }

    public void SetDestination(Vector3 pos)
    {
        naviAgent ??= gameObject.GetComponent<NavMeshAgent>();
        naviAgent.SetDestination(pos);
        animator?.SetBool("Move", true);

    }

    public void Take(Product product)
    {
        Count--;

        scriptBubble.DrawCount(Count);

        InputProduct(product);

        if (Count == 0)
        {
            scriptBubble.Hide();

            FindUseMachine();
        }
    }

    //public override bool InputProduct(Product product)
    //{
    //    if (products.Count == 0 || (products.Peek().Data == product.Data))
    //    {
    //        productsTR.gameObject.SetActive(true);


    //        product.transform.SetParent(productsTR);
    //        product.transform.localPosition = new Vector3(0, (products.Count * product.H), 0);
    //        products.Push(product);

    //        return true;
    //    }

    //    return false;
    //}

    private void FindUseMachine()
    {
        state = STATE.FindUsingMachine;

        stateCoroutine = StartCoroutine(TryFindUseMachine());
    }

    private IEnumerator TryFindUseMachine()
    {
        while(state == STATE.FindUsingMachine)
        {
            yield return new WaitForSeconds(1);

            scriptBubble.ShowNoSeat();

            if (Game.Stage.SetCustomerToUsingMachine(this))
            {
                scriptBubble.Hide();

                sellingMachine.CustomerOut(this);
                sellingMachine = null;

                break;
            }
        }
    }

    public void SetUsingMachine(UsingMachine machine, Vector3 targetPos)
    {
        state = STATE.GoToUisngMachine;

        usingMachine = machine;

        SetDestination(targetPos);

        stateCoroutine = StartCoroutine(CheckingArriveUsingMachine());
    }

    private IEnumerator CheckingArriveUsingMachine()
    {
        while (state == STATE.GoToUisngMachine)
        {
            yield return null;

            if(naviAgent.remainingDistance <= 0.1f && naviAgent.velocity.sqrMagnitude <= 0.1f)
            {
                state = STATE.UsingProduct;
            }
        }

        Count = TargetCount;

        usingMachine.AddProduct(products);

        animator?.SetBool("Move", false);
        animator?.SetBool("Work", false);
        animator?.SetTrigger("Eat");

    }

    public void SetStateGoToOutPoint()
    {
        state = STATE.GoToOutPoint;

        SetDestination(Game.Stage.OutPointTR.position);
        
        stateCoroutine = StartCoroutine(CheckingArriveOutPoint());
    }

    private IEnumerator CheckingArriveOutPoint()
    {
        while (state == STATE.GoToOutPoint)
        {
            yield return null;

            if (naviAgent.remainingDistance <= 0.1f && naviAgent.velocity.sqrMagnitude <= 0.1f)
            {
                break;
            }
        }

        Root.Resources.ReleaseCustomer(this);
    }
}

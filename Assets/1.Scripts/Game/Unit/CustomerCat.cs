using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CustomerCat : Customer
{
    protected UsingMachine usingMachine;
    protected NavMeshAgent naviAgent;
    protected ParticleSystem zzzParticle;
    protected Quaternion targetQuaternion;

    [SerializeField] SkinnedMeshRenderer mesh;
    [SerializeField] Material[] materials;

    public override void Create()
    {
        mesh.sharedMaterial = materials[UnityEngine.Random.Range(0, materials.Length)];

        base.Create();
    }

    public override void SetDestination(int posIdx, Vector3 pos)
    {
        base.SetDestination(posIdx, pos);

        naviAgent ??= gameObject.GetComponent<NavMeshAgent>();
        naviAgent.SetDestination(pos);

        PlayAnimation("Move", true);
    }

    protected override IEnumerator ReadyToFirstCustomer()
    {
        while (state == STATE.GoToChasherMachine)
        {
            yield return null;

            if (naviAgent.remainingDistance <= 0.5f && naviAgent.velocity.sqrMagnitude <= 0.5f)
            {
                transform.position = targetPos;

                PlayAnimation("Move", false);

                if (targetPos == sellingMachine.GetFirstCustomerPosition())
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

    public override void Take(Product product)
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

    private void FindUseMachine()
    {
        state = STATE.FindUsingMachine;

        stateCoroutine = StartCoroutine(TryFindUseMachine());
    }

    private IEnumerator TryFindUseMachine()
    {
        while (state == STATE.FindUsingMachine)
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

    public void SetUsingMachine(UsingMachine machine, Vector3 targetPos, Quaternion quaternion)
    {
        state = STATE.GoToUisngMachine;

        usingMachine = machine;

        SetDestination(0, targetPos);
        targetQuaternion = quaternion;

        stateCoroutine = StartCoroutine(CheckingArriveUsingMachine());
    }

    private IEnumerator CheckingArriveUsingMachine()
    {
        while (state == STATE.GoToUisngMachine)
        {
            yield return null;

            if (naviAgent.remainingDistance <= 0.1f && naviAgent.velocity.sqrMagnitude <= 0.1f)
            {
                state = STATE.UsingProduct;
            }
        }

        Count = TargetCount;

        transform.rotation = targetQuaternion;

        usingMachine.AddProduct(products);

        SetVisibleProductTr(false);

        PlayAnimation("Move", false);
        PlayAnimation("Work", false);
        PlayAnimation("Eat");
    }



    public override bool PlayEvent()
    {
        if(UnityEngine.Random.Range(0f, 1f) < 0.2f)
        {
            PlayAnimation("Sleep", true);

            eventCoolSec = 3;
            state = STATE.Event;
            stateCoroutine = StartCoroutine(CheckingArroundPlayer());
            scriptBubble.ShowEvent();

            return true;
        }

        return false;
    }

    float eventCoolSec = 0;

    private IEnumerator CheckingArroundPlayer()
    {
        while(state == STATE.Event)
        {
            yield return null;

            if (Vector3.SqrMagnitude(Game.Player.transform.position - transform.position) < 3f)
            {
                eventCoolSec -= Time.deltaTime;

                if (eventCoolSec <= 0)
                {
                    scriptBubble.Hide();

                    usingMachine.RemoveCustomer(this);

                    PlayAnimation("Sleep", false);

                    SetStateGoToOutPoint();
                }
            }
            else
            {
                eventCoolSec = 3;
            }

            scriptBubble.DrawEventBar(1 - (eventCoolSec / 3f));
        }
    }
}

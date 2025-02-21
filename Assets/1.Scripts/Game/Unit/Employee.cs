using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;

public class Employee : Worker
{
    public NavMeshAgent naviAgent;

    EmployeeAI AI;


    public void Initialize()
    {
        if(AI == null)
        {
            AI = new EmployeeAI(this);
            Stat = new WorkerStat();
            Stat.SetBaseStat(startStat.speed, startStat.capacity, startStat.amount);
            Stat.speedLv.Subscribe(OnChangeSpeedLv);
        }
    }

    private void OnChangeSpeedLv(ushort lv)
    {
        naviAgent ??= GetComponent<NavMeshAgent>();
        naviAgent.speed = Stat.speed;
    }

    public void SetDestination(Vector3 pos)
    {
        naviAgent ??= GetComponent<NavMeshAgent>();
        naviAgent.speed = Stat.speed;
        naviAgent.SetDestination(pos);

        PlayAnimation("Move", true);
    }


    private void Update()
    {
        if(transform.localPosition.y > 0)
        {
            Vector3 pos = transform.localPosition;
            pos.y = 0;

            transform.localPosition = pos;
        }
        AI.Update(true);
    }

    public Stack<Product> GetProducts()
    {
        return products;
    }

}

public class WorkerStat
{
    public ReactiveProperty<ushort> speedLv { get; protected set; } = new(0);
    public ReactiveProperty<ushort> capacityLv { get; protected set; } = new(0);
    public ReactiveProperty<ushort> amountLv { get; protected set; } = new(0);

    public ushort lv = 1;
    public ushort grade = 0;

    protected float startSpeed = 2;
    protected ushort startCapacity = 1;
    protected ushort startAmount = 3;

    public float speed { get; protected set; }
    public ushort capacity { get; protected set; }
    public ushort amount { get; protected set; }


    public WorkerStat()
    {
        Calculate();
    }


    public void SetBaseStat(float speed, ushort capacity, ushort amount)
    {
        startSpeed = speed;
        startCapacity = capacity;
        startAmount = amount;

        Calculate();
    }

    public void SetLv(ushort lv)
    {
        this.lv = lv;

        Calculate();
    }

    public void LevelUp()
    {
        ++lv;

        Calculate();
    }

    public virtual void Calculate()
    {
        int value = lv - 1;
        if(value > 0)
        {
            speedLv.Value = (ushort)((lv / 3) + (lv % 3 >= 1 ? 1 : 0));
            capacityLv.Value = (ushort)((lv / 3) + (lv % 3 >= 2 ? 1 : 0));
            amountLv.Value = (ushort)(lv / 3);
        }
        else
        {
            speedLv.Value = 0;
            capacityLv.Value = 0;
            amountLv.Value = 0;
        }

        speed = startSpeed + (speedLv.Value * 0.2f);
        capacity = (ushort)(startCapacity + capacityLv.Value);
        amount = (ushort)(startAmount + amountLv.Value);
    }

    public ushort GetMaxLv()
    {
        return (ushort)(9 + (grade * 3));
    }

    public bool IsMaxLv()
    {
        return lv == GetMaxLv();
    }
}



public class PlayerStat : WorkerStat
{
    public void LvUpSpeed()
    {
        ++speedLv.Value;
        Calculate();
    }

    public void LvUpCapacity()
    {
        ++capacityLv.Value;
        Calculate();
    }

    public void LvUpAmount()
    {
        ++amountLv.Value;
        Calculate();
    }

    public void SetStatLv(ushort speedLv, ushort capacityLv, ushort amountLv)
    {
        this.speedLv.Value = speedLv;
        this.capacityLv.Value = capacityLv;
        this.amountLv.Value = amountLv;

        Calculate();
    }


    public override void Calculate()
    {
        speed = startSpeed + (speedLv.Value * 0.2f);
        capacity = (ushort)(startCapacity + capacityLv.Value);
        amount = (ushort)(startAmount + amountLv.Value);
    }


#if UNITY_EDITOR

    public void AddSpeedUp(float speed)
    {
        startSpeed += speed;

        Calculate();
    }
#endif

}
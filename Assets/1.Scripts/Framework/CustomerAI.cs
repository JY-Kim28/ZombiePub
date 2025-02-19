using System.Collections.Generic;
using UnityEngine;

public class CustomerAI
{
    Customer me;

    

    SellingMachine targetSellingMachine;


    protected BTRunner _BT;
    protected Queue<BTActionNode> _BTActionQueue;
    
    public CustomerAI(Customer unit)
    {
        me = unit;

        _BTActionQueue = new Queue<BTActionNode>();

        _BT = new BTRunner(CreateBT());
    }

    public virtual IBTNode CreateBT()
    {
        //현재 내가 상품을 사야하는 상태?
        //상품을 다 샀? 안 샀?

        return new BTSelectNode(new List<IBTNode>()
        {
            new BTSelectNode(new List<IBTNode>()
            {

                new BTSequenceNode(new List<IBTNode>()
                {
                }),


                new BTSequenceNode(new List<IBTNode>()
                {
                })
            })
        });
    }

    public void Update(bool running)
    {
        _BT.Operate(running);
    }


}
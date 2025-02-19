using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class SellingMachine : ObjectBase
{
    [SerializeField] UnitTriggerArea insertTriggerArea;
    [SerializeField] UnitTriggerArea casherTriggerArea;
    [SerializeField] Image casherEnterCircle;

    [Space]
    [SerializeField] Transform insertedProductContainerTR;

    [Space]
    [SerializeField] UnitTriggerArea moneyTriggerArea;
    [SerializeField] Transform moneyContainerTR;

    [SerializeField] Waypoints wayPoint;
    List<Vector3> wayPoints;
    Customer[] customers;

    public ProductScriptableObject productData;

    Stack<Product> insertedProducts = new Stack<Product>();
    Stack<Product> moneys = new Stack<Product>();

    public Action<Unit, ProductScriptableObject> insertCallback;

    private void Awake()
    {
        objType = OBJ_TYPE.Selling;

        wayPoints = wayPoint.GetPoints();
        customers = new Customer[wayPoints.Count];

        casherEnterCircle.color = Color.red;
    }

    // Start is called before the first frame update
    void Start()
    {
        insertTriggerArea.callback = InsertProduct;
        casherTriggerArea.enterCallback = CasherIn;
        casherTriggerArea.callback = SellProduct;
        casherTriggerArea.exitCallback = CasherOut;
        moneyTriggerArea.callback = GiveMoney;

    }

    public override void SetData(ushort lv, ushort decoIdx)
    {
        base.SetData(lv, decoIdx);

        if (lv != 0)
        {
            insertTriggerArea.gameObject.SetActive(true);
            casherTriggerArea.gameObject.SetActive(true);
            moneyTriggerArea.gameObject.SetActive(true);
        }
        else
        {
            insertTriggerArea.gameObject.SetActive(false);
            casherTriggerArea.gameObject.SetActive(false);
            moneyTriggerArea.gameObject.SetActive(false);
        }
    }


    private void InsertProduct(Unit unit)
    {
        if (unit as Worker)
        {
            if (unit.GetProductData() != null)
            {
                if (unit.GetProductData().Type == productData.Type && unit.GetProductData().Idx == productData.Idx)
                {
                    Product product = unit.GetProduct();
                    if (product != null)
                    {
                        PutProduct(product);

                        insertCallback?.Invoke(unit, product.Data);
                    }
                }
            }
        }
    }

    private void PutProduct(Product product)
    {
        insertedProducts.Push(product);
        product.transform.SetParent(insertedProductContainerTR);
        product.transform.localPosition = new Vector3(0, 0.18f + (insertedProducts.Count * product.H), 0);
        product.gameObject.SetActive(true);
    }

    private void CasherIn(Unit unit)
    {
        if(worker == null)
        {
            if (unit as Worker)
            {
                worker = unit as Worker;
                casherEnterCircle.color = Color.green;
            }
        }
    }

    private void SellProduct(Unit unit)
    {
        //워커가 없는 경우에 세팅 
        if(worker == null)
        {
            CasherIn(unit);
        }

        if (worker != null)
        {
            if (customers[0] == null) return;
            if (customers[0].state != Customer.STATE.BuyingReady) return;
            if (insertedProducts.Count == 0) return;

            Product product = insertedProducts.Pop();
            TakeMoney((ushort)(product.value * worker.Stat.amount));
            customers[0].Take(product);
        }
    }

    private void CasherOut(Unit unit)
    {
        if (worker != null)
        {
            if (unit as Worker == worker)
            {
                worker = null;
                casherEnterCircle.color = Color.red;
            }
        }
    }

    public void CustomerOut(Customer customer)
    {
        if (customer != customers[0])
            return;

        customers[0] = null;

        ArrangementCustomerLine();
    }



    public void TakeMoney(uint value)
    {
        for (int i = 0; i < value; ++i)
        {
            Product money = Root.Resources.GetProduct(Game.Stage.productPrefabs[0].Data);
            money.SetValue(1);
            money.transform.SetParent(moneyContainerTR);
            money.transform.localPosition = Config.GetMoneyPos(moneys.Count);
            money.gameObject.SetActive(true);
            moneys.Push(money);
        }
    }

    private void GiveMoney(Unit obj)
    {
        Config.GiveMoneyToPlayer(obj as Player, moneys);
    }




    public bool IsCustomerFull()
    {
        return customers[^1] == null;
    }

    public bool InsertCustomer(Customer customer)
    {
        for(int i = 0; i < wayPoints.Count; ++i)
        {
            if(customers[i] == null)
            {
                customers[i] = customer;
                customer.SetDestination(wayPoints[i]);

                return true;
            }
        }

        return false;
    }

    private void ArrangementCustomerLine()
    {
        for(int i = 0; i < wayPoints.Count - 1; ++i)
        {
            if(customers[i] == null && customers[i + 1] != null)
            {
                customers[i] = customers[i + 1];
                customers[i + 1] = null;
                customers[i].SetDestination(wayPoints[i]);
            }
        }
    }

    public Vector3 GetFirstCustomerPosition()
    {
        return wayPoints[0];
    }


    public override bool IsNeedWorker()
    {
        if(HaveCustomer() && insertedProducts.Count != 0)
        {
            if (worker == null)
                return true;
        }

        return false;
    }

    public bool HaveCustomer()
    {
        return customers[0] != null;
    }

    public Vector3 InsertPos()
    {
        return insertTriggerArea.transform.position;
    }

    public Vector3 CasherPos()
    {
        return casherTriggerArea.transform.position;
    }

    public int GetProductCount()
    {
        return insertedProducts.Count;
    }


    public override string GetSaveData()
    {
        return $"{currLv},{decorationIdx},{moneys.Count}";
    }

    public override void SetSaveData(string save)
    {
        string[] data = save.Split(",");

        currLv = ushort.Parse(data[0]);
        decorationIdx = ushort.Parse(data[1]);

        SetData(currLv, decorationIdx);

        if (uint.TryParse(data[2], out uint m))
        {
            TakeMoney(m);
        }
    }



    //public void ShowOrder(int count)
    //{
    //    orderUI.gameObject.SetActive(true);
    //    orderUI.ShowOrderData($"Product{productData.Idx}", count);
    //}

    //public void ShowNoSeat()
    //{
    //    orderUI.gameObject.SetActive(true);
    //    orderUI.ShowNoSeat();
    //}

    //public void HideOrder()
    //{
    //    orderUI.gameObject.SetActive(false);
    //}
}
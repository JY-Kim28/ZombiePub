using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsingMachine : ObjectBase
{
    [SerializeField] Transform productTransform;
    [SerializeField] Transform[] customerTRs;

    [SerializeField] UnitTriggerArea moneyTriggerArea;
    [SerializeField] Transform moneyContainerTR;

    [SerializeField] GameObject trash;

    Customer[] customers;


    Stack<Product> products = new Stack<Product>();
    Stack<Product> moneys = new Stack<Product>();

    float usingSpeedBuff = 0;
    float priceBuff = 0;

    Coroutine usingProductCoroutine;
    float originalUsingSpeed = 1;
    float usingSpeed = 1;

    ushort readyTrashCount = 0;
    public ushort trashCount { get; private set; }


    private void Awake()
    {
        objType = OBJ_TYPE.Using;

        customers = new Customer[customerTRs.Length];
    }


    //public void SetData()
    private void Start()
    {
        products.Clear();
        moneys.Clear();

        trash.SetActive(false);

        moneyTriggerArea.callback = GiveMoney;
    }


    public override void SetData(ushort lv, ushort decoIdx)
    {
        base.SetData(lv, decoIdx);

        moneyTriggerArea.gameObject.SetActive(lv != 0);
    }

    public void AddProduct(Stack<Product> productsOfCustomer)
    {
        while (productsOfCustomer.Count != 0)
        {
            var product = productsOfCustomer.Pop();
            product.transform.SetParent(productTransform);
            product.transform.localPosition = new Vector3(0, products.Count * product.H, 0);
            product.value += (ushort)(product.value * priceBuff);
            products.Push(product);
        }

        if (usingProductCoroutine == null)
        {
            Customer customer = customers[^1];
            if (customer != null)
                if (customer.state == Customer.STATE.UsingProduct)
                {
                    usingProductCoroutine = StartCoroutine(UsingProduct());
                    readyTrashCount = (ushort)products.Count;
                }
        }
    }


    private IEnumerator UsingProduct()
    {
        usingSpeed = originalUsingSpeed - (originalUsingSpeed * usingSpeedBuff);

        while (usingSpeed > 0)
        {
            usingSpeed -= Time.deltaTime;

            if (usingSpeed <= 0)
            {
                Product product = products.Pop();

                TakeMoney(product.value);

                Root.Resources.ReleaseProduct(product);

                if (products.Count != 0)
                {
                    usingSpeed = originalUsingSpeed - (originalUsingSpeed * usingSpeedBuff);
                }
                else
                {
                    break;
                }
            }

            yield return null;
        }

        Customer customer;
        int size = customers.Length;

        for (int i = 0; i < size; ++i)
        {
            customer = customers[i];

            if (customer == null)
                continue;

            if(customer.state == Customer.STATE.UsingProduct)
            {
                if(customer.PlayEvent() == false)
                {
                    customer.SetStateGoToOutPoint();
                    customers[i] = null;
                }
            }
        }

        trashCount = readyTrashCount;
        trash.SetActive(true);

        usingProductCoroutine = null;
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

    public bool IsHaveEmtpy()
    {
        foreach (var c in customers)
        {
            if (c == null)
                return true;
        }

        return false;
    }

    public bool SetCustomer(Customer customer)
    {
        if (trashCount != 0)
            return false;

        int size = customers.Length;
        for (int i = 0; i < size; ++i)
        {
            if (customers[i] == null)
            {
                customers[i] = customer;

                (customer as CustomerCat).SetUsingMachine(this, customerTRs[i].position, customerTRs[i].rotation);

                return true;
            }
        }

        return false;
    }

    public void RemoveCustomer(Customer customer)
    {
        int size = customers.Length;
        for (int i = 0; i < size; ++i)
        {
            if (customers[i] != null)
            {
                if(customers[i] == customer)
                {
                    customers[i] = null;

                    break;
                }
            }
        }
    }

    public override bool IsNeedWorker()
    {
        if (readyWorker != null)
            return false;

        if (trashCount != 0 && EmptyAllCustomer())
                return true;

        return false;
    }

    private bool EmptyAllCustomer()
    {
        int emptyCount = 0;
        int size = customers.Length;
        for (int i = 0; i < size; ++i)
        {
            if (customers[i] == null) emptyCount++;
        }

        return emptyCount == size;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<Worker>(out Worker worker))
        {
            if(worker.GetProductData() == null
                || worker.GetProductData() == Game.Stage.GetTrashData())
            {
                if (trashCount != 0)
                {
                    if(EmptyAllCustomer())
                    {
                        worker.InputTrash(trash.transform.position, trashCount);

                        trash.SetActive(false);

                        trashCount = 0;

                        readyWorker = null;
                    }
                }
            }
        }
    }

    public override string GetSaveData()
    {
        saveCollectCount = collectMachine != null ? collectMachine.chargeCount : uint.MaxValue;

        return $"{currLv},{decorationIdx},{saveCollectCount},{moneys.Count},{trashCount}";
    }

    public override void SetSaveData(string save)
    {
        string[] data = save.Split(",");

        currLv = ushort.Parse(data[0]);
        decorationIdx = ushort.Parse(data[1]);

        SetData(currLv, decorationIdx);

        saveCollectCount = uint.Parse(data[2]);

        if (uint.TryParse(data[3], out uint result))
        {
            TakeMoney(result);
        }

        if(ushort.TryParse(data[4], out ushort tc))
        {
            trashCount = tc;
            trash.SetActive(tc != 0);
        }
    }
}
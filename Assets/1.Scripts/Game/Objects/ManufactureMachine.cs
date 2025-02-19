using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManufactureMachine : ObjectBase
{
    [SerializeField] UnitTriggerArea releaseArea;
    [SerializeField] Transform releasedProductContainer;
    [SerializeField] Transform deco;
    [SerializeField] GameObject max;

    Stack<Product> products = new Stack<Product>();

    public float makeTime = 3;
    public float time = 0;
    public int productLimit = 3;

    public ProductScriptableObject productData;


    private void Awake()
    {
        objType = OBJ_TYPE.Manufacture;

        releaseArea.gameObject.SetActive(false);
        max.SetActive(false);
    }

    private void Start()
    {
        releaseArea.callback = RemoveObj;
        releaseArea.exitCallback = ExitWorker;
    }

    public override void SetData(ushort lv, ushort decoIdx)
    {
        base.SetData(lv, decoIdx);

        releaseArea.gameObject.SetActive(lv != 0);
    }

    protected override void SetBuff()
    {
        base.SetBuff();

        BuffData speed = FindBuff(BUFF_TYPE.SpeedUp);
        if(speed.buffType != BUFF_TYPE.None)
            makeTime = 3 - (3 * speed.buffValue);
        else
            makeTime = 3;

        BuffData make = FindBuff(BUFF_TYPE.IncreaseCount);
        if (make.buffType != BUFF_TYPE.None)
            productLimit = 3 + (int)make.buffValue;
        else
            productLimit = 3;
    }

    private void Update()
    {
        if (currLv == 0)
            return;

        if (products.Count == productLimit) return;

        if(makeTime != 0)
        {
            time -= Time.deltaTime;

            if(time <= 0)
            {
                time = makeTime;

                AddObj();
            }
        }
    }


    private void AddObj()
    { 
        Product obj;

        obj = Root.Resources.GetProduct(productData);
        obj.transform.SetParent(releasedProductContainer);
        obj.transform.localPosition = new Vector3(0, 0.18f + (products.Count * obj.H), 0);

        products.Push(obj);

        obj.gameObject.SetActive(true);

        max.SetActive(products.Count == productLimit);
    }

    private void RemoveObj(Unit unit)
    {
        if (products.Count == 0) return;

        if(productData.Type == PRODUCT_TYPE.Money)
        {
            if(unit as Player)
            {
                Root.UserInfo.AddMoney(1);

                products.Pop();
            }
        }
        else
        {
            if(unit.InputProduct(products.Peek()))
            {
                products.Pop();

                max.SetActive(false);
            }
        }
    }

    private void ExitWorker(Unit unit)
    {
        worker = null;
    }

    public override bool IsNeedWorker()
    {
        if (products.Count != 0)
            return true;

        return false;
    }

    public Vector3 ReleasePos()
    {
        return releaseArea.transform.position;
    }

    public override string GetSaveData()
    {
        saveCollectCount = collectMachine != null ? collectMachine.chargeCount : uint.MaxValue;

        return $"{currLv},{decorationIdx},{saveCollectCount},{products.Count}";
    }

    public override void SetSaveData(string save)
    {
        string[] data = save.Split(',');

        currLv = ushort.Parse(data[0]);
        decorationIdx = ushort.Parse(data[1]);

        SetData(currLv, decorationIdx);

        saveCollectCount = ushort.Parse(data[2]);

        if (uint.TryParse(data[3], out uint result))
        {
            for(int i = 0; i < result; ++i)
            {
                AddObj();
            }
        }
    }
}

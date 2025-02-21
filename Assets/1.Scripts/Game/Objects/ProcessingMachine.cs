using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcessingMachine : ObjectBase
{
    [SerializeField] UnitTriggerArea insertArea;
    [SerializeField] UnitTriggerArea processingArea;
    [SerializeField] Image processingEnterCircle;
    [SerializeField] UnitTriggerArea releaseArea;

    [SerializeField] Transform insertedProductContainerTR;
    [SerializeField] Transform releasedProductContainerTR;

    public ProductScriptableObject insertProductData;
    public ProductScriptableObject releaseProductData;
    public ushort insertCountForRelease = 1;

    Stack<Product> insertedProducts = new Stack<Product>();
    Stack<Product> releasedProducts = new Stack<Product>();

    private void Awake()
    {
        objType = OBJ_TYPE.Processing;

        processingEnterCircle.color = Color.red;
    }

    // Start is called before the first frame update
    void Start()
    {
        insertArea.callback = InsertProduct;
        processingArea.enterCallback = WorkerIn;
        processingArea.callback = ProcessingProduct;
        processingArea.exitCallback = WorkerOut;
        releaseArea.callback = ReleaseProduct;
    }

    public override void SetData(ushort lv, ushort decoIdx)
    {
        base.SetData(lv, decoIdx);

        if (lv != 0)
        {
            insertArea.gameObject.SetActive(true);
            processingArea.gameObject.SetActive(true);
            releaseArea.gameObject.SetActive(true);
        }
        else
        {
            insertArea.gameObject.SetActive(false);
            processingArea.gameObject.SetActive(false);
            releaseArea.gameObject.SetActive(false);
        }
    }

    private void InsertProduct(Unit unit)
    {
        if(unit.GetProductData() == insertProductData)
        {
            Product product = unit.GetProduct();
            if(product != null)
            {
                if(product)
                product.transform.SetParent(insertedProductContainerTR);
                product.transform.localPosition = new Vector3(0, 0.18f + (insertedProducts.Count * product.H), 0);
                insertedProducts.Push(product);
            }
        }
    }

    private void WorkerIn(Unit unit)
    {
        if (worker == null)
        {
            if (unit as Worker)
            {
                worker = unit as Worker;
                readyWorker = null;
                processingEnterCircle.color = Color.green;
            }
        }
    }

    private void ProcessingProduct(Unit unit)
    {
        if (worker == null)
        {
            WorkerIn(unit);
        }

        if (worker != null)
        {
            if (insertedProducts.Count >= insertCountForRelease)
            {
                for (int i = 0; i < insertCountForRelease; ++i)
                {
                    Product product = insertedProducts.Pop();
                    Root.Resources.ReleaseProduct(product);
                }

                Product release = Root.Resources.GetProduct(releaseProductData);
                release.transform.SetParent(releasedProductContainerTR);
                release.transform.localPosition = new Vector3(0, 0.18f + (releasedProducts.Count * release.H), 0);
                release.gameObject.SetActive(true);
                releasedProducts.Push(release);
            }
        }
    }


    private void WorkerOut(Unit unit)
    {
        if (worker != null)
        {
            if (unit as Worker == worker)
            {
                worker = null;
                readyWorker = null;
                processingEnterCircle.color = Color.red;
            }
        }
    }

    private void ReleaseProduct(Unit unit)
    {
        if(releasedProducts.Count != 0)
        {
            if(unit.InputProduct(releasedProducts.Peek()))
            {
                releasedProducts.Pop();
            }
        }
    }


    public override bool IsNeedWorker()
    {
        if (insertedProducts.Count >= insertCountForRelease)
        {
            if (worker == null && readyWorker == null)
                return true;
        }

        return false;
    }



    public Vector3 InsertPos()
    {
        return insertArea.transform.position;
    }

    public Vector3 ReleasePos()
    {
        return releaseArea.transform.position;
    }

    public Vector3 ProcessPos()
    {
        return processingArea.transform.position;
    }

    public bool HaveReleaseProduct()
    {
        return releasedProducts.Count != 0;
    }
}

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
        releaseArea.callback = ReleaseProduct;
    }

    public override void Initialize()
    {
        base.Initialize();

        insertArea.gameObject.SetActive(false);
        processingArea.gameObject.SetActive(false);
        releaseArea.gameObject.SetActive(false);
    }

    private void InsertProduct(Unit unit)
    {
        if(unit.GetProductData() == insertProductData)
        {
            Product product = unit.GetProduct();
            if(product != null)
            {
                if(product)
                insertedProducts.Push(product);

                product.transform.SetParent(insertedProductContainerTR);
                product.transform.localPosition = Vector3.up * insertedProducts.Count;
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
                release.transform.localPosition = Vector3.up * releasedProducts.Count;
                releasedProducts.Push(release);
            }
        }
    }


    private void CasherOut(Unit unit)
    {
        if (worker != null)
        {
            if (unit as Worker == worker)
            {
                worker = null;
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

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProductPile : ObjectBase
{

    [SerializeField] UnitTriggerArea triggerArea;
    [SerializeField] Transform producdtContainerTR;

    Stack<Product> products = new Stack<Product>();

    public ProductScriptableObject productData;
    public int productCount;

    private void Awake()
    {
        objType = OBJ_TYPE.ProductPile;
    }

    public void Show()
    {
        triggerArea.SetTimeLimit(0.01f);
        triggerArea.enterCallback = GiveProduct;

        SetProduct(productCount);
    }

    //public void SetData(ProductData data, int count)
    //{
    //    productData = data;
    //    productCount = count;

    //    SetProduct(count);
    //}

    public void SetProduct(int count)
    {
        Product product;

        for(int i = 0; i < count; ++i)
        {
            product = Root.Resources.GetProduct(productData);
            products.Push(product);

            product.transform.SetParent(producdtContainerTR);
            product.transform.localPosition = Config.GetMoneyPos(i);
            product.gameObject.SetActive(true);
        }
    }

    private void GiveProduct(Unit obj)
    {
        Config.GiveMoneyToPlayer(obj as Player, products);

        Destroy(gameObject);
    }
}

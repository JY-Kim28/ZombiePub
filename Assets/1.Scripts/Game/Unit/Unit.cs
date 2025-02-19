using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    [SerializeField] protected Animator animator;

    [SerializeField] protected Transform productsTR;

    protected Stack<Product> products;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        products = new Stack<Product>();
        productsTR.gameObject.SetActive(false);
    }

    public virtual bool InputProduct(Product product)
    {
        if (products.Count == 0 || (products.Peek().Data == product.Data))
        {
            product.transform.SetParent(productsTR);
            product.transform.localPosition = new Vector3(0, (products.Count * product.H), 0);
            products.Push(product);

            productsTR.gameObject.SetActive(true);

            animator?.SetBool("Work", true);

            return true;
        }

        return false;
    }

    public ProductScriptableObject GetProductData()
    {
        if (products.Count == 0) return default;

        return products.Peek().Data;
    }

    public virtual Product GetProduct()
    {
        Product product = null;

        if (products.Count != 0)
        {
            product = products.Pop();

            DrawStack();
        }

        return product;
    }

    protected virtual void DrawStack()
    {
        int count = products.Count;

        productsTR.gameObject.SetActive(count != 0);
    }

    public int GetProductsCount()
    {
        return products.Count;
    }
}

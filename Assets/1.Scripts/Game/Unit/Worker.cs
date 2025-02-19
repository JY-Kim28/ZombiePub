using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Worker : Unit
{
    [SerializeField] GameObject max;

    public WorkerStat Stat { get; protected set; }

    protected override void Awake()
    {
        base.Awake();

        max.gameObject.SetActive(false);
    }

    public override bool InputProduct(Product product)
    {
        if (products.Count >= Stat.capacity) return false;

        if (products.Count == 0 || (products.Peek().Data == product.Data))
        {
            product.transform.SetParent(productsTR);
            product.transform.localPosition = new Vector3(0, (products.Count * product.H), 0);
            product.gameObject.SetActive(true);
            products.Push(product);

            if(animator != null)
                animator.SetBool("Work", true);

            productsTR.gameObject.SetActive(true);
            max.SetActive(products.Count >= Stat.capacity);

            return true;
        }

        return false;
    }


    public bool InputTrash(Vector3 pos, int count)
    {
        if (count == 0) return false;

        if(products.Count == 0 || products.Peek().Data == Game.Stage.productPrefabs[1].Data)
        {
            Product obj;
            ProductScriptableObject productData = Game.Stage.productPrefabs[1].Data;

            float delay = 0.5f / count;

            for (int i = 0; i < count; ++i)
            {
                obj = Root.Resources.GetProduct(productData);
                obj.transform.SetParent(productsTR);
                obj.transform.position = pos;
                obj.transform.DOLocalJump(new Vector3(0, (products.Count * obj.H), 0), 3, 1, 0.1f).SetDelay(delay * i);

                products.Push(obj);

                obj.gameObject.SetActive(true);
            }

            if(animator != null)
                animator.SetBool("Work", true);

            productsTR.gameObject.SetActive(true);
            max.SetActive(false);

        }
        return false;
    }


    public virtual void RemoveAllProduct(Transform parent, Vector3 pos)
    {
        int count = products.Count;
        if (count == 0) return;

        float delay = 0.5f / count;

        for(int i = 0; i < count; ++i)
        {
            Product product = products.Pop();
            product.transform.SetParent(parent);
            product.transform.DOJump(pos, 3, 1, 0.01f).SetDelay(delay * i).onComplete = () =>
            {
                Root.Resources.ReleaseProduct(product);
            };
        }

        productsTR.gameObject.SetActive(false);

        if(animator != null)
            animator.SetBool("Work", false);

    }

    protected override void DrawStack()
    {
        int count = products.Count;

        productsTR.gameObject.SetActive(count != 0);
        max.SetActive(products.Count >= Stat.capacity);
    }
}

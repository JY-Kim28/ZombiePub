using UnityEngine;
using DG.Tweening;

public class CustomerCar : Customer
{
    public override void SetDestination(int posIdx, Vector3 pos)
    {
        base.SetDestination(posIdx, pos);

        transform.DOMove(pos, 1f).SetEase(Ease.Linear);
        
        if (animator != null)    
            animator.SetBool("Move", true);
    }

    private void Update()
    {
        transform.LookAt(targetPos);
    }

    public override void Take(Product product)
    {
        Count--;

        scriptBubble.DrawCount(Count);

        InputProduct(product);

        if (Count == 0)
        {
            scriptBubble.Hide();

            sellingMachine.CustomerOut(this);
            sellingMachine = null;

            SetStateGoToOutPoint();
        }
    }


    public override bool InputProduct(Product product)
    {
        if (products.Count == 0 || (products.Peek().Data == product.Data))
        {
            //product.transform.SetParent(productsTR);
            //product.transform.localPosition = new Vector3(0, (products.Count * product.H), 0);
            //products.Push(product);

            Root.Resources.ReleaseProduct(product);

            return true;
        }

        return false;
    }

}

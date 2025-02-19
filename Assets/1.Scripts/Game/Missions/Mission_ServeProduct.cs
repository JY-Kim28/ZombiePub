using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Mission_ServeProduct : Mission
{
    [SerializeField] ProductScriptableObject productData;
    [SerializeField] SellingMachine machine;

    public override void Play(Action completeCallback)
    {
        machine.insertCallback += InsertProduct;

        base.Play(completeCallback);
    }

    private void InsertProduct(Unit unit, ProductScriptableObject unitProductData)
    {
        if (productData == null || unit as Player == null || unit.GetProductData() == null)
            return;

        if (unitProductData == productData)
        {
            ++playCount;
        }
    }

    //protected override IEnumerator CheckMissionState()
    //{
    //    while (playCount < value)
    //    {
    //        yield return null;
    //    }

    //    machine.insertCallback -= InsertProduct;

    //    Complete();
    //}

    protected override void PlayEvent()
    {
        Events().Forget();
    }

    private async UniTaskVoid Events()
    {
        await UniTask.WaitUntil(() => playCount >= value);

        machine.insertCallback -= InsertProduct;

        Complete();
    }

}

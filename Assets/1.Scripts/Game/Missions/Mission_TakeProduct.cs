using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Mission_TakeProduct : Mission
{
    [SerializeField] ProductScriptableObject productData;

    //protected override IEnumerator CheckMissionState()
    //{
    //    while (Game.Player.GetProductData() != productData || Game.Player.GetProductsCount() < value)
    //    {
    //        yield return null;
    //    }

    //    Complete();
    //}

    protected override void PlayEvent()
    {
        Events().Forget();
    }

    private async UniTaskVoid Events()
    {
        await UniTask.WaitUntil(() => Game.Player.GetProductData() == productData && Game.Player.GetProductsCount() >= value);

        Complete();
    }
}

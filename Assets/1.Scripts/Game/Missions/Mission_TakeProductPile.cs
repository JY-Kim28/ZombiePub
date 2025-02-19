using System.Collections;
using Cysharp.Threading.Tasks;

public class Mission_TakeProductPile : Mission
{
    //protected override IEnumerator CheckMissionState()
    //{
    //    while(playCount != value)
    //    {
    //        if(Game.Stage.pile == null)
    //        {
    //            ++playCount;
    //        }

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
        await UniTask.WaitUntil(() => Game.Stage.pile == null);

        Complete();
    }
}

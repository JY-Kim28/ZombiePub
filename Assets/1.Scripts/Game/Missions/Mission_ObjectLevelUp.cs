
using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Mission_ObjectLevelUp : Mission
{
    [SerializeField] ObjectBase targetObj;

    //protected override IEnumerator CheckMissionState()
    //{
    //    while (targetObj.currLv != value)
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
        await UniTask.WaitUntil(() => targetObj.currLv >= value);

        Complete();
    }
}

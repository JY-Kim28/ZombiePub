using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Mission_Arrive : Mission
{
    public Vector3 targetPos;
    public float minDistance = 1;

    private CancellationTokenSource cancel;

    protected override void PlayEvent()
    {
        Events().Forget();
    }

    private async UniTaskVoid Events()
    {
        cancel = new CancellationTokenSource();

        await UniTask.WaitUntil(() => Vector3.SqrMagnitude(targetPos - Game.Player.transform.position) <= minDistance, cancellationToken: cancel.Token, cancelImmediately:true);

        Complete();
    }

    private void OnDestroy()
    {
        cancel?.Cancel();
        cancel = null;
    }
}

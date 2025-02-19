using System.Collections;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public abstract class Mission : MonoBehaviour
{
    public int value;
    public float startDelaySeconds = 0;

    public Vector3 arrrowPos;

    protected Action completeCallback;

    protected int playCount;

    public string missionTextCode;


    public virtual void Play(Action completeCallback)
    {
        this.completeCallback = completeCallback;

        playCount = 0;

        PlayAsync().Forget();
    }

    private async UniTaskVoid PlayAsync()
    {
        await UniTask.Delay((int)(startDelaySeconds * 1000));

        if(missionTextCode != null)
        {
            Game.UI.ShowMissionText(missionTextCode);
        }
        else
        {
            Game.UI.HideMissionText();
        }

        Game.Stage.mission.ShowArrow(arrrowPos);

        PlayEvent();
    }

    protected virtual void PlayEvent()
    {

    }

    public void Stop()
    {
        completeCallback = null;
    }

    public void Complete()
    {
        completeCallback?.Invoke();

        Stop();
    }
}

using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Mission_CleanUsingMachine : Mission
{
    [SerializeField] UsingMachine usingMachine;

    protected override void PlayEvent()
    {
        Events().Forget();
    }

    private async UniTaskVoid Events()
    {
        Game.UI.HideMissionText();
        Game.Stage.mission.HideArrow();

        await UniTask.WaitUntil(() => usingMachine.trashCount != 0);

        Game.UI.ShowMissionText();
        Game.Stage.mission.ShowArrow(arrrowPos);

        await UniTask.WaitUntil(() => usingMachine.trashCount == 0);

        Complete();
    }
}

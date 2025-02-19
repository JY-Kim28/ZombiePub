using System;
using UnityEngine;
using UniRx;
using Cinemachine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class Game : Singleton<Game>
{
    [SerializeField] GameUI ui;
    public static GameUI UI => Instance.ui;

    [SerializeField] Player player;
    public static Player Player => Instance.player;

    [SerializeField] Stage stage;
    public static Stage Stage => Instance.stage;

    [SerializeField] CinemachineFreeLook playerCamera;
    [SerializeField] CinemachineFreeLook targetCamera;

    private void Start()
    {
        UI.Initialize();

        targetCamera.gameObject.SetActive(false);

        stage.LoadResources(OnLoadedProductPrefab);
    }

    private void OnLoadedProductPrefab()
    {
        var lvData = Root.Resources.LoadStage(1);
        stage.SetLevelData(lvData);
    }

    public async UniTaskVoid ShowTargetAsync(List<Transform> trs)
    {
        targetCamera.gameObject.SetActive(true);

        while (trs.Count != 0)
        {
            targetCamera.Follow = trs[0];
            targetCamera.LookAt = trs[0];

            await UniTask.Delay(2000);

            trs.RemoveAt(0);
        }

        targetCamera.gameObject.SetActive(false);
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DopaminTable;
using DG.Tweening;
using UnityEngine.Localization.Settings;

public class Root : Singleton<Root>
{
    [SerializeField] Resources resources;
    public static Resources Resources => Instance.resources;

    UserInfo userInfo;
    public static UserInfo UserInfo => Instance.userInfo;

    ushort firstLoadCount = 0;


    private void Start()
    {
        Application.targetFrameRate = 60;

        userInfo = new UserInfo();
        userInfo.Initialize();

        DOTween.Init();


        if(Application.systemLanguage == SystemLanguage.Korean)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
        }
        else
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        }


        LoadResources();
        CreateTables();

        firstLoadCount = 2;
    }

    private void LoadResources()
    {
        Resources.LoadResource(OnLoadResources);
    }

    private void OnLoadResources(float obj)
    {
        if (obj == 1)
        {
            OnCompleteFirstLoad();
        }
    }

    private void CreateTables()
    {
        Tables.Create();
        Tables.Load(OnTableLoad);
    }

    private void OnTableLoad(int curr, int total)
    {
        if (curr == total)
        {
            OnCompleteFirstLoad();
        }
    }

    private void OnCompleteFirstLoad()
    {
        --firstLoadCount;
        if (firstLoadCount == 0)
        {
            StartCoroutine(LoadMainScene());
        }
    }


    IEnumerator LoadMainScene()
    {
        var async = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

        while (!async.isDone)
        {
            yield return null;
        }
    }


    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            StateSave();
        }
    }

    private void OnApplicationQuit()
    {
        StateSave();
    }

    private void StateSave()
    {
        userInfo.Save();
    }
}

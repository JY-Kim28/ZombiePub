using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupAttribute : Attribute
{
    public string _ClassName;

    public UIPopupAttribute(string className)
    {
        _ClassName = className;
    }
}

public class UIPopupBase : UIBase
{
    [SerializeField] protected Button _CloseBtn;
    public Canvas Canvas { private set; get; }

    protected override void OnAwake()
    {
        base.OnAwake();

        Canvas = GetComponent<Canvas>();

        if(_CloseBtn != null)
            _CloseBtn.onClick.AddListener(OnClickCloseBtn);

        if (showTween != null)
            showTween.IsUnscaled = true;

        if (hideTween != null)
            hideTween.IsUnscaled = true;
    }

    public virtual void OnClickCloseBtn()
    {
        Hide();
    }

    public override void Show()
    {
        base.Show();
    }


    protected virtual void OnEnable()
    {
        //if (Root.UserInfo != null)
        //    Root.UserInfo.OnChangedLangauge += OnChangedLanguage;
    }

    protected virtual void OnDisable()
    {
        //if(Root.UserInfo != null)
        //{
        //    Root.UserInfo.OnChangedLangauge -= OnChangedLanguage;
        //}
    }

    protected virtual void OnChangedLanguage()
    {

    }
}

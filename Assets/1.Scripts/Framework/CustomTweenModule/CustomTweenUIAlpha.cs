using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CustomTweenUIAlpha : CustomTween
{
    public float startAlpha = 0;
    public float targetAlpha = 0;

    [SerializeField] CanvasGroup _cg;
    protected CanvasGroup cg
    {
        get
        {
            if (_cg == null)
                _cg = GetComponent<CanvasGroup>();
            return _cg;
        }
    }

    protected override void StartTween()
    {
        cg.alpha = startAlpha;

        _tween = cg.DOFade(targetAlpha, duration);
        _tween.SetEase(ease);
        _tween.SetUpdate(IsUnscaled);
        _tween.onComplete = OnEndTween;
    }

    public override void SetTargetState()
    {
        cg.alpha = targetAlpha;
    }
}

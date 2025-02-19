using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CustomTweenUIScale : CustomTween
{
    public Vector3 startScale = Vector3.one;
    public Vector3 targetScale = Vector3.one;

    [SerializeField] RectTransform _rectTransform;
    protected RectTransform rt
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    protected override void StartTween()
    {
        rt.localScale = startScale;

        _tween = rt.DOScale(targetScale, duration);
        _tween.SetEase(ease);
        _tween.SetUpdate(IsUnscaled);
        _tween.onComplete = OnEndTween;
    }

    public override void SetTargetState()
    {
        rt.localScale = targetScale;
    }
}

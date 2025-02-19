using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CustomTweenUIColor : CustomTween
{
    public Color startValue = Color.white;
    public Color targetValue = Color.white;

    [SerializeField] Image _img;
    protected Image img
    {
        get
        {
            if (_img == null)
                _img = GetComponent<Image>();
            return _img;
        }
    }

    protected override void StartTween()
    {
        img.color = startValue;

        _tween = img.DOColor(targetValue, duration);
        _tween.SetEase(ease);
        _tween.SetUpdate(IsUnscaled);
        _tween.onComplete = OnEndTween;
    }

    public override void SetTargetState()
    {
        img.color = targetValue;
    }
}

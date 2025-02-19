using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CustomTweenMoveY : CustomTween
{
    public Vector3 startPosition;
    public Vector3 targetPosition;

    [SerializeField] RectTransform _target = null;
    protected RectTransform targetTransform
    {
        get
        {
            if (_target == null)
                _target = GetComponent<RectTransform>();

            return _target;
        }
    }

    protected override void StartTween()
    {
        targetTransform.position = new Vector2(targetTransform.position.x, startPosition.y);

        _tween = targetTransform.DOMoveY(targetPosition.y, duration, false);
        _tween.SetEase(ease);
        _tween.SetUpdate(IsUnscaled);
        _tween.onComplete = OnEndTween;
    }

    public override void SetTargetState()
    {
        targetTransform.position = new Vector2(targetTransform.position.x, targetPosition.y);
    }
}

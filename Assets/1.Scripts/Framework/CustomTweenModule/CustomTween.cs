using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public abstract class CustomTween : MonoBehaviour
{
    public string tweenName = "";

    public float duration = 0.2f;
    public Ease ease = Ease.OutCubic;
    public UnityEvent onEndEndCallback;
    protected bool _isPause = false;
    protected Tween _tween;

    public bool IsUnscaled = false;

    protected abstract void StartTween();
    public abstract void SetTargetState();

    public void Play()
    {
        _tween?.Kill();

        _isPause = false;

        StartTween();
    }

    protected void OnEndTween()
    {
        _tween = null;
        onEndEndCallback?.Invoke();
    }

    public void Stop()
    {
        if(_tween != null)
        {
            _isPause = false;

            _tween.Kill();
            _tween = null;
        }
    }

    public void Pause()
    {
        _isPause = true;
        _tween.Pause();
    }

    public void Resume()
    {
        if(_isPause)
        {
            _tween.Play().SetUpdate(IsUnscaled);
            _isPause = false;
        }
    }
}

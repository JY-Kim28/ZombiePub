using UnityEngine;
using System;

public class UIBase : MonoBehaviour
{
    protected Animator _animator = null;

    public CustomTween showTween;
    public CustomTween hideTween;

    public Action OnEndShowCallback;
    public Action OnEndHideCallback;
    public Action OnBackButtonCallback;

    private void Awake()
    {
        TryGetComponent<Animator>(out _animator);

        OnAwake();
    }

    protected virtual void OnAwake()
    {
        if(showTween != null)
            showTween.onEndEndCallback.AddListener(OnEndShow);

        if (hideTween != null)
            hideTween.onEndEndCallback.AddListener(OnEndHide);
    }

    protected virtual void OnEndShow()
    {
        OnEndShowCallback?.Invoke();
        OnEndShowCallback = null;
    }

    protected virtual void OnEndHide()
    {
        gameObject.SetActive(false);

        OnEndHideCallback?.Invoke();
        OnEndHideCallback = null;
    }

    public virtual void Show()
    {
        if (gameObject.activeSelf)
            return;

        gameObject.SetActive(true);

        if (hideTween != null)
        {
            hideTween.Stop();
        }

        if (showTween != null)
        {
            showTween.Play();
        }
        else if(_animator)
        {
            _animator.Play("Show");
        }
        else
        {
            OnEndShowCallback?.Invoke();
            OnEndShowCallback = null;
        }
    }


    public virtual void Hide(bool force = false)
    {
        if (gameObject.activeSelf == false)
            return;

        if (force)
        {
            if (showTween != null)
            {
                showTween.Stop();
            }

            if (hideTween != null)
            {
                hideTween.Stop();
                hideTween.SetTargetState();
            }

            OnEndHide();
        }
        else
        {
            if (showTween != null)
            {
                showTween.Stop();
            }

            if (hideTween != null)
            {
                hideTween.Play();
            }
            else if (_animator)
            {
                _animator.Play("Hide");
            }
            else
            {
                gameObject.SetActive(false);

                OnEndHideCallback?.Invoke();
                OnEndHideCallback = null;
            }
        }
    }


}

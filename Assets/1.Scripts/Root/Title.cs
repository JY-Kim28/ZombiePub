using System;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] Slider _Progress;

    
    public void SetVisibleProgress(bool visible)
    {
        _Progress.gameObject.SetActive(visible);
    }

    public void SetProgress(float progress)
    {
        _Progress.value = progress;
    }

}
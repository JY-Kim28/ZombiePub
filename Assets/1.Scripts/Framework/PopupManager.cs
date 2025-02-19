using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PopupManager : Singleton<PopupManager>
{
    private List<UIPopupBase> _Popups = new List<UIPopupBase>();
    private List<UIPopupBase> _ActivePopups = new List<UIPopupBase>();

    public void ShowPopup<T>(Action<T> openCallback = null, Action closeCallback = null, Action backButtonCallback = null) where T : UIPopupBase
    {
        T popup = (T)_Popups.Find(p => p.GetType() == typeof(T));
        if(popup != null)
        {
            if (_ActivePopups.Contains(popup))
                return;

            popup.OnEndHideCallback = () =>
            {
                OnHidePopup(popup);

                closeCallback?.Invoke();
            };

            popup.OnBackButtonCallback = backButtonCallback;

            popup.Show();

            _ActivePopups.Add(popup);

            openCallback?.Invoke(popup);

            SortActivePopups();
        }
        else
        {
            StartCoroutine(LoadPopup<T>(openCallback, closeCallback));
        }
    }

    private IEnumerator LoadPopup<T>(Action<T> callback, Action closeCallback = null) where T : UIPopupBase
    {
        var attr = typeof(T).GetTypeInfo().GetCustomAttribute<UIPopupAttribute>();
        var handler = Addressables.LoadAssetAsync<GameObject>(attr._ClassName);
        yield return handler;

        if (handler.Status == AsyncOperationStatus.Succeeded)
        {
            var obj = Instantiate(handler.Result);
            obj.transform.SetParent(transform);

            var rect = obj.GetComponent<RectTransform>();
            rect.localScale = Vector3.one;
            rect.offsetMin = Vector3.zero;
            rect.offsetMax = Vector3.one;

            obj.SetActive(false);

            var canvas = obj.GetComponent<Canvas>();
            canvas.overrideSorting = true;


            Addressables.Release(handler);

            T popup = obj.GetComponent<T>();
            popup.OnEndHideCallback = () =>
            {
                OnHidePopup(popup);

                closeCallback?.Invoke();
            };

            _Popups.Add(popup);

            popup.Show();




            _ActivePopups.Add(popup);

            SortActivePopups();

            callback?.Invoke(popup);

        }
    }


    public void HidePopup<T>() where T : UIPopupBase
    {
        T popup = (T)_Popups.Find(p => p.GetType() == typeof(T));
        if (popup != null)
        {
            _ActivePopups.Remove(popup);

            popup.Hide();

            SortActivePopups();
        }
    }


    private void OnHidePopup<T>(T popup)
    {
        _ActivePopups.Remove(popup as UIPopupBase);

        SortActivePopups();
    }

    private void SortActivePopups()
    {
        int order = GetComponent<Canvas>().sortingOrder;
        _ActivePopups.ForEach(p => p.GetComponent<Canvas>().sortingOrder = ++order);
    }


    public bool BackButton()
    {
        if(_ActivePopups.Count > 0 )
        {
            UIPopupBase popup = _ActivePopups[^1];

            if(popup.OnBackButtonCallback != null)
            {
                popup.OnBackButtonCallback();
            }
            else
            {
                popup.OnClickCloseBtn();
            }

            return true;
        }

        return false;
    }

}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class TouchUtils
{

    public enum E_TOUCH_TYPE
    {
        NONE = 0,
        DOWN,
        UP,
        HELD
    }



    public static bool IsSingleTouch()
    {
        return Input.touchCount <= 1;
    }

    public static bool IsTouchDown()
    {
#if UNITY_EDITOR
        if (UnityEngine.Device.SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetMouseButtonDown(0))
                return true;
        }
        else
        {
            if (Input.touchCount == 1)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    return true;
                }
            }
        }
#else
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                return true;
            }
        }
#endif

        return false;
    }

    public static bool IsTouchHeld()
    {
#if UNITY_EDITOR
        if (UnityEngine.Device.SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetMouseButton(0))
                return true;
        }
        else
        {
            if (Input.touchCount == 1)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Stationary)
                {
                    return true;
                }
            }
        }
#else
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                return true;
            }
        }
#endif
        return false;
    }

    public static bool IsTouchUp()
    {
#if UNITY_EDITOR
        if (UnityEngine.Device.SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetMouseButtonUp(0))
                return true;
        }
        else
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    return true;
                }
            }
        }

#else
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                return true;
            }
        }
#endif
        return false;
    }


    public static bool IsUITouch()
    {
#if UNITY_EDITOR
        return EventSystem.current.IsPointerOverGameObject(-1);
#else
        return EventSystem.current.IsPointerOverGameObject(0);
#endif
    }

    public static Vector3 GetTouchScreenPoint()
    {
        Vector3 touchPos;

#if UNITY_EDITOR
        if (UnityEngine.Device.SystemInfo.deviceType == DeviceType.Desktop)
        {
            touchPos = Camera.main.WorldToScreenPoint(Input.mousePosition);
        }
        else
        {
            if (Input.touchCount > 0)
                touchPos = Input.GetTouch(0).position;
            else
                touchPos = Vector3.one;
        }
#else
             if (Input.touchCount > 0)
                touchPos = Input.GetTouch(0).position;
            else
                touchPos = Vector3.one;
#endif

        return touchPos;
    }


    public static List<RaycastResult> GetRayCastResultList(Vector3 screenTouchPos)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = screenTouchPos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results;
    }

    public static void PrintTouchedObjs()
    {
        if (TouchUtils.IsTouchDown())
        {
            if (TouchUtils.IsUITouch())
            {
                Debug.Log("======== PrintTouchedObjs Start");
                List<RaycastResult> results = GetRayCastResultList(GetTouchPoint());

                if (results.Count > 0)
                {
                    foreach (var r in results)
                    {
                        Debug.Log(r.gameObject.name);
                    }
                }

                Debug.Log("======== PrintTouchedObjs End");
            }
        }
    }

    public static bool IsTouchOtherAreaOfTarget(List<GameObject> objs)
    {
        if (TouchUtils.IsTouchDown())
        {
            if (TouchUtils.IsUITouch())
            {
                List<RaycastResult> results = GetRayCastResultList(GetTouchPoint());

                if (results.Count > 0)
                {
                    foreach (var r in results)
                    {
                        if(objs.Find(x=>x.gameObject == r.gameObject))
                            return false;
                    }
                }

                return true;
            }

            return true;
        }

        return false;
    }

    public static bool IsTouchOtherAreaOfTarget(GameObject obj)
    {
        if (TouchUtils.IsTouchDown())
        {
            if (TouchUtils.IsUITouch())
            {
                List<RaycastResult> results = GetRayCastResultList(GetTouchPoint());
                if (results.Count > 0)
                {
                    foreach (var r in results)
                    {
                        if (obj == r.gameObject)
                            return false;
                    }
                }

                return true;
            }

            return true;
        }

        return false;
    }


    public static void GetObjectsOnTouchPoint(E_TOUCH_TYPE touchType, out List<GameObject> objs)
    {
        objs = new List<GameObject>();

        if (MatchingTouchType(touchType) == false)
            return;

        Vector3 touchPos = GetTouchPoint();

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = touchPos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        if (results.Count > 0)
        {
            foreach (var r in results)
            {
                objs.Add(r.gameObject);
            }
        }
    }

    public static void GetUIOnMousePoint(out GameObject[] objs)
    {
        Vector3 touchPos = GetTouchPoint();

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = touchPos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        objs = new GameObject[results.Count];
        if (results.Count > 0)
        {
            for(int i = results.Count - 1; i != -1; i--)
            {
                objs[i] = results[i].gameObject;
            }
        }
    }

    public static void GetGameObjectOnMousePoint(out GameObject[] objs, int layerMask = 0)
    {
        Vector3 touchPos = GetTouchPoint();

        RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(touchPos), Vector2.zero, 1000);
        objs = new GameObject[hit.Length];
        for(int i = 0; i < hit.Length; i++)
        {
            objs[i] = hit[i].collider.gameObject;
        }
    }


    public static bool IsTouchObject(E_TOUCH_TYPE touchType, string objName)
    {
        if (MatchingTouchType(touchType) == false)
            return false;


        Vector3 touchPos = GetTouchPoint();

        Camera camera = Camera.allCameras[^1];

        Ray ray = camera.ScreenPointToRay(touchPos);

        RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector3(ray.origin.x, ray.origin.y, -10), camera.transform.forward);

        int max = hits.Length;
        for (int i = 0; i < max; ++i)
        {
            if (hits[i].collider != null)
            {
                if (hits[i].collider.gameObject.name.Equals(objName))
                    return true;
            }
        }

        return false;
    }


    public static bool IsTouchObjectByScreenPoint(E_TOUCH_TYPE touchType, GameObject obj)
    {
        if (MatchingTouchType(touchType) == false)
            return false;

        Vector3 touchPos = GetTouchPoint();

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = touchPos;

        return RectTransformUtility.RectangleContainsScreenPoint((obj.transform as RectTransform), touchPos);
    }


    public static Vector3 GetTouchPoint()
    {
#if UNITY_EDITOR
        if (UnityEngine.Device.SystemInfo.deviceType == DeviceType.Desktop)
        {
            return Input.mousePosition;
        }
        else
        {
            if (Input.touchCount > 0)
                return Input.GetTouch(0).position;
            else
                return Vector3.one;
        }

#else
        if (Input.touchCount > 0)
                return Input.GetTouch(0).position;
            else
                return Vector3.one;
#endif
    }


    public static bool MatchingTouchType(E_TOUCH_TYPE touchType)
    {
        if (touchType == E_TOUCH_TYPE.NONE)
            return false;
        if (touchType == E_TOUCH_TYPE.DOWN && TouchUtils.IsTouchDown())
            return true;
        if (touchType == E_TOUCH_TYPE.UP && TouchUtils.IsTouchUp())
            return true;
        if (touchType == E_TOUCH_TYPE.HELD && TouchUtils.IsTouchHeld())
            return true;

        return false;
    }
}

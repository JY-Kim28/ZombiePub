using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTriggerArea : MonoBehaviour
{
    public Action<Unit> exitCallback;
    public Action<Unit> callback;
    public Action<Unit> enterCallback;

    public float time = 0;
    public float timeLimit { get; private set; } = 0.1f;


    public void SetTimeLimit(float value)
    {
        timeLimit = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        time = 0;

        if (other.gameObject.TryGetComponent<Unit>(out Unit unit))
        {
            enterCallback?.Invoke(unit);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        time = 0;

        if (other.gameObject.TryGetComponent<Unit>(out Unit unit))
        {
            exitCallback?.Invoke(unit);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (time <= 0)
        {
            time = timeLimit;

            if(other.gameObject.TryGetComponent<Unit>(out Unit unit))
            {
                callback?.Invoke(unit);
            }
        }
        else
        {
            time -= Time.deltaTime;
        }
    }
}

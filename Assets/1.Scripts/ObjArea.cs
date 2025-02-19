using System.Collections;
using System;
using UnityEngine;

public class ObjArea : MonoBehaviour
{
    public Transform containerTR;
    public Action onTriggerWithPlayerCallback;

    public float time = 0;

    private void OnTriggerExit(Collider other)
    {
        time = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if(time <= 0)
        {
            time = 0.1f;

            if (other.gameObject.CompareTag("Player"))
            {
                onTriggerWithPlayerCallback?.Invoke();
            }
        }
        else
        {
            time -= Time.deltaTime;
        }
    }
}

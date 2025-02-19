using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeHR : ObjectBase
{
    [SerializeField] UnitTriggerArea triggerArea;

    [SerializeField] Transform employeePoint;

    private void Awake()
    {
        objType = OBJ_TYPE.Office_HR;
    }

    private void Start()
    {
        triggerArea.enterCallback = OnEnter;
    }

    private void OnEnter(Unit obj)
    {
        if(obj as Player)
        {
            PopupManager.Instance.ShowPopup<PopupEmployeeUpgrade>();
        }
    }
}

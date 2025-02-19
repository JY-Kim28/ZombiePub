using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeGM : ObjectBase
{
    [SerializeField] UnitTriggerArea triggerArea;

    private void Awake()
    {
        objType = OBJ_TYPE.Office_GM;
    }

    private void Start()
    {
        triggerArea.enterCallback = OnEnter;
    }

    private void OnEnter(Unit obj)
    {
        if (obj as Player)
        {
            PopupManager.Instance.ShowPopup<PopupPlayerUpgrade>();
        }
    }
}

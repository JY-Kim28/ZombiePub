using UnityEngine;
using UnityEngine.UI;


[UIPopup("PopupCheatLvUP")]
public class PopupCheatLvUP : UIPopupBase
{
    public int machineCapacityUpCount = 3;
    [SerializeField] Button machineCapacityUp;
    public float playerSpeedUpValue = 3;
    [SerializeField] Button playerSpeedUp;

    private void Start()
    {
        machineCapacityUp.onClick.AddListener(OnClickMachineCapacityBtn);
        playerSpeedUp.onClick.AddListener(OnClickPlayerSpeedUpBtn);
    }

    private void OnClickMachineCapacityBtn()
    {
        foreach(var obj in Game.Stage.objectList)
        {
            if (obj.currLv == 0)
                continue;

            if(obj.objType == OBJ_TYPE.Manufacture)
            {
                (obj as ManufactureMachine).productLimit = 10000000;
#if UNITY_EDITOR
                //(obj as ManufactureMachine).makeCount += machineCapacityUpCount;

                for(int i = 0; i < machineCapacityUpCount; ++i)
                {
                    (obj as ManufactureMachine).AddObj();
                }
#endif
            }
        }

    }

    private void OnClickPlayerSpeedUpBtn()
    {
        (Game.Player.Stat as PlayerStat).AddSpeedUp(playerSpeedUpValue);
    }
}


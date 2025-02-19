using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UIPopup("PopupEmployeeUpgrade")]
public class PopupEmployeeUpgrade : UIPopupBase
{
    [SerializeField] CellEmployeeUpgrade[] cells;

    public override void Show()
    {
        base.Show();

        DrawInfo();
    }

    private void DrawInfo()
    {
        for (int i = 0; i < Game.Stage.employeeLimit; ++i)
        {
            cells[i].SetData(i, Game.Stage.employees[i] == null ? null : Game.Stage.employees[i].Stat, LevelUp);
        }
    }

    public void LevelUp(int idx, WorkerStat stat, bool ads)
    {
        if(stat == null)
        {
            Game.Stage.CreateEmployee();

            DrawInfo();

            return;
        }

        uint price = (uint)((idx * 40) + ((stat.lv / 3) * 40));

        if(ads || Root.UserInfo.MinusMoney(price))
        {
            stat.LevelUp();

            foreach(var c in cells)
            {
                c.DrawStats();
            }
        }
    }
}

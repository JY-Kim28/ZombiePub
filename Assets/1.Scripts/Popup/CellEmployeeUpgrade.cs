using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CellEmployeeUpgrade : MonoBehaviour
{
    [SerializeField] CellEmployeeStatLevel[] statCells;
    [SerializeField] Button upgradeBtn;
    [SerializeField] TextMeshProUGUI upgradeText;
    [SerializeField] Button adBtn;
    [SerializeField] Button buyBtn;
    [SerializeField] GameObject max;
    [SerializeField] GameObject dim;

    int idx;
    WorkerStat stat;
    Action<int, WorkerStat, bool> lvUpCallback;

    private void Start()
    {
        upgradeBtn.onClick.AddListener(OnClickLvUp);
        adBtn.onClick.AddListener(OnClickAdsLvUp);
        buyBtn.onClick.AddListener(OnClickBuy);
    }

    public void SetData(int idx, WorkerStat stat, Action<int, WorkerStat, bool> lvUpCallback)
    {
        this.idx = idx;
        this.stat = stat;
        this.lvUpCallback = lvUpCallback;


        if (stat == null)
        {
            for (int i = 0; i < 3; ++i)
            {
                statCells[i].SetMaxLv(0);
            }
        }
        else
        {
            int maxLv = stat.GetMaxLv();
            for (int i = 0; i < 3; ++i)
            {
                statCells[i].SetMaxLv(maxLv);
            }
        }

        Redraw();
    }

    public void Redraw()
    {
        DrawStats();
        DrawBtns();
    }

    private void DrawStats()
    {
        if (idx == 0)
        {
            dim.SetActive(false);
        }
        else
        {
            if (Game.Stage.employees[idx - 1] == null)
            {
                dim.SetActive(true);
            }
            else if (Game.Stage.employees[idx - 1].Stat.IsMaxLv())
            {
                dim.SetActive(false);
            }
            else
            {
                dim.SetActive(true);
            }
        }

        if (stat == null) return;

        statCells[0].SetLv(stat.speedLv.Value);
        statCells[1].SetLv(stat.capacityLv.Value);
        statCells[2].SetLv(stat.amountLv.Value);
    }

    private void DrawBtns()
    {
        buyBtn.gameObject.SetActive(false);
        max.SetActive(false);
        upgradeBtn.gameObject.SetActive(false);
        adBtn.gameObject.SetActive(false);

        if (stat == null || stat.lv == 0)
        {
            buyBtn.gameObject.SetActive(true);
        }
        else if (stat.lv >= stat.GetMaxLv())
        {
            max.SetActive(true);
        }
        else
        {
            upgradeBtn.gameObject.SetActive(true);
            upgradeText.text = Config.GetEmployeeUpgradePrice(stat.lv).ToString();
            adBtn.gameObject.SetActive(true);
        }
    }

    private void OnClickLvUp()
    {
        lvUpCallback?.Invoke(idx, stat, false);
    }

    private void OnClickAdsLvUp()
    {
        lvUpCallback?.Invoke(idx, stat, true);
    }

    private void OnClickBuy()
    {
        lvUpCallback?.Invoke(idx, stat, false);
    }
}

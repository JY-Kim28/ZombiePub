using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class CellPlayerUpgrade : MonoBehaviour
{
    [SerializeField] protected Color activeColor;
    [SerializeField] protected Color deactiveColor;

    [SerializeField] protected Image[] points;

    [SerializeField] protected Button upgradeBtn;
    [SerializeField] protected TextMeshProUGUI upgradeText;
    [SerializeField] protected Button adsBtn;

    private void Start()
    {
        upgradeBtn.onClick.AddListener(OnClickLevelUp);
        adsBtn.onClick.AddListener(OnClickAds);

        SetStat();
    }

    protected abstract void SetStat();

    protected virtual void OnClickLevelUp()
    {
        if (Root.UserInfo.MinusMoney(GetLevelUpPrice()))
        {
            LvUp();
        }
        else
        {

        }
    }

    protected virtual void OnClickAds()
    {
        LvUp();
    }

    protected abstract uint GetLevelUpPrice();

    protected abstract void LvUp();


    protected virtual void OnDrawStatLv(ushort lv)
    {
        if (lv == 5)
        {
            adsBtn.gameObject.SetActive(false);
            upgradeBtn.gameObject.SetActive(false);
        }
        else
        {
            adsBtn.gameObject.SetActive(true);
            upgradeBtn.gameObject.SetActive(true);

            upgradeText.text = GetLevelUpPrice().ToString();
        }

        int size = points.Length;
        for (int i = 0; i < size; ++i)
        {
            points[i].color = lv > i ? activeColor : deactiveColor;
        }
    }

}

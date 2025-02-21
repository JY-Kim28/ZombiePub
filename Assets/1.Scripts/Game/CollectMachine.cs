using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CollectMachine : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image fill;
    [SerializeField] UnitTriggerArea area;

    LevelUpData lvUpData;
    Action<LevelUpData> onCompleteCallback;

    public uint chargeCount { get; private set; } = 0;

    private void Start()
    {
        area.SetTimeLimit(0.01f);
        area.enterCallback = GiveMe;
    }

    public void SetTableData(LevelUpData data, Action<LevelUpData> onCompleteCallback)
    {
        gameObject.SetActive(false);

        lvUpData = data;

        chargeCount = 0;

        DrawCount();

        this.onCompleteCallback = onCompleteCallback;

        transform.localScale = Vector3.one;

        gameObject.SetActive(true);
    }

    public void SetChargeCount(uint count)
    {
        chargeCount = count;

        DrawCount();
    }

    private void GiveMe(Unit obj)
    {
        if (chargeCount == lvUpData.needPrice) return;

        Player player = obj as Player;
        if (player)
        {
            if (Root.UserInfo.MinusMoney(lvUpData.needPrice - chargeCount))
            {
                DOTween.To(()=> chargeCount, DrawCount, lvUpData.needPrice, 0.5f).onComplete = OnCompleteCountdown;
            }
            else
            {
                uint money = Root.UserInfo.GetMoney();
                if (Root.UserInfo.MinusMoney(money))
                {
                    DOTween.To(() => chargeCount, DrawCount, chargeCount + money, 0.5f);
                }
            }
        }
    }


    private void OnCompleteHideAnimation()
    {
        gameObject.SetActive(false);

        onCompleteCallback?.Invoke(lvUpData);
    }

    private void DrawCount()
    {
        text.text = (lvUpData.needPrice - chargeCount).ToString();
        fill.fillAmount = (chargeCount / (float)lvUpData.needPrice);
    }

    private void DrawCount(uint num)
    {
        chargeCount = num;

        //text.text = num.ToString();
        //fill.fillAmount = num / (float)lvUpData.needPrice;

        DrawCount();
    }

    private void OnCompleteCountdown()
    {
        transform.DOScale(0.5f, 0.5f).SetEase(Ease.InBack, 3).onComplete = OnCompleteHideAnimation;
    }
}

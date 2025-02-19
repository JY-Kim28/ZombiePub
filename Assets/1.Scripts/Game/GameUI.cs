using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

public class GameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;

    [SerializeField] GameObject mission;
    [SerializeField] Text missionText;

    private void Awake()
    {
        HideMissionText();
    }

    public void Initialize()
    {
        Root.UserInfo.OnValueChangeMoney += DrawMoney;

        DrawMoney(Root.UserInfo.GetMoney());
    }

    private void DrawMoney(uint value)
    {
        moneyText.text = value.ToString();
    }

    public void ShowMissionText()
    {
        mission.SetActive(true);
    }

    public void ShowMissionText(string code)
    {
        missionText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Guide", code);
        ShowMissionText();
    }

    public void HideMissionText()
    {
        mission.SetActive(false);
    }
}

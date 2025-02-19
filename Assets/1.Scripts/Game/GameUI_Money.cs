using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI_Money : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;

    public void Start()
    {
        Root.UserInfo.OnValueChangeMoney += DrawMoney;

        DrawMoney(Root.UserInfo.GetMoney());
    }

    private void DrawMoney(uint value)
    {
        moneyText.text = value.ToString();
    }
}

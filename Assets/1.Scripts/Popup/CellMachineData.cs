using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using System.Text;

public class CellMachineData : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI[] buffTexts;
    [SerializeField] Button selectBtn;

    Action<ushort> onSelectCallback;

    ushort idx;
    StringBuilder sb = new StringBuilder();

    private void Start()
    {
        selectBtn.onClick.AddListener(OnClickSelectBtn);
    }

    public void SetData(ushort idx, List<BuffData> buffDatas, Action<ushort> callback)
    {
        this.idx = idx;
        onSelectCallback = callback;


        nameText.text = "Design " + idx;


        int size = buffTexts.Length;
        for(int i = 0; i < size; ++i)
        {
            if (buffDatas.Count <= i)
                buffTexts[i].text = "-";
            else
            {
                sb.Clear();
                sb.Append(LocalizationSettings.StringDatabase.GetLocalizedString("UI", $"BUFF_TYPE_{(ushort)buffDatas[i].buffType}"));
                sb.Append(" +");
                sb.Append(buffDatas[i].buffValue);

                buffTexts[i].text = sb.ToString();
            }
        }

    }

    private void OnClickSelectBtn()
    {
        onSelectCallback?.Invoke(idx);
    }
}

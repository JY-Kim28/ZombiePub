using System.Collections.Generic;
using UnityEngine;
using System;
using DopaminTable;
using TMPro;

public class ObjectBase : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI decoLvText;

    public CollectMachine collectMachine;

    public GameObject[] decorations;
    public ushort decorationIdx;

    public OBJ_TYPE objType { protected set; get; }
    public ushort currLv = 0;

    protected List<BuffData> buffList = new List<BuffData>();

    public Worker worker;
    public Worker readyWorker;

    public uint saveCollectCount;

    public virtual void Initialize()
    {
        gameObject.SetActive(false);

        if(collectMachine != null)
            collectMachine.gameObject.SetActive(false);

        foreach (var d in decorations)
        {
            d.gameObject.SetActive(false);
        }
    }

    public void ShowCollectMachine(LevelUpData data, Action<LevelUpData> onCompleteCallback)
    {
        if (collectMachine != null)
        {
            collectMachine.SetTableData(data, onCompleteCallback);
            collectMachine.SetChargeCount(saveCollectCount);
        }
        gameObject.SetActive(true);
    }

    public void LevelUp()
    {
        SetData((ushort)(currLv + 1), 1);

        if(currLv > 1)
        {
            PopupManager.Instance.ShowPopup<PopupMachineLevelUp>(open =>
            {
                open.Show(objType, currLv, SetDecoration);
            });
        }
    }

    public virtual void SetData(ushort lv, ushort decoIdx)
    {
        currLv = lv;

        if(currLv != 0)
        {
            gameObject.SetActive(true);
        }

        SetDecoration(decoIdx);
    }

    public virtual void SetDecoration(ushort decoIdx)
    { 
        decorationIdx = decoIdx;

        buffList.Clear();

        if(currLv != 0)
        {
            if (decorations.Length != 0)
            {
                if (decoIdx > decorations.Length) decoIdx = (ushort)decorations.Length;

                decorations[decoIdx - 1].gameObject.SetActive(true);
            }

            SetBuff();
        }

        ShowDecoLv(currLv);
    }

    protected void ShowDecoLv(ushort lv)
    {
        if (decoLvText == null) return;

        if (currLv != 0)
        {
            decoLvText.transform.parent.gameObject.SetActive(true);
            decoLvText.text = $"Lv.{currLv}";
        }
        else
        {
            decoLvText.transform.parent.gameObject.SetActive(false);
        }
    }

    protected virtual void SetBuff()
    {
        Table_MachineLevel200Data data = Tables.MachineLevel200.GetData(Table_MachineLevel200.CreateCode((ushort)objType, currLv));
        if (data.id == 0)
            return;

        if (decorationIdx != 3)
        {
            int size = Math.Min(data.buffList.Count, data.buffValue.Count);
            for (int i = 0; i < size; ++i)
            {
                BuffData buffData;
                buffData.buffType = (BUFF_TYPE)data.buffList[i];
                buffData.buffValue = data.buffValue[i];
                buffList.Add(buffData);
            }
        }
        else
        {
            int size = Math.Min(data.specialBuffList.Count, data.specialBuffValue.Count);
            for (int i = 0; i < size; ++i)
            {
                BuffData buffData;
                buffData.buffType = (BUFF_TYPE)data.specialBuffList[i];
                buffData.buffValue = data.specialBuffValue[i];
                buffList.Add(buffData);
            }
        }
    }

    protected BuffData FindBuff(BUFF_TYPE type)
    {
        return buffList.Find(x => x.buffType == type);
    }



    public virtual bool IsNeedWorker()
    {
        return false;
    }


    public virtual string GetSaveData()
    {
        saveCollectCount = collectMachine != null ? collectMachine.chargeCount : uint.MaxValue;
        return $"{currLv},{decorationIdx},{saveCollectCount}";
    }

    public virtual void SetSaveData(string save)
    {
        string[] data = save.Split(',');

        currLv = ushort.Parse(data[0]);
        decorationIdx = ushort.Parse(data[1]);

        SetData(currLv, decorationIdx);

        saveCollectCount = uint.Parse(data[2]);
    }
}

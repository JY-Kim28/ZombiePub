using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DopaminTable;

[UIPopup("PopupMachineLevelUp")]
public class PopupMachineLevelUp : UIPopupBase
{
    [SerializeField] CellMachineData[] cells;

    Action<ushort> selectDecoCallback;

    public void Show(OBJ_TYPE objType, ushort lv, Action<ushort> selectCallback)
    {
        selectDecoCallback = selectCallback;

        Table_MachineLevel200Data data;

        data = Tables.MachineLevel200.GetData(Table_MachineLevel200.CreateCode((ushort)objType, lv));

        List<BuffData> buffs = new List<BuffData>();
        int size = data.buffList.Count < data.buffValue.Count ? data.buffList.Count : data.buffValue.Count;
        for(int i = 0; i < size; ++i)
        {
            BuffData buffData;
            buffData.buffType = (BUFF_TYPE)data.buffList[i];
            buffData.buffValue = data.buffValue[i];
            buffs.Add(buffData);
        }

        List<BuffData> sbuffs = new List<BuffData>();
        size = data.specialBuffList.Count < data.specialBuffValue.Count ? data.specialBuffList.Count : data.specialBuffValue.Count;
        for (int i = 0; i < size; ++i)
        {
            BuffData buffData;
            buffData.buffType = (BUFF_TYPE)data.specialBuffList[i];
            buffData.buffValue = data.specialBuffValue[i];
            sbuffs.Add(buffData);
        }

        cells[0].SetData(1, buffs, SelectCell);
        cells[1].SetData(2, buffs, SelectCell);
        cells[2].SetData(3, sbuffs, SelectCell);
    }

    private void SelectCell(ushort idx)
    {
        selectDecoCallback?.Invoke(idx);

        Hide();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo
{
    SaveDataManager data;

    public void Initialize()
    {
        Load();

        data.saveData.money.OnValueCallback += OnChangeMoney;
    }


    #region System
    public void Load()
    {
        if (data == null)
        {
            data = new SaveDataManager();
            data.Load();
        }
        else
        {
            data.Load();
        }
    }

    public void Save()
    {
        data.Save();
    }

    public byte[] GetSaveDataToByte()
    {
        return data.GetSaveDataToByte();
    }

    public void Delete()
    {
        data.Delete();
    }
    #endregion System



    public bool MinusProduct(PRODUCT_TYPE type, uint value)
    {
        if(type == PRODUCT_TYPE.Money)
        {
            return MinusMoney(value);
        }

        return false;
    }


    #region Money
    public uint GetMoney()
    {
        return data.saveData.money.Value;
    }

    public Action<uint> OnValueChangeMoney;
    public void OnChangeMoney(uint value)
    {
        OnValueChangeMoney?.Invoke(value);
    }

    public bool MinusMoney(uint value)
    {
        if (data.saveData.money.Value != 0)
        {
            if(data.saveData.money.Value >= value)
            {
                data.saveData.money.Value -= value;
                return true;
            }
        }
        return false;
    }

    public void AddMoney(uint value)
    {
        if (data.saveData.money.Value != uint.MaxValue)
        {
            if (uint.MaxValue - data.saveData.money.Value < value)
            {
                data.saveData.money.Value = uint.MaxValue;
            }
            else
            {
                data.saveData.money.Value += value;
            }
        }
    }
    #endregion Money

}

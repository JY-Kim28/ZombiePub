using System;
using UnityEngine;

public enum OBJ_TYPE
{
    Collector,
    Manufacture,
    Processing,
    Selling,
    Using,
    Decoration,
    ProductPile,
    ManageRoom,
    Office_HR,
    Office_GM,

    TrashBin,
};

public enum PRODUCT_TYPE
{
    None,
    Money,
    Product,
};

public enum SHOW_LOCK_TYPE
{
    Level,
    OpenPrevItemNeedAreaIdx
}

public enum BUFF_TYPE
{
    None,
    IncreaseCount,
    SpeedUp,
    PriceUp
}

public struct BuffData
{
    public BUFF_TYPE buffType;
    public float buffValue;
}


public struct ProductChangeData
{
    public PRODUCT_TYPE insertType;
    public ushort needInsertCount;

    public PRODUCT_TYPE releaseType;
    public ushort releaseCount;
}

public struct LevelUpData
{
    public ushort step;
    public ushort objIdx;
    public ushort objLv;
    public uint needPrice;
    public ushort needLv;

    public void SetData(string data)
    {
        string[] datas = data.Split(',');
        step = ushort.Parse(datas[0]);
        objIdx = ushort.Parse(datas[1]);
        objLv = ushort.Parse(datas[2]);
        needPrice = uint.Parse(datas[3]);
        needLv = ushort.Parse(datas[4]);
    }
}

public enum LEVEUP_SHOW_TYPE
{
    Index,
    Level
}

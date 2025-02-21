using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DG.Tweening;
using UnityEngine;

public static class Config
{
    public static Vector3 GetMoneyPos(int idx)
    {
        Vector3 pos = new Vector3(
                x: 0.4f * ((idx % 4) - 2),
                y: (idx / 12) * 0.12f,
                z: -0.8f * (((idx / 4) % 2) - 1)
                );

        return pos;
    }

    public static void GiveMoneyToPlayer(Player player, Stack<Product> moneys)
    {
        if (player == null) return;
        if (moneys.Count == 0) return;

        int count = moneys.Count;

        float delay = 0.5f / count;

        for (int i = 0; i < count; ++i)
        {
            Product product = moneys.Pop();
            product.transform.SetParent(player.transform.parent);
            product.transform.DOLocalJump(player.transform.localPosition + Vector3.up, 3, 1, 0.01f).SetDelay(delay * i).onComplete = () =>
            {
                Root.Resources.ReleaseProduct(product);
            };

            if (product.Data.Type == PRODUCT_TYPE.Money)
            {
                Root.UserInfo.AddMoney(product.value);
            }
        }
    }




    public static void SaveTxt(string fileName, string value)
    {
        if (Directory.Exists(Application.persistentDataPath) == false)
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }

        byte[] data = Encoding.UTF8.GetBytes(value);

        FileStream fs = new FileStream($"{Application.persistentDataPath}/{fileName}.txt", FileMode.Create);
        fs.Write(data, 0, data.Length);
        fs.Close();
    }

    public static string LoadTxt(string fileName)
    {
        if (File.Exists($"{Application.persistentDataPath}/{fileName}.txt") == false)
        {
            return "";
        }

        FileStream filestream = new FileStream($"{Application.persistentDataPath}/{fileName}.txt", FileMode.Open, FileAccess.Read);
        StreamReader sr = new StreamReader(filestream, System.Text.Encoding.UTF8);
        string re = sr.ReadToEnd();
        sr.Close();
        filestream.Close();

        return re;
    }



    public static uint GetEmployeeUpgradePrice(ushort lv)
    {
        return (uint)(40 + (10 * (lv / 3)));
    }
}

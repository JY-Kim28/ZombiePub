using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;

[Serializable]
public class SaveData
{
    public SaveDataProperty<uint> money = new SaveDataProperty<uint>();
}

[Serializable]
public class SaveDataProperty<T>
{
    T _Value = default;
    public T Value
    {
        get { return _Value; }
        set { _Value = value; OnValueCallback?.Invoke(_Value); }

    }

    [NonSerialized]
    public Action<T> OnValueCallback;
}


public class SaveDataManager
{
    private static string _FilePath = Application.persistentDataPath + "/SaveData.json";
    
    public SaveData saveData { get; private set; }


    public void SetData(byte[] data)
    {
        string json = Encoding.UTF8.GetString(data);
        saveData = JsonConvert.DeserializeObject<SaveData>(json);
        
        Save();
    }

    public void Load()
    {
        if (File.Exists(_FilePath) == false)
        {
            saveData ??= new SaveData();

            Save();

            return;
        }

        FileStream filestream = new FileStream(_FilePath, FileMode.Open, FileAccess.Read);
        StreamReader sr = new StreamReader(filestream, System.Text.Encoding.UTF8);
        string json = sr.ReadToEnd();
        sr.Close();
        filestream.Close();

        saveData = JsonConvert.DeserializeObject<SaveData>(json);
    }

    public void Delete()
    {
        if (Directory.Exists(Application.persistentDataPath))
        {
            string json = JsonConvert.SerializeObject(saveData = new SaveData());

            FileStream fs = new FileStream(_FilePath, FileMode.Create);
            byte[] data = Encoding.UTF8.GetBytes(json);
            fs.Write(data, 0, data.Length);
            fs.Close();
        }
    }

    public void Save()
    {
        if(Directory.Exists(Application.persistentDataPath) == false)
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }

        byte[] data = GetSaveDataToByte();

        FileStream fs = new FileStream(_FilePath, FileMode.Create);
        fs.Write(data, 0, data.Length);
        fs.Close();
    }

    public byte[] GetSaveDataToByte()
    {
        string json = JsonConvert.SerializeObject(saveData ??= new SaveData());
        byte[] data = Encoding.UTF8.GetBytes(json);
        return data;
    }


    public void Save<T>(ref SaveDataProperty<T> property, T value)
    {
        property.Value = value;
        Save();
    }

    public void Save<T>(ref T type, T value)
    {
        type = value;
        Save();
    }

    public void Save<T>(ref T[] type, int idx, T value)
    {
        type[idx] = value;
        Save();
    }

    public void Save<T>(ref List<T> type, int idx, T value)
    {
        type[idx] = value;
        Save();
    }
}

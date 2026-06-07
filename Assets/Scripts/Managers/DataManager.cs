using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Text;

public class DataManager : SingleTons<DataManager>
{
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        LoadData();
    }
    public void SaveData()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/SaveData"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/SaveData");
        }
        if(!File.Exists(Application.persistentDataPath + "/SaveData/MapData.txt"))
        {
            File.Create(Application.persistentDataPath + "/SaveData/MapData.txt").Dispose();
        }
        using (FileStream PlayerDataFile = File.Open(Application.persistentDataPath + "/SaveData/MapData.txt", FileMode.Open))
        {
            string json = JsonUtility.ToJson(MapManager.Instance.mapData, true);
            PlayerDataFile.Write(Encoding.UTF8.GetBytes(json), 0, json.Length);
            PlayerDataFile.Close();
        }
    }
    private void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData/MapData.txt"))
        {
            using (FileStream PlayerDataFile = new FileStream(Application.persistentDataPath + "/SaveData/MapData.txt", FileMode.Open))
            {
                byte[] bytes = new byte[PlayerDataFile.Length];
                PlayerDataFile.Read(bytes, 0, (int)PlayerDataFile.Length);
                string json = Encoding.UTF8.GetString(bytes);
                JsonUtility.FromJsonOverwrite(json, MapManager.Instance.mapData);
                PlayerDataFile.Close();
            }
        }
    }
}

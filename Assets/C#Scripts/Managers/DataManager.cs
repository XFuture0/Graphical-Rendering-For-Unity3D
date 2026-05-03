using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataManager : SingleTons<DataManager>
{
    private BinaryFormatter formatter;
    protected override void Awake()
    {
        base.Awake();
        formatter = new BinaryFormatter();
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
            var Json = JsonUtility.ToJson(MapManager.Instance.mapData);
            formatter.Serialize(PlayerDataFile, Json);
            PlayerDataFile.Close();
        }
    }
    private void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData/MapData.txt"))
        {
            using (FileStream PlayerDataFile = new FileStream(Application.persistentDataPath + "/SaveData/MapData.txt", FileMode.Open))
            {
                JsonUtility.FromJsonOverwrite(formatter.Deserialize(PlayerDataFile).ToString(), MapManager.Instance.mapData);
                PlayerDataFile.Close();
            }
        }
    }
}

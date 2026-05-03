using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartGameCanvs : MonoBehaviour
{
    public Button InitGameButton_Sel;
    public Button StartGameButton_Sel;
    public Button BackButton_Sel;
    public Button InitGameButton_Init;
    public Button BackButton_Init;
    public TMP_InputField SeedInput;
    public GameObject MapSlots;
    public GameObject MapSlotPrefab;
    private void Awake()
    {
        InitGameButton_Sel.onClick.AddListener(OnInitGame_Sel);
        StartGameButton_Sel.onClick.AddListener(OnStartGame_Sel);
        BackButton_Sel.onClick.AddListener(OnBack_Sel);
        InitGameButton_Init.onClick.AddListener(OnInitGame_Init);
        BackButton_Init.onClick.AddListener(OnBack_Init);
    }
    private void OnEnable()
    {
        InitMapList();
    }
    private void InitMapList()
    {
        
        foreach(Transform child in MapSlots.transform)
        {
            Destroy(child.gameObject);
        }
        foreach(MapSlot mapSlot in MapManager.Instance.mapData.MapSlots)
        {
            GameObject mapSlotObj = Instantiate(MapSlotPrefab, MapSlots.transform);
            mapSlotObj.transform.GetChild(0).GetComponent<TMP_Text>().text = mapSlot.Seed.ToString();
        }
    }
    private void OnStartGame_Sel()
    {
        bool success = MapManager.Instance.InitMap();
        if(success)
        {
            UIManager.Instance.StartGameCanvs.SetActive(false);
        }
    }
    private void OnInitGame_Sel()
    {
        transform.GetChild(1).gameObject.SetActive(true);
    }
    private void OnBack_Sel()
    {
        UIManager.Instance.StartGameCanvs.SetActive(false);
        UIManager.Instance.MainMenuCanvs.SetActive(true);
    }
    private void OnInitGame_Init()
    {
        int Seed;
        if(SeedInput.text == "")
        {
            Seed = int.Parse(((TextMeshProUGUI)SeedInput.placeholder).text);
        }
        else
        {
            Seed = int.Parse(SeedInput.text);
        }
        MapManager.Instance.mapData.MapSlots.Add(new MapSlot(Seed));
        InitMapList();
        DataManager.Instance.SaveData();
    }
    private void OnBack_Init()
    {
        transform.GetChild(1).gameObject.SetActive(false);
    }
}

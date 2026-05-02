using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartGameCanvs : MonoBehaviour
{
    public Button InitGameButton;
    public Button BackButton;
    public TMP_InputField SeedInput;
    private void Awake()
    {
        InitGameButton.onClick.AddListener(OnInitGame);
        BackButton.onClick.AddListener(OnBack);
    }
    private void OnInitGame()
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
        MapManager.Instance.InitMap(Seed);
        UIManager.Instance.StartGameCanvs.SetActive(false);
    }
    private void OnBack()
    {
        UIManager.Instance.StartGameCanvs.SetActive(false);
        UIManager.Instance.MainMenuCanvs.SetActive(true);
    }
}

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
        Debug.Log("InitGame");
    }
    private void OnBack()
    {
        UIManager.Instance.StartGameCanvs.SetActive(false);
        UIManager.Instance.MainMenuCanvs.SetActive(true);
    }
}

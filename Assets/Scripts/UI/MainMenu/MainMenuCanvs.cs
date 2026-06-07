using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvs : MonoBehaviour
{
    public Button StartGameButton;
    public Button NetGameButton;
    public Button SettingButton;
    public Button QuitGameButton;
    public GameObject StartGameCanvs;
    private void Awake()
    {
        StartGameButton.onClick.AddListener(OnStartGame);
        NetGameButton.onClick.AddListener(OnNetGame);
        SettingButton.onClick.AddListener(OnSetting);
        QuitGameButton.onClick.AddListener(OnQuitGame);
    }
    private void OnStartGame()
    {
        StartGameCanvs.SetActive(true);
        gameObject.SetActive(false);
    }
    private void OnNetGame()
    {
        Debug.Log("NetGame");
    }
    private void OnSetting()
    {
        Debug.Log("Setting");
    }
    private void OnQuitGame()
    {
        Application.Quit();
    }
}

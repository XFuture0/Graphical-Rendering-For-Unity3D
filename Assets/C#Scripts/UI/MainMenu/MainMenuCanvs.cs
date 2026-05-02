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
    private void Awake()
    {
        StartGameButton.onClick.AddListener(OnStartGame);
        NetGameButton.onClick.AddListener(OnNetGame);
        SettingButton.onClick.AddListener(OnSetting);
        QuitGameButton.onClick.AddListener(OnQuitGame);
    }
    private void OnStartGame()
    {
        UIManager.Instance.StartGameCanvs.SetActive(true);
        UIManager.Instance.MainMenuCanvs.SetActive(false);
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

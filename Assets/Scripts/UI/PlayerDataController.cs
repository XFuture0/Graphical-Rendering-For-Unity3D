using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataController : MonoBehaviour
{
    public PlayerData playerData;
    private PlayerCanvas view;
    private void Awake()
    {
        view = GetComponent<PlayerCanvas>();
    }
    void Update()
    {
        NotifyViews();
    }
    private void NotifyViews()
    {
        view.UpdateHealth(playerData.playerHealth);
        view.UpdateFood(playerData.playerFood);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingleTons<UIManager>
{
    public GameObject PlayerBag;
    public GameObject PlayerCanvs;
    public GameObject BagCanvs;
    public GameObject SelectSlot;
    public GameObject MainMenuCanvs;
    public GameObject StartGameCanvs;
    public GameObject Player;
    public void InitGame(int Height)
    {
        Player.transform.position = new Vector3(0, Height, 0);
        Player.SetActive(true);
        PlayerCanvs.SetActive(true);
        BagCanvs.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}

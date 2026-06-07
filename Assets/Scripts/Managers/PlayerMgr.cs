using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMgr : MonoBehaviour
{
    public GameObject Player;
    private void Start()
    {
        EventMgr.Instance.AddEventListener<int>(EventType.InitMap, InitMap);
    }
    private void InitMap(int Height)
    {
        Player.transform.position = new Vector3(0, Height, 0);
        Player.SetActive(true);
    }
}

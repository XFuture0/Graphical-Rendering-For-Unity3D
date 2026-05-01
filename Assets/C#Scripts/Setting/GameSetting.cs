using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;//固定帧率为60
        Cursor.lockState = CursorLockMode.Locked;//锁定鼠标
        Cursor.visible = false;//隐藏鼠标
    }
}

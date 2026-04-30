using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 50;//固定帧率为50
        QualitySettings.vSyncCount = 0;//帧率会被锁定为显示器刷新率的整数倍,0为关闭垂直同步
        Cursor.lockState = CursorLockMode.Locked;//锁定鼠标
        Cursor.visible = false;//隐藏鼠标
    }
}

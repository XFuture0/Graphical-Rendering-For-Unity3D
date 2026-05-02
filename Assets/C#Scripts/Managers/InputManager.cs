using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingleTons<InputManager>
{
    public float GetKeyDown_Horizontal()
    {
        return Input.GetAxisRaw("Horizontal");
    }
    public float GetKeyDown_Vertical()
    {
        return Input.GetAxisRaw("Vertical");
    }
    public bool GetKey_Space()
    {
        return Input.GetKey(KeyCode.Space);
    }
    public float GetKey_MouseX()
    {
        return Input.GetAxis("Mouse X");
    }
    public float GetKey_MouseY()
    {
        return Input.GetAxis("Mouse Y");
    }
    public bool GetKeyDown_MouseLeft()
    {
        return Input.GetMouseButtonDown(0);
    }
    public bool GetKeyDown_MouseRight()
    {
        return Input.GetMouseButtonDown(1);
    }
    public bool GetKeyDown_E()
    {
        return Input.GetKeyDown(KeyCode.E);
    }
    public int GetKeyDown_Number()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) return 1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) return 2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) return 3;
        if (Input.GetKeyDown(KeyCode.Alpha4)) return 4;
        if (Input.GetKeyDown(KeyCode.Alpha5)) return 5;
        if (Input.GetKeyDown(KeyCode.Alpha6)) return 6;
        if (Input.GetKeyDown(KeyCode.Alpha7)) return 7;
        if (Input.GetKeyDown(KeyCode.Alpha8)) return 8;
        if (Input.GetKeyDown(KeyCode.Alpha9)) return 9;
        return 0;
    }
}

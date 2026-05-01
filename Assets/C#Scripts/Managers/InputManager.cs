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
}

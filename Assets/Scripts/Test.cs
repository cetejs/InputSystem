using UD.Globals;
using UD.Services.InputSystem;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void OnGUI()
    {
        var input = Global.GetService<InputManager>();
        GUILayout.TextArea($"Horizontal : {input.GetAxis("Horizontal")}");
        GUILayout.TextArea($"Vertical : {input.GetAxis("Vertical")}");
        GUILayout.TextArea($"Mouse X : {input.GetAxis("Mouse X")}");
        GUILayout.TextArea($"Mouse Y : {input.GetAxis("Mouse Y")}");
        GUILayout.TextArea($"HorizontalArrow : {input.GetAxis("HorizontalArrow")}");
        GUILayout.TextArea($"VerticalArrow : {input.GetAxis("VerticalArrow")}");
        GUILayout.TextArea($"Button0 : {input.GetButton("Button0")}");
        GUILayout.TextArea($"Button1 : {input.GetButton("Button1")}");
        GUILayout.TextArea($"Button2 : {input.GetButton("Button2")}");
        GUILayout.TextArea($"Button3 : {input.GetButton("Button3")}");
        GUILayout.TextArea($"Button4 : {input.GetButton("Button4")}");
        GUILayout.TextArea($"Button5 : {input.GetButton("Button5")}");
        GUILayout.TextArea($"Button6 : {input.GetButton("Button6")}");
        GUILayout.TextArea($"Button7 : {input.GetButton("Button7")}");
        GUILayout.TextArea($"Button8 : {input.GetButton("Button8")}");
        GUILayout.TextArea($"Button9 : {input.GetButton("Button9")}");
        GUILayout.TextArea($"Button10 : {input.GetButton("Button10")}");
        GUILayout.TextArea($"LeftStickClick : {input.GetButton("LeftStickClick")}");
        GUILayout.TextArea($"RightStickClick : {input.GetButton("RightStickClick")}");

        if (input.GetButtonDown("Button8"))
        {
            Debug.Log("Button8 Down");
        }

        if (input.GetButtonUp("Button8"))
        {
            Debug.Log("Button8 Up");
        }
    }
}

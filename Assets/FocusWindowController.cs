using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Class to controll the window for showing selected device data
public class FocusWindowController : MonoBehaviour
{
    public Text Focus; 

    public void updateText(Device_Status input)
    {
        Focus.text = input.toString();
    }
}

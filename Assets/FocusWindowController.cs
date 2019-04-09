using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Class to controll the window for showing selected device data
public class FocusWindowController : MonoBehaviour
{
    public Text Focus;
    public MarkerManager marker_manager;
    public Device_Status currently_selected;

    void Update()
    {
        currently_selected = marker_manager.current_target_status;

        if (currently_selected == null)
        {
            clearText();
        }
        else
        {
            updateText(currently_selected);

        }
    }
    public void updateText(Device_Status input)
    {
        Focus.text = input.toString();
    }
    public void clearText()
    {
        Focus.text = "No device currently selected";
    }
}

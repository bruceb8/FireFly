using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelControls : MonoBehaviour
{
    public Vector2 droneMenuAway = new Vector2(1, (float) 0.5) ;
    public Vector2 droneMenuOnScreen = new Vector2(0, (float) 0.5);

    public Vector2 deviceMenuAway = new Vector2(1, (float)0.5);
    public Vector2 deviceMenuOnScreen = new Vector2(-.5f, (float)0.5);


    public GameObject DevicePanel;
    public GameObject DronePanel;

    bool droneMenuIsActive = false;
    bool deviceMenuIsActive =true;
    
    public void ToggleDroneMenu() {
        if(droneMenuIsActive == true){
            DronePanel.GetComponent<RectTransform>().pivot = droneMenuAway;
            droneMenuIsActive = false;
        }else{
            DronePanel.GetComponent<RectTransform>().pivot = droneMenuOnScreen;
            droneMenuIsActive = true;
        }
    }

    public void ToggleDeviceMenu()
    {
        if (deviceMenuIsActive == false)
        {
            DevicePanel.GetComponent<RectTransform>().pivot = deviceMenuAway;
            deviceMenuIsActive = true;
        }
        else
        {
            DevicePanel.GetComponent<RectTransform>().pivot = deviceMenuOnScreen;
            deviceMenuIsActive = false;
        }
    }
}

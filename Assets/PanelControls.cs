using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PanelControls : MonoBehaviour
{
    public Vector2 droneMenuAway = new Vector2(1, (float)0.5);
    public Vector2 droneMenuOnScreen = new Vector2(0, (float)0.5);

    public Vector2 deviceMenuAway = new Vector2(1, (float)0.5);
    public Vector2 deviceMenuOnScreen = new Vector2(-.5f, (float)0.5);


    public GameObject DevicePanel;
    public GameObject DronePanel;
    public Button DroneMenuButton;


    bool droneMenuIsActive = false;
    bool deviceMenuIsActive = true;

    void Start()
    {
        if (PlayerPrefs.GetInt("UserIsChief") == 0)
        {
            DroneMenuButton.enabled = false;
            DroneMenuButton.transform.position = new Vector3(100, 100, 0);
        }

    }
    public void ToggleDroneMenu()
    {
        if (droneMenuIsActive == true)
        {
            DronePanel.GetComponent<RectTransform>().pivot = droneMenuAway;
            droneMenuIsActive = false;
        }
        else
        {
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

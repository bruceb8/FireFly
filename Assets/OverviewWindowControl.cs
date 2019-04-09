using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverviewWindowControl : MonoBehaviour
{

    public Text OverviewField;
    public WSNetworkManager WS;
    public MarkerManager MS;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateWindowText", 1, 1);
    }

    public void UpdateWindowText()
    {
        string output = "";
        output += "> Runtime: " + TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + "\n";
        if (WS.firefighters != null)
        {
            output += "> Number of Firefighters: " + WS.firefighters.Count + "\n";
        }
        if (WS.beacons != null)
        {
            output += "> Number of Beacons: " + WS.beacons.Count + "\n";
        }
        if(WS.drones != null)
        {
            output += "> Number of Drones: " + WS.drones.Count + "\n";

        }
        output += "> Maximum Temperature from devices: " + MS.GetMaxTemp() + "\n";
        output += "> Maximum CO from devices: " + MS.GetMaxCO() + "\n";
        OverviewField.text = output;
    }
}

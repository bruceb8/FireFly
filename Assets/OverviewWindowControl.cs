using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverviewWindowControl : MonoBehaviour
{

    public Text OverviewField;
    public WSNetworkManager WS;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateWindowText", 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
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

        OverviewField.text = output;
    }
}

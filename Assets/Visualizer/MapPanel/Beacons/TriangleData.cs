using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleData : MonoBehaviour{

    public float lat;
    public float lng;
    public float alt;
    public int id;
    public float tmp;

    public GameObject RestManager;
    public GameObject FocusWindow;
    public GameObject Spawner;
    public GameObject Self;

    public bool currentTarget = false;

    private void OnMouseDown()
    {
        var header = FocusWindow.GetComponent<FocusController>().header;
        header.Clear();
        header.Add("Type: Beacon");
        header.Add("ID: " + id);
        header.Add("Lat/Lon: " + lat + ", " + lng);
        header.Add("Temp: " + tmp + "°F");
        FocusWindow.GetComponent<FocusController>().updateFocus();
        RestManager.GetComponent<RESTNetworkManager>().GetREST("/beacon" + id.ToString());
    }

    public void UpdateFocus()
    {
        if (currentTarget)
        {
            var header = FocusWindow.GetComponent<FocusController>().header;
            header.Clear();
            header.Add("Type: Beacon");
            header.Add("ID: " + id);
            header.Add("Lat/Lon: " + lat + ", " + lng);
            header.Add("Temp: " + tmp + "°F");
            FocusWindow.GetComponent<FocusController>().updateFocus();
        }
    }
}

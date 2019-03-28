using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class UIFunctions : MonoBehaviour
{

    public GameObject map;
    private OnlineMaps m;
    private OnlineMapsMarkerManager m_manager;
    public WSNetworkManager WSManager;
    // Start is called before the first frame update
    void Start()
    {
        m = map.GetComponent<OnlineMaps>();
        m_manager = map.GetComponent<OnlineMapsMarkerManager>();
    }

    // Update is called once per frame
    void Update() { }

    public void BackToMenu()
    {
        SceneManager.LoadScene("menu", LoadSceneMode.Single);
    }
    public void CenterMapOnDevices()
    {
        if (m_manager.Count > 0) m.SetPosition(getAvgLon(), getAvgLat());

    }
    private double getAvgLon()
    {
        double output = 0;
        foreach (OnlineMapsMarker marker in m_manager.items)
        {
            output += marker.position.x;
        }
        return output / m_manager.Count;
    }
    private double getAvgLat()
    {
        double output = 0;
        foreach (OnlineMapsMarker marker in m_manager.items)
        {
            output += marker.position.y;
        }
        return output / m_manager.Count;
    }
    public void ToggleBeacons()
    {
        foreach (OnlineMapsMarker marker in m_manager.items)
        {
            string markerID = marker.label;
            StringComparison something = StringComparison.InvariantCulture;
            if (markerID.StartsWith("BN:", something))
            {
                marker.enabled = !marker.enabled;
            }
        }
    }

    public void ToggleDrones()
    {
        foreach (OnlineMapsMarker marker in m_manager.items)
        {
            string markerID = marker.label;
            StringComparison something = StringComparison.InvariantCulture;
            if (markerID.StartsWith("DN:", something))
            {
                marker.enabled = !marker.enabled;
            }
        }
    }

    public void ToggleFireFighters()
    {
        foreach (OnlineMapsMarker marker in m_manager.items)
        {
            string markerID = marker.label;
            StringComparison something = StringComparison.InvariantCulture;
            if (markerID.StartsWith("FF:", something))
            {
                marker.enabled = !marker.enabled;
            }
        }
    }

    void CenterOnDevice(string id)
    {
        foreach (OnlineMapsMarker marker in m_manager.items)
        {
            string[] markerID = marker.label.Split(':');
            if (id == markerID[1])
            {
                m.SetPosition(marker.position.x, marker.position.y);
                return;
            }

        }
    }
}

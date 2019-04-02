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
    public OnlineMapsDrawingLine line_drawer;
    public MarkerManager local_m_manager;

    public bool all_paths_shown;
    public bool path_is_drawn;

    // Start is called before the first frame update
    void Start()
    {
        m = map.GetComponent<OnlineMaps>();
        m_manager = map.GetComponent<OnlineMapsMarkerManager>();
        path_is_drawn = false;
        all_paths_shown = false;
    }

    // Update is called once per frame
    void Update() {

     }

    public void BackToMenu()
    {
        SceneManager.LoadScene("menu", LoadSceneMode.Single);
    }
    public void CenterMapOnDevices()
    {
        if (m_manager.Count > 0)
        {
            m.SetPosition(getAvgLon(), getAvgLat());
            m.zoom = CalculateNewMapZoom();
        }
    }
    private int CalculateNewMapZoom()
    {
        return 20;
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

    public void CenterOnDevice(string id)
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
    //will draw the current position log of currently selected device
    public void TogglePath()
    {
        if (!path_is_drawn)
        {
            if (local_m_manager.current_target_marker != null)
            {
                string[] current_id = local_m_manager.current_target_marker.label.Split(':');
              
                Device_Status marker = WSManager.GetDevice(current_id[1], current_id[0]);

                if (marker.position_log != null)
                {
                    line_drawer = new OnlineMapsDrawingLine(marker.position_log, local_m_manager.current_target_marker_color, 5);
                    OnlineMapsDrawingElementManager.AddItem(line_drawer);
                    path_is_drawn = true;
                }
            }
        }
        else
        {
            path_is_drawn = false;
            OnlineMapsDrawingElementManager.RemoveItem(line_drawer);
        }
    }
    public void ShowPath()
    {
        if(line_drawer != null) OnlineMapsDrawingElementManager.RemoveItem(line_drawer);
        if (!path_is_drawn)
        {
            if (local_m_manager.current_target_marker != null)
            {
                string[] current_id = local_m_manager.current_target_marker.label.Split(':');

                Device_Status marker = WSManager.GetDevice(current_id[1], current_id[0]);

                if (marker.position_log != null)
                {
                    line_drawer = new OnlineMapsDrawingLine(marker.position_log, local_m_manager.current_target_marker_color, 5);
                    OnlineMapsDrawingElementManager.AddItem(line_drawer);
                 
                }
            }
        }

    }

    public void ShowAllPaths()
    {
        all_paths_shown = !all_paths_shown;
        if (all_paths_shown)
        {
            foreach (Device_Status FF in WSManager.firefighters)
            {
                OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(FF.position_log, FF.marker_color, 5));
            }
            foreach (Device_Status BN in WSManager.beacons)
            {
                OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(BN.position_log, BN.marker_color, 5));

            }
            foreach (Device_Status DN in WSManager.drones)
            {
                OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(DN.position_log, DN.marker_color, 5));

            }
        }
        else
        {
            OnlineMapsDrawingElementManager.RemoveAllItems();
        }
    }
}

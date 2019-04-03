using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

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
    void Update(){}

    public void BackToMenu()
    {
        SceneManager.LoadScene("menu", LoadSceneMode.Single);
    }
    public void CenterMapOnDevices()
    {
        if( m_manager.Count > 0) m.SetPosition(getAvgLon(), getAvgLat());
     
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
}

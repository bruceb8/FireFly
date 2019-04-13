using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using WebSocketSharp;


[System.Serializable]
public struct WebMessage
{
    public string type;
    public PositionsRecieved body;
}

[System.Serializable]
public struct PositionsRecieved
{
    public List<Device_Positions> ff;
    public List<Device_Positions> dn;

}

[System.Serializable]
public struct Device_Positions
{
    public string id;
    public List<float> lat;
    public List<float> lon;
}

public class ToSend
{
    public string id;
    public string type;

    public ToSend(string v1, string v2)
    {
        this.id = v1;
        this.type = v2;
    }
}

public class RestNetworkManager : MonoBehaviour
{
    public TerminalWindowControl terminal;

    public WSNetworkManager websocket;

    public WebMessage message;
    public PositionsRecieved initial_pos;
    public Device_Positions ffs;

    public bool got_positions = false;
    class Conditions
    {
        float high_temp;
        int high_temp_id;
        float high_co;
        int high_co_id;
        int nr_onscene;
        int nr_beacons;

        public override string ToString()
        {
            return "Tempurature High: " + high_temp
                + "\nTempurature High ID: " + high_temp_id
                + "\nCO High: " + high_co
                + "\nCO High ID: " + high_co_id
                + "\nNumber of Beacons on scene: " + nr_beacons
                + "\nNumber of FireFighters on scene: " + nr_onscene;
        }
    }
    string address;
    void Start(){}
    public void GetREST(string path)
    {
        address = "http://" + WSNetworkManager.ip + ":8080" + path;
        terminal.TerminalColorPrint("REST API: Requesting " + address, Color.yellow);
        switch (path)
        {
            case "/positions":
                StartCoroutine(GetRequestPositions(address));

                break;
            default:
                break;

        }
      
    }


    IEnumerator GetRequestPositions(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            got_positions = true;
            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                //initial_pos = JsonUtility.FromJson<PositionsRecieved>(webRequest.downloadHandler.text);

                message = JsonUtility.FromJson<WebMessage>(webRequest.downloadHandler.text);
                List<Device_Positions> drone_s = message.body.dn;
                List<Device_Positions> fire_fighters = message.body.ff;
          
                fire_fighters.ForEach(HandleAction1);
                drone_s.ForEach(HandleAction);

                websocket.startWebsocketConnection();
                //Conditions temp = JsonUtility.FromJson<Conditions>(webRequest.downloadHandler.text);
                //terminal.TerminalColorPrint("Current Server Recorded Conditions:\n" + temp.ToString(), Color.yellow);
            }
        }
    }
    void HandleAction1(Device_Positions ff)
    {
        Debug.Log(ff);
        List<Vector2> temp = new List<Vector2>();
      
        for (int i = 0; i < ff.lat.Count; i++)
        {
            temp.Add(new Vector2(ff.lon[i], ff.lat[i]));
        }
        Device_Status status = new Device_Status(ff.id, "FF", temp);
        websocket.firefighters.Add(status);
    }


    void HandleAction(Device_Positions dn)
    {
        Debug.Log(dn);
        List<Vector2> temp = new List<Vector2>();
     
        for (int i = 0; i < dn.lat.Count; i++)
        {
            temp.Add(new Vector2(dn.lon[i], dn.lat[i]));
        }
        Device_Status status = new Device_Status(dn.id, "DN", temp);
        websocket.drones.Add(status);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RestNetworkManager : MonoBehaviour
{
    public TerminalWindowControl terminal;
    public WSNetworkManager ws;
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
    void Start()
    {
    }
    public void GetREST(string path)
    {
        address = "http://" + WSNetworkManager.ip + ":8080" + path;
        terminal.TerminalColorPrint("REST API: Requesting " + address, Color.yellow);
        StartCoroutine(GetRequest(address));
      
    }
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                Conditions temp = JsonUtility.FromJson<Conditions>(webRequest.downloadHandler.text);
                terminal.TerminalColorPrint("Current Server Recorded Conditions:\n" + temp.ToString(), Color.yellow);
            }
        }
    }
}

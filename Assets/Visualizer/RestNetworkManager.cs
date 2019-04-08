using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RestNetworkManager : MonoBehaviour
{
    public TerminalWindowControl terminal;
    public WSNetworkManager ws;

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
            }
        }
    }
}

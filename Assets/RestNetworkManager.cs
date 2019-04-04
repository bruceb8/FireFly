using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


//This is a class to use to get the REST server data. Call the Function GetREST with the appropriate path
// and the result should show up on TerminalWindow

public class RestNetwokManager : MonoBehaviour
{
    public TerminalWindowControl terminal;
    public WSNetworkManager ws;

    string address;

    public bool GetREST(string path)
    {
        //if (true)
        //{
            address = "http://" + WSNetworkManager.ip + ":" + WSNetworkManager.port + path;
            terminal.TerminalColorPrint("REST API: Requesting " + path, Color.yellow);
            UnityWebRequest request = new UnityWebRequest(address);

            return true;
        //}
       //terminal.TerminalColorPrint("Failed to connect to REST API", Color.red);
        //return false;
        
    }

}

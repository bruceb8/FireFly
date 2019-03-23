using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net;
using UnityEngine.UI;

//

public class RESTNetworkManager : MonoBehaviour {

    public GameObject FocusWindow;
    public GameObject terminal;

    public bool GetREST(string path)
    {
        if (WSNetworkManager.connected)
        {
            string address = "http://" + WSNetworkManager.ip + ":" + WSNetworkManager.port + path;
            terminal.GetComponent<TerminalController>().TerminalColorPrint("REST API: Requesting " + path, Color.yellow);
            //WWW request = new WWW("https://jsonplaceholder.typicode.com");
            WWW request = new WWW(address);
            StartCoroutine(WaitForResponse(request));
            return true;
        }
        else
        {
            terminal.GetComponent<TerminalController>().TerminalColorPrint("Failed to connect to REST API", Color.red);
            return false;
        }
    }

    private IEnumerator WaitForResponse(WWW request)
    {
        yield return request;
        if(request.error == null)
        {
            Debug.Log(request.text);
            FocusWindow.GetComponent<FocusController>().body = request.text;
            FocusWindow.GetComponent<FocusController>().updateFocus();
            terminal.GetComponent<TerminalController>().TerminalColorPrint("REST API: Request Successful", Color.green);

        }
        else
        {
            Debug.Log(request.error);
            terminal.GetComponent<TerminalController>().TerminalColorPrint("REST API Error: " + request.error, Color.red);
        }
    }
}

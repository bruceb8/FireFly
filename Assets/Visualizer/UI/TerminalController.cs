using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Terminal Window: Displays messages regarding system information in a familiar terminal format. Caps number of displayed messages at 100, and allows users to scroll using a scroll bar. Displayed messages include:
	•	Server Connection status (Connecting, Connected, Disconnected)
	•	Instantiation of new objects
	•	REST API requests
	•	Any message sent by server using the “terminal” WebMessage type (See Data Specification)
*/


public class TerminalController : MonoBehaviour {

    public Text display;
    private int nLines = 0;

    
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        while (WSNetworkManager.TMQueue.Count > 0)
        {
            string message = WSNetworkManager.TMQueue.Dequeue();
            TerminalColorPrint("Message from Server: " + message, Color.green);
        }
        while (nLines > 100)
        {
            int index = display.text.IndexOf(System.Environment.NewLine);
            display.text = display.text.Substring(index + System.Environment.NewLine.Length);
            nLines--;
        }
	}

    public void TerminalColorPrint(string message, Color color)
    {
        nLines += 1;
        string toPrint = "> ";
        toPrint += "<color=#";
        toPrint += ((int)(color.r*255)).ToString("X2") + ((int)(color.g * 255)).ToString("X2") + ((int)(color.b * 255)).ToString("X2");
        toPrint += ">";
        toPrint += message.Replace('\n', ' ');
        toPrint += "</color>";
        toPrint += "\n";
        display.text += toPrint;
    }

    public void TerminalPrint(string message)
    {
        nLines += 1;
        string toPrint = "> ";
        toPrint += message.Replace('\n', ' ') + "\n";
        display.text += toPrint;
    }
}

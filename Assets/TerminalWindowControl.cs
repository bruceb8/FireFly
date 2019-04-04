using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TerminalWindowControl : MonoBehaviour
{
    public Text TerminalWindowText;
    public WSNetworkManager WS;

    private int nlines = 0;

    // Start is called before the first frame update
    void Start()
    {
        while(WS.TMQueue.Count > 0)
        {
            string message = WS.TMQueue.Dequeue();
            TerminalColorPrint(message, Color.green);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TerminalColorPrint(string message, Color text_color)
    {
        nlines++;
        string toPrint = "> ";
        toPrint += "<color=#";
        toPrint += ((int)(text_color.r * 255)).ToString("X2") + ((int)(text_color.g * 255)).ToString("X2") + ((int)(text_color.b * 255)).ToString("X2");
        toPrint += ">";
        toPrint += message.Replace('\n', ' ');
        toPrint += "</color>";
        toPrint += "\n";
        TerminalWindowText.text += toPrint;
    }
    public void TerminalPrint(string message) {
        nlines++;
        string toPrint = "> ";
        toPrint += message.Replace('\n', ' ') + "\n";
        TerminalWindowText.text += toPrint;
    }

}

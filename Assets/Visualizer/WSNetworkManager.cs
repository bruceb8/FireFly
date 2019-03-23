using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

/*This script is automatically started when the vizualization scene is loaded, it does this 
 * by being in a game object. All game objects are automatically loaded when a scene is.
 * So by putting this in a game object, this script is getting called automatically.
 * 
 * This script grabs the settings saved in the player prefs and uses those to instantiate 
 * a new socket connection
 */

public class WSNetworkManager : MonoBehaviour {

    WebSocket ws;
    public GameObject terminal;
    string wsAddress;

    public static string ip = "localhost";
    public static string port = "8080";

    public static bool connected = false;

    // Instruction queues for each data type
    public static Queue<string> FFQueue = new Queue<string>();
    public static Queue<string> BNQueue = new Queue<string>();
    public static Queue<string> TMQueue = new Queue<string>();

    private string last = "";

    class WebMessage
    {
        public string type;
        public string body;
    }

	// Use this for initialization of the WebSocket
	void Start () {
        ip = PlayerPrefs.GetString("ServerIP");
        port = PlayerPrefs.GetString("ServerPort");
        wsAddress = "ws://" + ip + ":" + port;//this is the address for the javascript node server...
        terminal.GetComponent<TerminalController>().TerminalColorPrint("Connecting to " + wsAddress, Color.yellow);

        ws = new WebSocket(wsAddress);//create new websocket with address from player prefs
        ws.Connect();//Establish a websocket connection to the server.

		//Functions are defined below and are set as function pointers of the Websocket object
        ws.OnOpen += Ws_OnOpen;
        ws.OnMessage += Ws_OnMessage;
        ws.OnClose += Ws_OnClose;
        ws.OnError += Ws_OnError;
	}

    private void Ws_OnError(object sender, ErrorEventArgs e)
    {
        terminal.GetComponent<TerminalController>().TerminalColorPrint("WebSocket Error", Color.red);
        terminal.GetComponent<TerminalController>().TerminalColorPrint(e.Message, Color.red);
    }

    private void Ws_OnClose(object sender, CloseEventArgs e)
    {
        connected = false;
        terminal.GetComponent<TerminalController>().TerminalColorPrint("Connection to Server Closed", Color.green);
    }

    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
        connected = true;
        WebMessage message = JsonUtility.FromJson<WebMessage>(e.Data);
        last = e.Data;
//		Debug.Log (message.body);
        if(message.type == "ff" || message.type == "FF")
        {
            FFQueue.Enqueue(message.body);
        }

        if(message.type == "bn" || message.type == "BN")
        {
            BNQueue.Enqueue(message.body);

        }

        if(message.type == "terminal")
        {
            TMQueue.Enqueue(message.body);
        }
    }

    private void Ws_OnOpen(object sender, System.EventArgs e)
    {
        connected = true;
        terminal.GetComponent<TerminalController>().TerminalColorPrint("Connected Successfully", Color.green);
    }

    private void OnApplicationQuit()
    {
        connected = false;
        ws.Close();
    }

    // Update is called once per frame
    void Update () {
        if (UIFunctions.closeScene)
        {
            connected = false;
            ws.Close();
            UIFunctions.closeScene = false;
        }
	}
}

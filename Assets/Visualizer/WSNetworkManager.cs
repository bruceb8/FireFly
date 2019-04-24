using System;
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
public class Current_Status
{
    public string id;
    public float lat;
    public float lon;
    public float alt;
    public float temp;
    public string time;
    public float light;
    public float co;
}

public class WSNetworkManager : MonoBehaviour
{
    public WebSocket ws;
    public RestNetworkManager RestManager;
    public string wsAddress;

    public static string ip = "localhost";
    public static string port = "8080";
    public static bool connected = false;


    public List<Device_Status> firefighters = new List<Device_Status>();
    public List<Device_Status> beacons = new List<Device_Status>();
    public List<Device_Status> drones = new List<Device_Status>();

    public Queue<string> TMQueue = new Queue<string>();


    public TerminalWindowControl terminal;

    class WebMessage
    {
        public string type;
        public string body;
    }

    public void startWebsocketConnection()
    {
        ws = new WebSocket(wsAddress);//create new websocket with address from player prefs

        ws.OnOpen += Ws_OnOpen;
        ws.OnMessage += Ws_OnMessage;
        ws.OnClose += Ws_OnClose;
        ws.OnError += Ws_OnError;

        terminal.TerminalColorPrint("Connecting to " + wsAddress, Color.yellow);
        ws.Connect();//Establish a websocket connection to the server.
    }
    // Use this for initialization of the WebSocket
    void Start(){
        ip = PlayerPrefs.GetString("ServerIP");
        port = PlayerPrefs.GetString("ServerPort");
        wsAddress = "ws://" + ip + ":" + port;//this is the address for the javascript node server...

        RestManager = this.GetComponent<RestNetworkManager>();
        RestManager.GetREST("/positions");
    }

    private void Ws_OnOpen(object sender, MessageEventArgs e)
    {
        connected = true;
        terminal.TerminalColorPrint("Connected to " + wsAddress, Color.green);
    }
    private void Ws_OnError(object sender, ErrorEventArgs e){
        terminal.TerminalColorPrint(e.Message, Color.red);
    }

    private void Ws_OnClose(object sender, CloseEventArgs e)
    {
        connected = false;
    }

    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
       
        connected = true;

        WebMessage message = JsonUtility.FromJson<WebMessage>(e.Data);
        //Debug.Log(message.body);

        if (message.type == "terminal")
        {
            terminal.TerminalColorPrint(message.body, Color.green);
        }
        else
        {
            Current_Status incoming_device = JsonUtility.FromJson<Current_Status>(message.body);
            Device_Status temp_device = new Device_Status(incoming_device);

            int index = -1;
            if (message.type == "ff" || message.type == "FF")
            {
                index = findDeviceByID(temp_device, 1);
                if (index >= 0)
                {
                    firefighters[index].updateStatus(temp_device);
                  


                }
                else
                {
           
                    firefighters.Add(temp_device);
                    terminal.TerminalColorPrint("Added firefighter with id: " + temp_device.id, Color.cyan);

                }
            }
            else if (message.type == "bn" || message.type == "BN")
            {
                index = findDeviceByID(temp_device, 2);
                if (index >= 0)
                {
                    beacons[index].updateStatus(temp_device);
                }
                else
                {
                    temp_device.position_log.Add(new Vector2(temp_device.lon, temp_device.lat));

                    beacons.Add(temp_device);
                    terminal.TerminalColorPrint("Added beacon with id: " + temp_device.id, Color.cyan);
                }

            }
            else if (message.type == "dn" || message.type == "DN")
            {
                index = findDeviceByID(temp_device, 3);
                if (index >= 0)
                {
                    drones[index].updateStatus(temp_device);
                 

                }
                else
                {
                    temp_device.position_log.Add(new Vector2(temp_device.lon, temp_device.lat));

                    drones.Add(temp_device);
                    terminal.TerminalColorPrint("Added drone with id: " + temp_device.id, Color.cyan);
                }
            }
        }
    }

    private void Ws_OnOpen(object sender, System.EventArgs e)
    {
        connected = true;
    }

    private void OnApplicationQuit()
    {
        connected = false;
        ws.Close();
    }

    public Device_Status GetDevice(string input, string type_of_device)
    {
        int index = 0;

        switch (type_of_device)
        {
            case "FF":
                foreach (Device_Status ff in firefighters)
                {
                    if (input == ff.id)
                        return ff;
                    index++;
                }
                break;
            case "BN":
                foreach (Device_Status bn in beacons)
                {
                    if (input == bn.id)
                        return bn;
                    index++;
                }
                break;
            case "DN":
                foreach (Device_Status dn in drones)
                {
                    if (input == dn.id)
                        return dn;
                    index++;
                }
                break;
        }
        return null;
    }
    //return index if found else -1
    private int findDeviceByID(Device_Status input, int type_of_device)
    {
        int index = 0;

        switch (type_of_device)
        {
            case 1:
                foreach (Device_Status ff in firefighters)
                {
                    if (input == ff)
                        return index;
                    index++;
                }
                break;
            case 2:
                foreach (Device_Status bn in beacons)
                {
                    if (input == bn)
                        return index;
                    index++;
                }
                break;
            case 3:
                foreach (Device_Status dn in drones)
                {
                    if (input == dn)
                        return index;
                    index++;
                }
                break;
        }
        return -1;
    }
    public int findDeviceByID(string input, string type_of_device)
    {
        int index = 0;

        switch (type_of_device)
        {
            case "FF":
                foreach (Device_Status ff in firefighters)
                {
                    if (input == ff.id)
                        return index;
                    index++;
                }
                break;
            case "BN":
                foreach (Device_Status bn in beacons)
                {
                    if (input == bn.id)
                        return index;
                    index++;
                }
                break;
            case "DN":
                foreach (Device_Status dn in drones)
                {
                    if (input == dn.id)
                        return index;
                    index++;
                }
                break;
        }
        return -1;
    }
}

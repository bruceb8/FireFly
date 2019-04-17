/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

public class path{
    public double[] lat;
    public double[] lng;
    public double alt;
    public double cycles;

}



namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to create a marker on click.
    /// </summary>
    public class BoundingMarkers:MonoBehaviour
    {

            public GameObject map;

            public GameObject flyButton;

            private Button startButton;

            public WSNetworkManager wsManager;

            public GameObject droneDropDown;

            public DropDownController droneSelect;

            public TerminalWindowControl terminal;

            public GameObject inputAltitude;
            private OnlineMaps m;
            private OnlineMapsMarkerManager m_manager;

            public OnlineMapsMarker m0;
            public OnlineMapsMarker m1; 

            public List<OnlineMapsDrawingRect> heatMap;

            public List<OnlineMapsDrawingLine> heatDot;

            public OnlineMapsMarker mStart;

            public OnlineMapsDrawingLine pathLine;

            



            private int currentMarker = 0;

            private Boolean isPlaced = false;

            private bool placeStart = false;

            private bool pathRecieve = false;

            private int altitude = 30;

            private int cycles = 1;

            public int placeMarker = 0;
            private string url = "http://localhost:8080/GARBAGE"; 

            Texture2D stipple;

            Boolean isFlying = false; //Is the drone flying?

            
        void Start()
        {
            droneSelect = droneDropDown.GetComponent<DropDownController>();

            heatMap = new List<OnlineMapsDrawingRect>();

            heatDot = new List<OnlineMapsDrawingLine>();
            
            startButton = flyButton.GetComponent<Button>();

            // Subscribe to the click event.
            OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
            // Create a label for the marker.
            string label = "FP";

            // Create a new marker.
            m0 = OnlineMapsMarkerManager.CreateItem(0, 0, label);
            m1 = OnlineMapsMarkerManager.CreateItem(0, 0, label);
            mStart = OnlineMapsMarkerManager.CreateItem(0, 0, label);
            m0.enabled = false;
            m1.enabled = false;
            mStart.enabled = false;


            stipple = Resources.Load<Texture2D>("circle");

            // foreach(OnlineMapsMarker marker in m_manager.items){
            //     string markerID = marker.label;
            //     StringComparison something = StringComparison.InvariantCulture;
            //     if (markerID.StartsWith("FF:", something)){
            //         marker.enabled = false;
            //     }
            // }
        }


        /*
        We only need to place a set amount of beacons (probably 2 to create a box)
        clicking will move the markers and the marker that is being moved will just be
        a pointer that cycles from the end of this short list to the beginning.

        So heres what we need:
        a list to store the 2 markers (we might need more in the future)
        an index of the list that represents the current marker
        Seperate button to hide the click markers
        
        We might create the markers on start and just move them on click. Keep them hidden from the
        beginning. I can actually hide these if I have a clear texture. (NEVERMIND)

        There is a marker enable/disable just run that make the markers then disable them.
        Enable them and move them when we click on the map.
        Have a button that disables both of them
        Have a button that sends their two locations or four locations if we want a strict box shape.
         */
        private void OnMapClick()
        {
            // Get the coordinates under the cursor.
            double lng, lat;
            if(placeMarker > 0){
                OnlineMapsControlBase.instance.GetCoords(out lng, out lat);
                if(currentMarker == 0){
                    m0.SetPosition(lng, lat);
                    m0.enabled = true;
                    currentMarker+=1;
                }else if (currentMarker == 1){
                    m1.SetPosition(lng, lat);
                    m1.enabled = true;
                    currentMarker +=1;

                    if(mStart.enabled == true){
                        isPlaced = true;
                    }

                }

                currentMarker = currentMarker % 2;
                placeMarker = placeMarker - 1;
            }

            if(placeStart == true && placeMarker == 0){
                OnlineMapsControlBase.instance.GetCoords(out lng, out lat);

                mStart.enabled = true;
                mStart.SetPosition(lng,lat);

                placeStart = false;

                if(m1.enabled == true && m0.enabled == true){
                    isPlaced = true;
                }


            }
            

 
        }

        public void startMarkers(){
            clearMarkers();
            placeMarker = 2;
        }

        public void clearMarkers(){
            currentMarker = 0;
            if(pathLine != null){
                OnlineMapsDrawingElementManager.RemoveItem(pathLine);
            }
            // m0.SetPosition(0,0);
            // m1.SetPosition(0,0);
            // mStart.SetPosition(0,0);
            m0.enabled = false;
            m1.enabled = false;
            mStart.enabled = false;
            isPlaced = false;


        }

        public void updateAltitude(string theInput){
            altitude = int.Parse(theInput);
        }


        public void setStart(){
            // Get the coordinates under the cursor.
            placeStart = true;
            

        }

        public void updateCycles(string theInput){
            cycles = int.Parse(theInput);
        }

        private void flightPath(path thePath){
            //Now we need to iterate through the list of paired lat longs
            //And draw lines between each point in order.
            List<Vector2> pathCoord = new List<Vector2>();

            for(int i = 0; i < thePath.lat.Length; i++){
                
                //Debug.Log(thePath.lat[i]);
                //Debug.Log(thePath.lng[i]);

                pathCoord.Add(new Vector2((float) thePath.lat[i],(float) thePath.lng[i]));
                
            }

            //if path isnt null, clear it
            if(pathLine != null){
                OnlineMapsDrawingElementManager.RemoveItem(pathLine);
            }


            pathLine = new OnlineMapsDrawingLine(pathCoord, Color.red, 5);
            OnlineMapsDrawingElementManager.AddItem(pathLine);

            

        }


        /*
        Heatmap drawing will start once we tell the drone to take off with a button
        Then we will populate an array with pointers to rectangle drawings that we
        put on the map.  These drawings will have varied color based on what we get
        from the sensor. We can have one color and change the alpha value to be more
        or less transparent.  This can replicate a heat map effect kinda.

        The other option is to associate pixel values with a gps location on the
        map.  This is way more complicated becuse it involves putting an overlay
        on the map and having it scale with the 
         */

        //This function sc
        public Texture2D updateTexture(float theCO){
            float maxCO = 250;
            float minCO = 0;

            float diffCO = maxCO - minCO;

            int scaledData = Mathf.RoundToInt((theCO - minCO) * 512/diffCO);
            Color32[] pixels = stipple.GetPixels32();
            Color32[] temppixels = new Color32[pixels.Length];

            Color32 newColor;

            if(scaledData < 256){
                newColor = new Color32(
                255,        // R
                (byte) scaledData,        // G
                0,        // B
              200);

            }else{
                scaledData = scaledData - 255;
                newColor = new Color32(
                (byte)scaledData,        // R
                255,        // G
                0,        // B
              200);
                
            }

            //Assume red is max at the start

            //First we start pumping green up

            //When green is at 255, we need to start bringing red down.

            for (int i = 0; i < stipple.height * stipple.width; i++)
        {
            if (pixels[i].a != 0)
            {
                temppixels[i] = newColor;
            }
            else
            {
                temppixels[i] = pixels[i];
            }
        }



            Texture2D temp = new Texture2D(stipple.width, stipple.height);
            temp.SetPixels32(temppixels, 0);
            temp.Apply();

            return temp;

            

        }

        public Color32 updateColor(float theCO){
            float maxCO = 250;
            float minCO = 0;

            float diffCO = maxCO - minCO;

            int scaledData = Mathf.RoundToInt((theCO - minCO) * 512/diffCO);

            Color32 newColor;

            if(scaledData < 256){
                newColor = new Color32(
                255,        // R
                (byte) scaledData,        // G
                0,        // B
              200);

            }else{
                scaledData = scaledData - 255;
                newColor = new Color32(
                (byte)scaledData,        // R
                255,        // G
                0,        // B
              200);
                
            }
            return newColor;
        }


        public void SendGeofence()
        {
            url = "http://" + PlayerPrefs.GetString("ServerIP") + ":" + "8080/GARBAGE"; 

            var request = new UnityWebRequest(url, "POST");

            //Now we have to build the JSON string from the beacons
            string test = 
            "{\"lats\":\""+ mStart.position.x 
            +"\",\"lngs\":\"" + mStart.position.y + "\","
            +"\"lat0\":\"" + m0.position.x + "\","
            +"\"lng0\":\"" + m0.position.y + "\","
            +"\"lat1\":\"" + m0.position.x + "\","
            +"\"lng1\":\"" + m1.position.y + "\","
            +"\"lat2\":\"" + m1.position.x + "\","
            +"\"lng2\":\"" + m1.position.y + "\","
            +"\"lat3\":\"" + m1.position.x + "\","
            +"\"lng3\":\"" + m0.position.y + "\","
            +"\"alt\":\"" + altitude + "\","
            +"\"cycles\":\"" + cycles + "\"}";


            test = "{\"lat\":[\"" + mStart.position.x + "\",\"" 
            + m0.position.x + "\",\""
            + m0.position.x + "\",\""
            + m1.position.x + "\",\""
            + m1.position.x + "\"],"
            +"\"lng\":[\"" + mStart.position.y + "\",\"" 
            + m0.position.y + "\",\""
            + m1.position.y + "\",\""
            + m1.position.y + "\",\""
            + m0.position.y + "\"], "
            + "\"alt\":\"" + altitude + "\","
            + "\"cycles\":\"" + cycles + "\"}";

            byte[] bodyRaw = Encoding.UTF8.GetBytes(test);
            request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "text/plain");
            

            

            if(isPlaced == true){
                request.SendWebRequest();
                while(request.isDone != true){}
                Debug.Log("Status Code: " + request.responseCode);
                string response = request.downloadHandler.text;
                terminal.TerminalPrint(test);
                path mypath = JsonUtility.FromJson<path>(response);

                pathRecieve = true;
                //Debug.Log(mypath.lat[0]);

                flightPath(mypath);
            }else{
                Debug.Log("Y'all didnt place nothin");
                terminal.TerminalPrint("Make sure to place the Starting Waypoint" +
                "\n and the two bounding waypoints");
            }


        }

        //DONT MAKE SO MANY TEXTURES YOU DOPE
        float plon = 0;
        float plat = 0;

        private void stippleMap(){
            

            foreach( Device_Status d in wsManager.drones){
                //For each of the drones, we need to start dropping markers
                if(d.lat != plat || d.lon != plon){
                    float carbonMon = d.co;
                    //first we make a texture based on the reading
                    //Texture2D tempTex = updateTexture(carbonMon);
                    Color32 tempCol = updateColor(0);
                    //then we create a marker using the texture
                    List<Vector2> tempDot = new List<Vector2>();
                    tempDot.Add(new Vector2(d.lon,d.lat));
                    tempDot.Add(new Vector2(d.lon + (float) 0.000005,d.lat + (float) 0.000005));
                    OnlineMapsDrawingLine tempLine = new OnlineMapsDrawingLine(tempDot, tempCol, 10);
                    heatDot.Add(tempLine);

                    OnlineMapsDrawingRect tempRect = new OnlineMapsDrawingRect(d.lon, d.lat, 0.000025, 0.000025, Color.green,0 , Color.red);
                    heatMap.Add(tempRect);
                    Debug.Log("We just made a rectangle");


                    plat = d.lat;
                    plon = d.lon;


                   //OnlineMapsDrawingElementManager.AddItem(tempRect);
                    
                    OnlineMapsDrawingElementManager.AddItem(tempLine);


                    /*IMPORTANT SINCE WE ONLY PLAN ON USING ONE DRONE WE BREAK INSTANTLY '*/
                    /*INSTEAD OF ITERATING THROUGH AN ENTIRE LIST */
                    break; 

                }
                


                // heatMap.Add(OnlineMapsMarkerManager.CreateItem(
                //     new Vector2(d.lat, d.lon),
                //     tempTex,
                //     "hmp"
                // ));

                

                

            }

        }

        public void landDrone(){
            url = "http://" + PlayerPrefs.GetString("ServerIP") + ":" + "8080/appLand"; 
            var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes("{\"drone\":\"id placeholder\"}");
            request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "text/plain");
            

            

            
                request.SendWebRequest();
                while(request.isDone != true){}
                isFlying = false;

                // foreach(OnlineMapsDrawingRect r in heatMap){
                //     OnlineMapsDrawingElementManager.AddItem(r);
                // }

            
        }


        public void startFlight(){
            url = "http://" + PlayerPrefs.GetString("ServerIP") + ":" + "8080/appStartFlight"; 
            var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes("{\"drone\":\"id placeholder\"}");
            request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "text/plain");
            

            

            if(pathRecieve == true){
                request.SendWebRequest();
                while(request.isDone != true){}
                isFlying = true;
            } else{
                terminal.TerminalPrint("Path was not recieved by the server.");
            }
        }
          
        void Update(){
 
            if(isFlying == true){
                stippleMap();
            }

            if(pathRecieve == false){
                startButton.interactable = false;
            }else{
                startButton.interactable = true;
            }
        }
    }

    

    


}

/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEngine;
using System;
using System.Text;
using System.Collections;
using UnityEngine.Networking;


namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to create a marker on click.
    /// </summary>
    public class BoundingMarkers:MonoBehaviour
    {

            public GameObject map;
            private OnlineMaps m;
            private OnlineMapsMarkerManager m_manager;

            public OnlineMapsMarker m0;
            public OnlineMapsMarker m1; 



            private int currentMarker = 0;

            private Boolean isPlaced = false;

            public int placeMarker = 0;
            private string url = "http://localhost:8080/GARBAGE"; 
        private void Start()
        {
            // Subscribe to the click event.
            OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
            // Create a label for the marker.
            string label = "FP";

            // Create a new marker.
            m0 = OnlineMapsMarkerManager.CreateItem(0, 0, label);
            m1 = OnlineMapsMarkerManager.CreateItem(0, 0, label);
            m0.enabled = false;
            m1.enabled = false;

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

                    isPlaced = true;
                }

                currentMarker = currentMarker % 2;
                placeMarker = placeMarker - 1;
            }
            

 
        }

        public void startMarkers(){
            clearMarkers();
            placeMarker = 2;
        }

        public void clearMarkers(){
            currentMarker = 0;
            m0.SetPosition(0,0);
            m1.SetPosition(0,0);
            m0.enabled = false;
            m1.enabled = false;
            isPlaced = false;


        }



        

        public void SendGeofence()
        {
            var request = new UnityWebRequest(url, "POST");

            //Now we have to build the JSON string from the beacons
            string test = "{\"lat\":\""+ m0.position.x 
            +"\",\"long\":\"" + m0.position.y + "\"}"
            ;
            byte[] bodyRaw = Encoding.UTF8.GetBytes(test);
            request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "test/plain");

            if(isPlaced == true){
                request.SendWebRequest();

                Debug.Log("Status Code: " + request.responseCode);
            }else{
                Debug.Log("Y'all didnt place nothin");
            }



        }
    }


}

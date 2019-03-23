using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



//This controls the Focus window in the Visualizer scene on Unity
/*
 * Focus Window: Displays existing model data upon selection of an object to focus on. Only one object can be the focus subject at a time. Displayed data includes:
	•	Type: Object type
	•	ID: Object ID
	•	Lat/Lon: Position of object
	•	Temp: Temperature of object
	•	CO: CO reading of object
	•	Humidity: Humidity reading of object
	•	Light: Light reading of object
	•	REST data: Selection of a object to focus on invokes a REST API request in the format 
		“/[object type][ID]”, eg. “/firefighter3”. This request is sent to the server.
		It asynchronously waits for a reply, and on return appends the additional data to the focus window 
		following the data above.
 * */
public class FocusController : MonoBehaviour {

    public Text Focus;

    public List<string> header;
    public string body = "";

	public void updateFocus()
    {
        Focus.text = "";
        foreach(string message in header)
        {
            Focus.text += "> " + message + "\n";
        }
        Focus.text += body;
    }

    
}

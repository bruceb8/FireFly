using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using InfinityCode;

//Higher level class for controlling the buttons on the Visualization Scene

public class UIFunctions : MonoBehaviour {

    public GameObject slider;
    public GameObject LoadScreenPrefab;
    public GameObject canvas;
    public GameObject map;
	public GameObject ff;
    public static bool closeScene = false;
	public static bool displayPaths = false;
    // Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {
        slider.GetComponent<UnityEngine.UI.Slider>().value += Input.GetAxis("Mouse ScrollWheel");
//		if(displayPaths) 
	}

    public void BackToMenu()
    {
        closeScene = true;
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
	public void displayFFPaths(){
		
		var ffs = ff.GetComponent<FFSpawner>();
		foreach (GameObject ff in ffs.FFList) {
			var ffpos = ff.GetComponent<CircleData>();
			if (!displayPaths)
				ffpos.renderPath ();
			else
				ffpos.removePath ();
			
		}
		displayPaths = !displayPaths;
	}

    public void toSattelite()
    {
        OnlineMaps.instance.mapType = "google.satellite";
    }

    public void toDark()
    {
        OnlineMaps.instance.mapType = "mapbox classic";
    }

    public void toLight()
    {
        OnlineMaps.instance.mapType = "google.relief";
    }

}

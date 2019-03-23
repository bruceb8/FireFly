using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleData : MonoBehaviour {
    public float lat;
    public float lng;
    public float alt;
    public float tmp;
    public int id;
    public string time;
    public float light;
    public float co;
    public float humidity;
    public bool currentTarget = false;

	//Log the positions of firefighters
	public Vector3[] positions = new Vector3[512];

    public GameObject RestManager;
    public GameObject FocusWindow;
    public GameObject Spawner;
    public GameObject Self;
    public GameObject TargetPrefab;

    private GameObject thisTarget;
	private LineRenderer lr;
	public int pathsize = 0;
	public bool render =false;
    private void OnMouseDown()
    {
        if (currentTarget) { UpdateFocus(); };
        RestManager.GetComponent<RESTNetworkManager>().GetREST("/person");
        Spawner.GetComponent<FFSpawner>().RequestTarget();
        currentTarget = true;
        thisTarget = Instantiate(TargetPrefab);
        thisTarget.transform.parent = Self.transform;
        thisTarget.GetComponent<Transform>().localScale = new Vector3((float).3, (float).3, (float).3);
        thisTarget.GetComponent<SpriteRenderer>().color = Self.GetComponent<SpriteRenderer>().color;
		renderPath();
        thisTarget.transform.localPosition = new Vector3(0, 0, 0);
    }

	public void renderPath(){
		Debug.Log ("Render on");
		lr = this.GetComponent<LineRenderer> ();
//		lr.positionCount = this.positions.Length;
		lr.positionCount = pathsize;

		lr.SetPositions(positions);
		lr.enabled = true;
		render = true;
	}

	public void removePath(){
		Debug.Log ("Render off");
		render = false;
		this.GetComponent<LineRenderer> ().enabled = false;

	}

    public void RemoveTarget(){     
        if (currentTarget)
        {
            Destroy(thisTarget);
            currentTarget = false;
			removePath();
        }
    }

    public void UpdateFocus(){
        var header = FocusWindow.GetComponent<FocusController>().header;
        header.Clear();
        header.Add("Type: Firefighter");
        header.Add("ID: " + id);
        header.Add("Lat/Lon: " + lat + ", " + lng);
        header.Add("Temp: " + tmp + "°F");
        header.Add("CO: " + co + " ppm");
        header.Add("Humidity: " + humidity + "%");
        header.Add("Light: " + light);
        FocusWindow.GetComponent<FocusController>().updateFocus();
    }
}

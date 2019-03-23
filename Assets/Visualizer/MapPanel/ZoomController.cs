using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomController : MonoBehaviour {

    public GameObject Camera;
    public GameObject map;
    public GameObject ffcontroller;
    public GameObject bncontroller;

    public static float ZoomScale = 1;

    private int minZoom = 10;
    private int maxZoom = 20;
	
    // Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {}

    public void UpdateZoom(float val)
    {
		//update the map??
        float level = (float)(1.0 / Mathf.Pow(2 , (int)val - 15));
        ZoomScale = (float)(1.0 / Mathf.Pow(2, val - 15));
        //Debug.Log(level);

		//Change scales!
        Camera.GetComponent<Camera>().orthographicSize = (float)1024 / (Mathf.Pow(2, val - 15));
        map.GetComponent<Transform>().localScale = new Vector3(level, level, 10);
        map.GetComponent<Transform>().localPosition = new Vector3((float)-20.48 * level, (float)-20.48 * level, 10);
        map.GetComponent<OnlineMaps>().zoom = (int)val;

		//update the firefighter and beacon locations per new scale
        ffcontroller.GetComponent<FFSpawner>().UpdateScale();
        bncontroller.GetComponent<BNSpawner>().UpdateScale();
    }
}

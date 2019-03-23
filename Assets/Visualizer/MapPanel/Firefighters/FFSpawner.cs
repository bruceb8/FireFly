using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class FFSpawner : MonoBehaviour {


    public GameObject FFPrefab;
    public GameObject Spawner;
    public GameObject Panel;
    public GameObject terminal;
    public GameObject NetworkManager;
    public GameObject FocusWindow;
    public GameObject TargetPrefab;

    public List<GameObject> FFList = new List<GameObject>();

    public float latCenter;
    public float lngCenter;
    public float latRange;
    public float lngRange;
	public float resolutionSize = .001f;
    private float DotScale;

	private List<Vector3> positions = new List<Vector3>();

    class FF
    {
        public float lat;
        public float lon;
        public float alt;
        public float temp;
        public int id;
        public string time;
        public float light;
        public float co;
        public float humidity;
    }
	const int MAX_DIFFERENCE = 20;

	// Use this for initialization
	void Start () {
        DotScale = PlayerPrefs.GetFloat("DotScale");
	}

	// Update is called once per frame
	void Update () {
        int index = -1;
        while (WSNetworkManager.FFQueue.Count > 0)
        {
            string instruction = WSNetworkManager.FFQueue.Dequeue();
            if (instruction.Contains("Destroy"))
            {
                Destroy(FFList[index]);//Delete FF object
                FFList.RemoveAt(index);
            }
            else
            {
                FF n = JsonUtility.FromJson<FF>(instruction);
                index = FFList.FindIndex(FF => FF.GetComponent<CircleData>().id == n.id);

                if (index >= 0)
                {
                    // Object already in list
                    var ffpos = FFList[index].GetComponent<CircleData>();
                    var m = Panel.GetComponent<LocationController>();
					var map = m.GetComponent<OnlineMaps> ();

					//If there is bad data sent from server hopefully this will catch it
					if(ffpos.lng - n.lon < MAX_DIFFERENCE)
                    	ffpos.lng = n.lon;
					if(ffpos.lat - n.lat < MAX_DIFFERENCE)
                    	ffpos.lat = n.lat;
					
                    ffpos.alt = n.alt;
                    ffpos.tmp = n.temp;
                    ffpos.time = n.time;
                    ffpos.humidity = n.humidity;
                    ffpos.light = n.light;
                    ffpos.co = n.co;
                    if (ffpos.currentTarget) { ffpos.UpdateFocus(); };

					//added 1/26/18 by Alex
					double xpos, ypos;
					if (map != null) {
						xpos = (ffpos.lng - m.lngMinMap) / (m.lngMaxMap - m.lngMinMap) * (2 * map.position.x) + map.position.x;
						ypos = (ffpos.lat - m.latMinMap) / (m.latMaxMap - m.latMinMap) * (2 * map.position.y) + map.position.y;
					} else {
						xpos = (ffpos.lng - m.lngMinMap) / (m.lngMaxMap - m.lngMinMap) * 40.96 - 20.48;
						ypos = (ffpos.lat - m.latMinMap) / (m.latMaxMap - m.latMinMap) * 40.96 - 20.48;
					}
					//==============================

					//Pathing implementation

					FFList[index].transform.localPosition = new Vector3((float)xpos, (float)ypos, 1);
					ffpos.positions [ffpos.pathsize] = new Vector3((float)xpos, (float)ypos, 1);
					if (ffpos.pathsize < 528) {
						ffpos.pathsize++;
					} else {
						Array.Clear (ffpos.positions, 0, ffpos.pathsize);
						ffpos.pathsize = 0;
						Vector3[] temp = new Vector3[ffpos.GetComponent<LineRenderer> ().positionCount];
						ffpos.GetComponent<LineRenderer> ().GetPositions(temp);
						for (int i = 0; i<temp.Length ;i ++ ) {
							ffpos.positions[i] = temp[i];
						}
					}
					ffpos.GetComponent<LineRenderer> ().Simplify (.001f);
//					ffpos.pathsize = ffpos.GetComponent<LineRenderer> ().positionCount;

                }
                else
                {
                    GameObject nobj = (GameObject)Instantiate(FFPrefab);

					Color nColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                    nobj.GetComponent<SpriteRenderer>().color = nColor;

				
					LineRenderer ffLine = nobj.GetComponent<LineRenderer> ();
//					ffLine.material = new Material(Shader.Find("Sprites/Default"));
					ffLine.material.color = nColor;
					ffLine.widthMultiplier = 0.2f;
					ffLine.positionCount = 0;
				
					ffLine.enabled = false;

                    var data = nobj.GetComponent<CircleData>();
                    data.lat = n.lat;
                    data.lng = n.lon;
                    data.alt = n.alt;
                    data.id = n.id;
                    data.tmp = n.temp;
                    //data.time = n.time;
					data.time = "TimeStamp";
                    //data.humidity = n.humidity;
					data.humidity = 1;
                    data.light = n.light;
                    data.co = n.co;

                    data.RestManager = NetworkManager;
                    data.Spawner = Spawner;
                    data.Self = nobj;
                    data.FocusWindow = FocusWindow;
                    data.TargetPrefab = TargetPrefab;

                    nobj.transform.parent = Spawner.transform;
                    nobj.GetComponent<Transform>().localScale = new Vector3(DotScale * ZoomController.ZoomScale, DotScale * ZoomController.ZoomScale, 1);
                    FFList.Add(nobj);
                    Panel.GetComponent<LocationController>().UpdateMap();
                    terminal.GetComponent<TerminalController>().TerminalColorPrint("New FF object with ID: " + data.id, Color.cyan);
                }
            }
        }

        if (UIFunctions.closeScene)
        {
            foreach(GameObject ff in FFList)
            {
                Destroy(ff);
            }
            FFList.Clear();
        }
	}

	public float getDistance(Vector3 a, Vector3 b){
		return (float) Math.Sqrt (Mathf.Pow ((a.x - b.x),2) + Mathf.Pow ((a.y - b.y),2));

	}
    public Boolean UpdateCenter()
    {
        float latMin;
        float lngMin;
        float latMax;
        float lngMax;

        if (FFList.Count > 0)
        {
            latMin = latMax = FFList[0].GetComponent<CircleData>().lat;
            lngMin = lngMax = FFList[0].GetComponent<CircleData>().lng;

            foreach (GameObject ff in FFList)
            {
                var data = ff.GetComponent<CircleData>();
                if (data.lat < latMin) latMin = data.lat;
                if (data.lng < lngMin) lngMin = data.lng;
                if (data.lat > latMax) latMax = data.lat;
                if (data.lng > lngMax) lngMax = data.lng;
            }


            float latRangen = latMax - latMin;
            float lngRangen = lngMax - lngMin;
            float latCentern = latMax - latRangen / 2;
            float lngCentern = lngMax - lngRangen / 2;

            if (Math.Abs(latCentern - latCenter) > latRange / 4 || Math.Abs(lngCentern - lngCenter) > lngRange / 4)
            {
                latRange = latRangen;
                lngRange = lngRangen;
                latCenter = latCentern;
                lngCenter = lngCentern;
                return true;
            }
        }
        return false;
    }
    public void UpdateScale()
    {
        foreach(GameObject ff in FFList)
        {
            ff.GetComponent<Transform>().localScale = new Vector3(DotScale * ZoomController.ZoomScale, DotScale * ZoomController.ZoomScale, 1);
        }
    }

    public void RequestTarget()
    {
        foreach(GameObject ff in FFList)
        {
            if (ff.GetComponent<CircleData>().currentTarget)
            {
                Debug.Log("Removing");
                ff.GetComponent<CircleData>().RemoveTarget();
                break;
            }
        }
    }

    public float maxTemp()
    {
        float max = -1;
        foreach (GameObject ff in FFList)
        {
            max = Math.Max(max, ff.GetComponent<CircleData>().tmp);
        }
        return max;
    }

    public float maxCO()
    {
        float max = -1;
        foreach(GameObject ff in FFList)
        {
            max = Math.Max(max, ff.GetComponent<CircleData>().co);
        }
        return max;
    }

}

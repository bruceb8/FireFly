using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class BNSpawner : MonoBehaviour{
    public GameObject BNPrefab;
    public GameObject Spawner;
    public GameObject Panel;
    public GameObject terminal;
    public GameObject NetworkManager;
    public GameObject FocusWindow;

    public List<GameObject> BNList = new List<GameObject>();

    private float TriScale;

//	//Stolen from the FFSpawner.cs=======
	public float latCenter;
	public float lngCenter;
	public float latRange;
	public float lngRange;
//	//===================================

    class BN
    {
		public int id;
		public float lat;
		public float lon;
		public float alt;
		public float temp;
    }
	const int MAX_DIFFERENCE = 20;

    // Use this for initialization
    void Start(){
        TriScale = PlayerPrefs.GetFloat("TriScale");
    }

    // Update is called once per frame
    void Update(){
        int index = -1;
        while (WSNetworkManager.BNQueue.Count > 0)
		{
            string instruction = WSNetworkManager.BNQueue.Dequeue();
            if (instruction.Contains("Destroy"))
            {
				Destroy(BNList[index]);//Delete FF object
				BNList.RemoveAt(index);
            }

            else
            {
                BN n = JsonUtility.FromJson<BN>(instruction);
                index = BNList.FindIndex(BN => BN.GetComponent<TriangleData>().id == n.id);

                if (index >= 0)
                {
                    // Object already in list
                    var bnpos = BNList[index].GetComponent<TriangleData>();
                    var m = Panel.GetComponent<LocationController>();
					var map = m.GetComponent<OnlineMaps> ();
					if (bnpos.lng - n.lon < MAX_DIFFERENCE)
                    	bnpos.lng = n.lon;
					if(bnpos.lat - n.lat < MAX_DIFFERENCE)
                    	bnpos.lat = n.lat;
					
                    bnpos.alt = n.alt;
                    bnpos.tmp = n.temp;
                    bnpos.UpdateFocus();

					//added 1/26/18 by Alex

					double xpos, ypos;
					if (map != null) {
						xpos = (bnpos.lng - m.lngMinMap) / (m.lngMaxMap - m.lngMinMap) * (2 * map.position.x) + map.position.x;
						ypos = (bnpos.lat - m.latMinMap) / (m.latMaxMap - m.latMinMap) * (2 * map.position.y) + map.position.y;
					} else {
						xpos = (bnpos.lng - m.lngMinMap) / (m.lngMaxMap - m.lngMinMap) * 40.96 - 20.48;
						ypos = (bnpos.lat - m.latMinMap) / (m.latMaxMap - m.latMinMap) * 40.96 - 20.48;
					}
					//==============================


                    BNList[index].transform.localPosition = new Vector3((float)xpos, (float)ypos, 1);
                }
                else
                {
                    GameObject nobj = (GameObject)Instantiate(BNPrefab);
                    var data = nobj.GetComponent<TriangleData>();

                    data.lat = n.lat;
                    data.lng = n.lon;
//					Debug.Log (data.lng);
                    data.alt = 1;
                    data.id = n.id;
                    data.tmp = n.temp;

                    data.RestManager = NetworkManager;
                    data.Spawner = Spawner;
                    data.Self = nobj;
                    data.FocusWindow = FocusWindow;

                    nobj.transform.parent = Spawner.transform;
                    nobj.GetComponent<Transform>().localScale = new Vector3(TriScale * ZoomController.ZoomScale, TriScale * ZoomController.ZoomScale, 1);
                    BNList.Add(nobj);
                    Panel.GetComponent<LocationController>().UpdateMap();
                    terminal.GetComponent<TerminalController>().TerminalColorPrint("New BN object with ID: " + data.id, nobj.GetComponent<SpriteRenderer>().color);
                }
            }
        }

        if (UIFunctions.closeScene)
        {
            foreach (GameObject bn in BNList)
            {
                Destroy(bn);
            }
            BNList.Clear();
        }
    }

	public Boolean UpdateCenter()
	{
		float latMin;
		float lngMin;
		float latMax;
		float lngMax;

		if (BNList.Count > 0)
		{
			latMin = latMax = BNList[0].GetComponent<TriangleData>().lat;
			lngMin = lngMax = BNList[0].GetComponent<TriangleData>().lng;

			foreach (GameObject bn in BNList)
			{
				var data = bn.GetComponent<TriangleData>();
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

			/*
            Debug.Log("###########################################");
            Debug.Log(latRange);
            Debug.Log(lngRange);
            Debug.Log(latCenter);
            Debug.Log(lngCenter);
            */
		}
		return false;
	}
    public void UpdateScale()
	{
        foreach (GameObject bn in BNList)
        {
            bn.GetComponent<Transform>().localScale = new Vector3(TriScale * ZoomController.ZoomScale, TriScale * ZoomController.ZoomScale, 1);
        }
    }

    public float maxTemp()
    {
        float max = -1;
        foreach(GameObject bn in BNList)
        {
            max = Math.Max(max, bn.GetComponent<TriangleData>().tmp);
        }
        return max;
    }
}

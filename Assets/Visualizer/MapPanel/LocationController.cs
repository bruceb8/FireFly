using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class LocationController : MonoBehaviour {
    public GameObject map;
    public GameObject ffspawner;
    public GameObject bnspawner;
    public GameObject slider;
	public OnlineMaps myMap;

    public double latMinMap;
    public double lngMinMap;
    public double latMaxMap;
    public double lngMaxMap;
	public bool setrun;

    // Use this for initialization
    void Start () {
        //yield return new WaitForSeconds(5);
        //UpdateMap();
		setrun = true;
	}

	// Update is called once per frame
	void Update () {

	}

    public void UpdateMap()
    {
        var ffs = ffspawner.GetComponent<FFSpawner>();
        var bns = bnspawner.GetComponent<BNSpawner>();
        var m = map.GetComponent<OnlineMaps>();



		if ((bns.UpdateCenter () || ffs.UpdateCenter ()) && setrun ) {
			setrun = false; //only want this code below to run once a beacon or ff has spawned on map

			if (ffs.FFList.Count < 1 && bns.BNList.Count > 0) {
				m.SetPosition (bns.lngCenter, bns.latCenter);
			} else if (ffs.FFList.Count > 0 && bns.BNList.Count < 1) {
				m.SetPosition (ffs.lngCenter, ffs.latCenter);
			} else
				m.SetPosition ((ffs.lngCenter + bns.lngCenter) / 2, (ffs.latCenter + bns.latCenter) / 2);

			//Get curent map longitude and latitude positions (4 coordinates)
			m.GetBottomRightPosition (out lngMaxMap, out latMinMap);
			m.GetTopLeftPosition (out lngMinMap, out latMaxMap);
		} 

		foreach (GameObject ff in ffs.FFList)
		{
			var ffpos = ff.GetComponent<CircleData>();
			double xpos = (ffpos.lng - lngMinMap) / (lngMaxMap - lngMinMap) * (2* m.position.x) + m.position.x;
			double ypos = (ffpos.lat - latMinMap) / (latMaxMap - latMinMap) * (2 * m.position.y) + m.position.y;
			ff.transform.localPosition = new Vector3((float)xpos, (float)ypos, 1);//Move to new postion on map
		}

		foreach(GameObject bn in bns.BNList)
		{
			var bnpos = bn.GetComponent<TriangleData>();
			double xpos = (bnpos.lng - lngMinMap) / (lngMaxMap - lngMinMap) * (2* m.position.x) + m.position.x;
			double ypos = (bnpos.lat - latMinMap) / (latMaxMap - latMinMap) * (2 * m.position.y) + m.position.y;
			bn.transform.localPosition = new Vector3((float)xpos, (float)ypos, 1);//Move position
		}

		//Need to figure out if this doing the right thing...
		OnlineMapsTile.OnTileDownloaded += OnTileDownloaded;
		if (OnlineMapsCache.instance != null) OnlineMapsCache.instance.OnStartDownloadTile += OnStartDownloadTile;
		else OnlineMaps.instance.OnStartDownloadTile += OnStartDownloadTile;
    }

	public void centerMap(){
		var ffs = ffspawner.GetComponent<FFSpawner>();
		var bns = bnspawner.GetComponent<BNSpawner>();
		var m = map.GetComponent<OnlineMaps>();
		bns.UpdateCenter ();
		ffs.UpdateCenter ();

	}

	public static string GetTilePath(OnlineMapsTile tile)
	{
		string[] parts =
		{
//			Application.persistentDataPath,
//			"OnlineMapsTileCache",
			Application.dataPath,
			"/Resources/MapsCached",
			tile.mapType.provider.id,
			tile.mapType.id,
			tile.zoom.ToString(),
			tile.x.ToString(),
			tile.y + ".png"
		};
		return string.Join("/", parts);
	}


	public void OnStartDownloadTile(OnlineMapsTile tile)
	{
		// Get local path.
		string path = GetTilePath(tile);

		// If the tile is cached.
		if (File.Exists(path))
		{
			// Load tile texture from cache.
			Texture2D tileTexture = new Texture2D(256, 256, TextureFormat.RGB24, false);
			tileTexture.LoadImage(File.ReadAllBytes(path));
			tileTexture.wrapMode = TextureWrapMode.Clamp;

			// Send texture to map.
			if (OnlineMaps.instance.target == OnlineMapsTarget.texture)
			{
				tile.ApplyTexture(tileTexture);
				OnlineMaps.instance.buffer.ApplyTile(tile);
				OnlineMapsUtils.DestroyImmediate(tileTexture);
			}
			else
			{
				tile.texture = tileTexture;
				tile.status = OnlineMapsTileStatus.loaded;
			}

			// Redraw map.
			OnlineMaps.instance.Redraw();
		}
		else
		{
			// If the tile is not cached, download tile with a standard loader.
			OnlineMaps.instance.StartDownloadTile(tile);
		}
	}

	public void OnTileDownloaded(OnlineMapsTile tile)
	{
		// Get local path.
		string path = GetTilePath(tile);
		// Cache tile.
		FileInfo fileInfo = new FileInfo(path);
		DirectoryInfo directory = fileInfo.Directory;
		if (!directory.Exists) directory.Create();

		File.WriteAllBytes(path, tile.www.bytes);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MarkerManager : MonoBehaviour {
	public GameObject map;
	private OnlineMaps m;
	public WSNetworkManager WSManager; 
	private Texture2D FF_texture, BN_texture, DN_texture;

	// Use this for initialization
	void Start () {
		m = map.GetComponent<OnlineMaps>();
		//load textures for devices
		byte[] FileData;

		FileData = File.ReadAllBytes("Assets/Resources/Square2.png");//firefighter texture
		FF_texture = new Texture2D(2, 2);           // Create new "empty" texture
		FF_texture.LoadImage(FileData);
	}
	
	// Update is called once per frame
	void Update () {
		updateMarkerPos ();
	}
		

	void updateMarkerPos(){
		List<Device_Status> ffs = WSManager.firefighters;
		List<Device_Status> bns = WSManager.beacons;
		List<Device_Status> dns = WSManager.drones;

		foreach(Device_Status ff in ffs){
			if (!ff.isMarker) {
				m.AddMarker (new Vector2 (ff.lon, ff.lat), FF_texture, ff.id);
				ff.isMarker = true;
			} else {
				updateMarker (ff);
			}
		}
		foreach (Device_Status bn in bns) {
			if (!bn.isMarker) {
				m.AddMarker (new Vector2 (bn.lon, bn.lat), FF_texture, bn.id);
				bn.isMarker = true;
			} else {
				updateMarker (bn);
			}
		}
		foreach (Device_Status dn in dns) {

		}
	}
	void updateMarker(Device_Status device){
		foreach (OnlineMapsMarker marker in m.markers) {
//			Debug.Log (device.id + "\t" + marker.label);
			if (device.id == marker.label) {
//				if (new Vector2 (device.lon, device.lat) != marker.position) {
//					Debug.Log ("Changed Position");
//					Debug.Log(device.lon + "\t" + device.lat);
					marker.SetPosition (device.lon, device.lat);
					OnlineMaps.instance.Redraw();
					break;
//				}
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct DevicePosition{
	float lat;
	float lon;

	public DevicePosition(float input_lat, float input_lon){
		lat = input_lat;
		lon = input_lon;
	}
}

//A general class that holds information that every firefighter, beacon or drone has.
public class Device_Status{
	public string id;
	public string type; 
	public float lat;
	public float lon;
	//Sensor information
	public float alt;
	public float temp;
	public string time;
	public float light;
	public float co;

	public List <DevicePosition> position_log;
	public bool isMarker = false;


	public Device_Status(Current_Status input){
		this.id = input.id;
//		this.type = input.type; 
		this.lat = input.lat;
		this.lon = input.lon;
		this.alt = input.alt;
		this.temp = input.temp;
		this.time = input.time;
		this.light = input.light;
		this.co = input.co;

		this.isMarker = false;
	}

	public void updateStatus(Device_Status input){
		this.alt = input.alt;
		this.temp = input.temp;
		this.time = input.time;
		this.light = input.light;
		this.co = input.co;

//		this.position_log.Add (new DevicePosition(this.lat,this.lon));
		this.lat = input.lat;
		this.lon = input.lon;
	}

	public static bool operator == (Device_Status a, Device_Status b){
		if (a.id == b.id) return true;
		return false;

	}
	public static bool operator != (Device_Status a, Device_Status b){
		if (a.id != b.id) return true;
		return false;

	}
    //public bool Equals(Object obj)
    //{
    //    Device_Status t = obj as Device_Status;
    //    if(this.id ==t.id)
    //        return true;
    //    return false;
    //}

    //public override int GetHashCode()//fix later!
    //{
    //    return 1;
    //}
}

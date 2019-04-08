using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DevicePosition{
	public double x;
	public double y;

	public DevicePosition(double input_x, double input_y){
		x = input_x;
		y = input_y;
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

    public List<Vector2> position_log = new List<Vector2>();
    public Color32 marker_color;
    public OnlineMapsMarker marker;
    public OnlineMapsDrawingLine line;
    public bool path_is_drawn = false;
	public bool isMarker = false;


	public Device_Status(Current_Status input){
        this.id = input.id;
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
		this.lat = input.lat;
		this.lon = input.lon;
	}

	public static bool operator == (Device_Status a, Device_Status b){
        if (object.ReferenceEquals(a, null) &&  !object.ReferenceEquals(b, null)) return false;
        if (!object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null)) return false;
        if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null)) return true;
     

        if (a.id == b.id) return true;

		return false;

	}
	public static bool operator != (Device_Status a, Device_Status b){
        if (object.ReferenceEquals(a, null) && !object.ReferenceEquals(b, null)) return true;
        if (!object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null)) return true;
        if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null)) return false;
    

        if (a.id != b.id) return true;
		return false;

	}

    public string toString()
    {
        return "ID: " + this.id
            + "\nType: " + this.type
            + "\nLatitude: " + this.lat
            + "\nLongitdude: " + this.lon
            + "\nAltitdude: " + this.alt
            + "\nTemperature: " + this.temp
            + "\nCO: " + this.co;
    }
   
}

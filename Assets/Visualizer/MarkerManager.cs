using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class MarkerManager : MonoBehaviour
{
    public GameObject map;
    private OnlineMaps m;
    private OnlineMapsMarkerManager m_manager;
    public WSNetworkManager WSManager;

    public MarkerTextureUtility m_texture;

    public FocusWindowController FocusWindow;
    public UIFunctions UI;

    public Device_Status current_target_status;
    public Color32 current_target_marker_color;
    private Texture2D current_target_old_texture;

    // Use this for initialization
    void Start()
    {
        m = map.GetComponent<OnlineMaps>();
        m_manager = map.GetComponent<OnlineMapsMarkerManager>();
        m_texture = this.GetComponent<MarkerTextureUtility>();

    }

    // Update is called once per frame
    void Update()
    {
        updateMarkerPos();
    }


    void updateMarkerPos()
    {
        List<Device_Status> ffs = WSManager.firefighters;
        List<Device_Status> bns = WSManager.beacons;
        List<Device_Status> dns = WSManager.drones;

        foreach (Device_Status ff in ffs)
        {
            if (!ff.isMarker)
            {
                OnlineMapsMarker temp = m_manager.Create(new Vector2(ff.lon, ff.lat), m_texture.FF_texture , "FF:" + ff.id);
                ff.marker_color = m_texture.FF_color;
                temp.scale = 0.1f;
                ff.marker = temp;

                ff.isMarker = true;
                temp.OnClick += OnMarkerClick;
            }
            else
            {
                updateMarker(ff);
            }
        }
        foreach (Device_Status bn in bns)
        {
            if (!bn.isMarker)
            {
                OnlineMapsMarker temp = m_manager.Create(new Vector2(bn.lon, bn.lat), m_texture.BN_texture, "BN:" + bn.id);
                bn.marker_color = m_texture.BN_color;
                temp.scale = 0.1f;
                bn.marker = temp;
                temp.OnClick += OnMarkerClick;
                bn.isMarker = true;
            }
            else
            {
                updateMarker(bn);
            }
        }
        foreach (Device_Status dn in dns)
        {
            if (!dn.isMarker)
            {
                OnlineMapsMarker temp = m_manager.Create(new Vector2(dn.lon, dn.lat), m_texture.DN_texture, "DN:" + dn.id);
                
                temp.scale = 0.1f;
                dn.marker = temp;
                dn.marker_color = m_texture.DN_color;

                temp.OnClick += OnMarkerClick;
                dn.isMarker = true;
            }
            else
            {
                updateMarker(dn);
            }
        }
    }

    private void updateMarker(Device_Status device)
    {
        if (new Vector2(device.lon, device.lat) != device.marker.position)
        {
            device.marker.SetPosition(device.lon, device.lat);

            device.position_log.Add(device.marker.position);
            OnlineMaps.instance.Redraw();
        }
    }

    //click on marker event
    private void OnMarkerClick(OnlineMapsMarkerBase marker)
    {
        if (current_target_status == null) //nothing is currently selected...
        {
            OnlineMapsMarker next_marker = getMarker(marker.label);
            string[] tempstr = next_marker.label.Split(':');
            current_target_status = WSManager.GetDevice(tempstr[1], tempstr[0]);

            current_target_old_texture = m_texture.CopyTexture(next_marker.texture);
            next_marker.texture = m_texture.OverlayCurrentTarget(next_marker.texture); //add target texture overlay

            next_marker.scale = next_marker.scale * 2;

            current_target_status = WSManager.GetDevice(tempstr[1], tempstr[0]);
            current_target_marker_color = current_target_status.marker_color;
            //FocusWindow.updateText(current_target_status);


        }
        else //something is currently selected
        {
            if (current_target_status.marker.label != marker.label) // if a new target has been clicked on
            {
                //revert to base texture
                current_target_status.marker.texture = m_texture.CopyTexture(current_target_old_texture);

                //scale back down
                current_target_status.marker.scale = current_target_status.marker.scale / 2;


                OnlineMapsMarker next_marker = getMarker(marker.label);
                string[] tempstr = next_marker.label.Split(':');
                current_target_status = WSManager.GetDevice(tempstr[1], tempstr[0]);
                current_target_marker_color = current_target_status.marker_color;

                current_target_old_texture = m_texture.CopyTexture(next_marker.texture);
                next_marker.texture = m_texture.OverlayCurrentTarget(next_marker.texture); //add target texture overlay

                next_marker.scale = next_marker.scale * 2;


                //FocusWindow.updateText(current_target_status);

            }
            else //toggle off current selection meaning that the previous target is the same as the one we just clicked on.
            {

                current_target_status.marker.texture = m_texture.CopyTexture(current_target_old_texture);

                //scale back down
                current_target_status.marker.scale = current_target_status.marker.scale / 2;
                current_target_status = null;
               
                //FocusWindow.clearText();
            }

        }
    }

    public double GetMaxTemp() {

        double maxtemp = 0;
        foreach (Device_Status ff in WSManager.firefighters)
        {
            if (ff.temp >= maxtemp) maxtemp = ff.temp;
        }
        foreach(Device_Status bn in WSManager.beacons) {
            if (bn.temp >= maxtemp) maxtemp = bn.temp;
        }
        foreach(Device_Status dn in WSManager.drones)
        {
            if (dn.temp >= maxtemp) maxtemp = dn.temp;
        }
        return maxtemp;
    }

    public double GetMaxCO()
    {
        double maxco = 0;
        foreach (Device_Status ff in WSManager.firefighters)
        {
            if (ff.co >= maxco) maxco = ff.temp;
        }
        foreach (Device_Status bn in WSManager.beacons)
        {
            if (bn.co >= maxco) maxco = bn.temp;
        }
        foreach (Device_Status dn in WSManager.drones)
        {
            if (dn.co >= maxco) maxco = dn.temp;
        }
        return maxco;
    }
    private OnlineMapsMarker getMarker(string label)
    {
        foreach (OnlineMapsMarker marker in m_manager.items)
        {
            if (marker.label == label) return marker;
        }
        return null;
    }

}

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
    private Texture2D FF_texture, BN_texture, DN_texture, Currently_Selected;

    public UIFunctions UI;

    public OnlineMapsMarker current_target_marker;
    public Device_Status current_target_marker_status;
    public Color32 current_target_marker_color;
    private Texture2D current_target_old_texture;

    // Use this for initialization
    void Start()
    {
        m = map.GetComponent<OnlineMaps>();
        m_manager = map.GetComponent<OnlineMapsMarkerManager>();
        //load textures for devices
        //byte[] FileData;

        //FileData = File.ReadAllBytes("Assets/Resources/Square2.png");//firefighter texture
        //FF_texture = new Texture2D(2, 2);           // Create new "empty" texture
        //FF_texture.LoadImage(FileData);

        //byte[] FileData1;

        //FileData1 = File.ReadAllBytes("Assets/Resources/circle.png");//firefighter texture
        //DN_texture = new Texture2D(2, 2);           // Create new "empty" texture
        //DN_texture.LoadImage(FileData1);

        //byte[] FileData2;

        //FileData2 = File.ReadAllBytes("Assets/Resources/triangle.png");//firefighter texture
        //BN_texture = new Texture2D(2, 2);           // Create new "empty" texture
        //BN_texture.LoadImage(FileData2);

        //byte[] FileData3;

        //FileData3 = File.ReadAllBytes("Assets/Resources/target.png");//firefighter texture
        //Currently_Selected = new Texture2D(2, 2);           // Create new "empty" texture
        //Currently_Selected.LoadImage(FileData3);

        FF_texture = Resources.Load<Texture2D>("Square2");
        BN_texture = Resources.Load<Texture2D>("triangle");
        DN_texture = Resources.Load<Texture2D>("circle");
        Currently_Selected = Resources.Load<Texture2D>("target");
        current_target_marker = null;
        current_target_marker_status = null;

        current_target_old_texture = null;
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
                OnlineMapsMarker temp = m_manager.Create(new Vector2(ff.lon, ff.lat), changeTextureColor(FF_texture, out ff.marker_color), "FF:" + ff.id);
                temp.scale = 0.1f;
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
                OnlineMapsMarker temp = m_manager.Create(new Vector2(bn.lon, bn.lat), BN_texture, "BN:" + bn.id);
                temp.scale = 0.1f;
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
                OnlineMapsMarker temp = m_manager.Create(new Vector2(dn.lon, dn.lat), changeTextureColor(DN_texture,out dn.marker_color), "DN:" + dn.id);

                temp.scale = 0.1f;
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

        foreach (OnlineMapsMarker marker in m_manager.items)
        {
            if (device.id == marker.label.Split(':')[1])
            {
                if (new Vector2(device.lon, device.lat) != marker.position)
                {

                    marker.SetPosition(device.lon, device.lat);
                  
                    device.position_log.Add(marker.position);
                    OnlineMaps.instance.Redraw();
                    break;
                }
            }
        }
    }

   


    //click on marker event
    private void OnMarkerClick(OnlineMapsMarkerBase marker)
    {

        if (current_target_marker == null) //nothing is currently selected...
        {
            OnlineMapsMarker next_marker = getMarker(marker.label);
            current_target_old_texture = CopyTexture(next_marker.texture);
            next_marker.texture = OverlayCurrentTarget(next_marker.texture); //add target texture overlay

            next_marker.scale = next_marker.scale * 2;

            string[] tempstr = next_marker.label.Split(':');
            current_target_marker_color = WSManager.GetDevice(tempstr[1], tempstr[0]).marker_color;

            current_target_marker = next_marker;


        }
        else //something is currently selected
        {
            if (current_target_marker.label != marker.label) // if a new target has been clicked on
            {
                //revert to base texture

                current_target_marker.texture = CopyTexture(current_target_old_texture);

                //scale back down
                current_target_marker.scale = current_target_marker.scale / 2;

                OnlineMapsMarker next_marker = getMarker(marker.label);
                string[] tempstr = next_marker.label.Split(':');
                current_target_marker_color = WSManager.GetDevice(tempstr[1], tempstr[0]).marker_color;
                current_target_old_texture = CopyTexture(next_marker.texture);
                next_marker.texture = OverlayCurrentTarget(next_marker.texture); //add target texture overlay

                next_marker.scale = next_marker.scale * 2;



                current_target_marker = next_marker;


               
            }
            else //toggle off current selection meaning that the previous target is the same as the one we just clicked on.
            {
             
                current_target_marker.texture = CopyTexture(current_target_old_texture);

                //scale back down
                current_target_marker.scale = current_target_marker.scale / 2;
                current_target_marker = null;
                UI.path_is_drawn = false;
            }
            UI.ShowPath();
        }


        //show on window

    }
    private OnlineMapsMarker getMarker(string label)
    {
        foreach (OnlineMapsMarker marker in m_manager.items)
        {
            if (marker.label == label) return marker;
        }
        return null;
    }

    private Texture2D OverlayCurrentTarget(Texture2D input)
    {
        Color32[] pixels = input.GetPixels32(); //get current texture pixels
        Color32[] current_pixels = Currently_Selected.GetPixels32();
        Color32[] temppixels = new Color32[pixels.Length]; //empty texture
        Color32 overlayColor = new Color32(255, 0, 0, 255);

        for (int i = input.width * 5 + 1; i < input.width * input.height; i++)
        {
            if (current_pixels[i].a != 0)
            {
                temppixels[i] = overlayColor;
            }
            else
            {
                temppixels[i] = pixels[i];
            }
        }

        Texture2D output = new Texture2D(input.width, input.height);
        output.SetPixels32(temppixels, 0);
        output.Apply();
        return output;
    }

    private Texture2D CopyTexture(Texture2D input)
    {
        Color32[] pixels = input.GetPixels32();
        Color32[] temppixels = new Color32[pixels.Length];

        for (int i = 0; i < input.height * input.width; i++)
        {
            temppixels[i] = pixels[i];
        }

        Texture2D output = new Texture2D(input.width, input.height);
        output.SetPixels32(temppixels, 0);
        output.Apply();
        return output;
    
}

    private Texture2D GetBaseMarkerTexture(OnlineMapsMarker marker)
    {
        string markerID = marker.label;
        StringComparison something = StringComparison.InvariantCulture;
        if (markerID.StartsWith("FF:", something))
        {
            return FF_texture;
        }

        else if (markerID.StartsWith("BN:", something))
        {
            return BN_texture;
        }
        else if (markerID.StartsWith("DN:", something))
        {
            return DN_texture;
        }
        return null;

    }
    public Texture2D SetMarkerTextureColor(Color32 new_color, Texture2D tex)
    {

        Color32[] pixels = tex.GetPixels32();
        Color32[] temppixels = new Color32[pixels.Length];
      
        for (int i = 0; i < tex.height * tex.width; i++)
        {
            if (pixels[i].a != 0)
            {
                temppixels[i] = new_color;
            }
            else
            {
                temppixels[i] = pixels[i];
            }
        }

        Texture2D output = new Texture2D(tex.width, tex.height);
        output.SetPixels32(temppixels, 0);
        output.Apply();
        return output;
    }

    //Make a random color for the the device textures. This is assumes simple 
    //textures such that a non transparent color will be changed by this function.
    public Texture2D changeTextureColor(Texture2D input, out Color32 newColor)
    {
        Color32[] pixels = input.GetPixels32();
        Color32[] temppixels = new Color32[pixels.Length];newColor = new Color32(
             (byte)UnityEngine.Random.Range(0, 255),        // R
             (byte)UnityEngine.Random.Range(0, 255),        // G
             (byte)UnityEngine.Random.Range(0, 255),        // B
             200);      // A

        for (int i = 0; i < input.height * input.width; i++)
        {
            if (pixels[i].a != 0)
            {
                temppixels[i] = newColor;
            }
            else
            {
                temppixels[i] = pixels[i];
            }
        }

        Texture2D output = new Texture2D(input.width, input.height);
        output.SetPixels32(temppixels, 0);
        output.Apply();
        return output;
    }
}

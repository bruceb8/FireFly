using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MarkerManager : MonoBehaviour
{
    public GameObject map;
    private OnlineMaps m;
    private OnlineMapsMarkerManager m_manager;
    public WSNetworkManager WSManager;
    private Texture2D FF_texture, BN_texture, DN_texture;

    // Use this for initialization
    void Start()
    {
        m = map.GetComponent<OnlineMaps>();
        m_manager = map.GetComponent<OnlineMapsMarkerManager>();
        //load textures for devices
        byte[] FileData;

        FileData = File.ReadAllBytes("Assets/Resources/Square2.png");//firefighter texture
        FF_texture = new Texture2D(2, 2);           // Create new "empty" texture
        FF_texture.LoadImage(FileData);

        byte[] FileData1;

        FileData1 = File.ReadAllBytes("Assets/Resources/circle.png");//firefighter texture
        DN_texture = new Texture2D(2, 2);           // Create new "empty" texture
        DN_texture.LoadImage(FileData1);

        byte[] FileData2;

        FileData2 = File.ReadAllBytes("Assets/Resources/triangle.png");//firefighter texture
        BN_texture = new Texture2D(2, 2);           // Create new "empty" texture
        BN_texture.LoadImage(FileData2);
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
                m_manager.Create(new Vector2(ff.lon, ff.lat), changeTextureColor(FF_texture), ff.id);
                ff.isMarker = true;
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
                m_manager.Create(new Vector2(bn.lon, bn.lat), BN_texture, bn.id);
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
                m_manager.Create(new Vector2(dn.lon, dn.lat), changeTextureColor(DN_texture), dn.id);
                dn.isMarker = true;
            }
            else
            {
                updateMarker(dn);
            }
        }
    }

    void updateMarker(Device_Status device)
    {
        foreach (OnlineMapsMarker marker in m_manager.items)
        {
            if (device.id == marker.label)
            {
                if (new Vector2(device.lon, device.lat) != marker.position)
                {
                    marker.SetPosition(device.lon, device.lat);
                    OnlineMaps.instance.Redraw();
                    break;
                }
            }
        }
    }

    //Since texture 2d is annoying we will have to grab the pixel values and change the
    // color ourselves!
    public Texture2D changeTextureColor(Texture2D input)
    {
        Color32[] pixels = input.GetPixels32();
        Color32[] temppixels = new Color32[pixels.Length];
        Color32 newColor = new Color32(
             (byte)Random.Range(0, 255),        // R
             (byte)Random.Range(0, 255),        // G
             (byte)Random.Range(0, 255),        // B
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

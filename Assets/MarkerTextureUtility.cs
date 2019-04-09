using UnityEngine;
using System.Collections;
using System;

public class MarkerTextureUtility : MonoBehaviour
{
    public Texture2D FF_texture, BN_texture, DN_texture, Currently_Selected;

    // Use this for initialization
    void Start()
    {
        FF_texture = Resources.Load<Texture2D>("Square2");
        BN_texture = Resources.Load<Texture2D>("triangle");
        DN_texture = Resources.Load<Texture2D>("circle");
        Currently_Selected = Resources.Load<Texture2D>("target");
    }

    public Texture2D OverlayCurrentTarget(Texture2D input)
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

    public Texture2D CopyTexture(Texture2D input)
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

    public Texture2D GetBaseMarkerTexture(OnlineMapsMarker marker)
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
        Color32[] temppixels = new Color32[pixels.Length]; newColor = new Color32(
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*Script File 2 of 2 for Menu Scene
//This handles all the Settings needed for the visualizer scene
	All the settings are saved as player prefs. This is a unity global way of saving data and is used by other
	scripts in the project to grab shared data.
	but it is not the only way. It can be accessed by "players" and manipulated. So not secure I would say...
*/


public class SettingsController : MonoBehaviour {

    public GameObject SettingsPanel;
    public GameObject SettingsIcon;
    public GameObject CloseIcon;
    public GameObject IPField;
    public GameObject PortField;
    public GameObject FFSlider;
    public GameObject FFExample;
    public GameObject BNSlider;
    public GameObject BNExample;

    private bool isActive = false;

	// Use this for initialization
	void Start () {
        IPField.GetComponent<InputField>().text = PlayerPrefs.GetString("ServerIP");
        PortField.GetComponent<InputField>().text = PlayerPrefs.GetString("ServerPort");
        float scale = PlayerPrefs.GetFloat("DotScale");//Player prefs is a way of saving data in unity, not the only way though...
        FFSlider.GetComponent<Slider>().value = scale;
        FFExample.transform.localScale = new Vector3(scale * 100, scale * 100, 1);
        scale = PlayerPrefs.GetFloat("TriScale");
        BNSlider.GetComponent<Slider>().value = scale;
        BNExample.transform.localScale = new Vector3(scale * 100, scale * 100, 1);
    }
	// Update is called once per frame
	void Update () {
		
	}
    public void ToggleSettings()
    {
        if (isActive)
        {
            SettingsPanel.transform.GetComponent<RectTransform>().pivot = new Vector2(0, (float)0.5);
            SettingsIcon.GetComponent<Renderer>().enabled = true;
            CloseIcon.GetComponent<Renderer>().enabled = false;
            isActive = false;
        }
        else
        {
            SettingsPanel.transform.GetComponent<RectTransform>().pivot = new Vector2(1, (float)0.5);
            SettingsIcon.GetComponent<Renderer>().enabled = false;
            CloseIcon.GetComponent<Renderer>().enabled = true;
            isActive = true;
        }
    }

    public void UpdateIP(string ip)//Input from the IP input box in Settings
    {
        PlayerPrefs.SetString("ServerIP", ip);
    }

    public void UpdatePort(string port)//Input from the Port input box in Settings
    {
        PlayerPrefs.SetString("ServerPort", port);
    }

    public void UpdateFF(float scale)//input is from the slider in Fire Fighter Icon Size Settings
    {
        FFExample.transform.localScale = new Vector3(scale*100, scale*100, 1);
        PlayerPrefs.SetFloat("DotScale", scale);
    }

    public void UpdateBN(float scale)//Input is from the slider in Beacon Icon Size Settings
    {
        BNExample.transform.localScale = new Vector3(scale*100, scale*100, 1);
        PlayerPrefs.SetFloat("TriScale", scale);
    }
}

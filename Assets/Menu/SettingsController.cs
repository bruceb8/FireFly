using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SettingsController : MonoBehaviour {

    public GameObject SettingsPanel;
    public GameObject SettingsIcon;
    public GameObject CloseIcon;
    public GameObject IPField;
    public GameObject PortField;
    public Toggle ToggleChief;

    private bool isActive = false;

	// Use this for initialization
	void Start () {
        IPField.GetComponent<InputField>().text = PlayerPrefs.GetString("ServerIP");
        PortField.GetComponent<InputField>().text = PlayerPrefs.GetString("ServerPort");
        PlayerPrefs.SetInt("UserIsChief", 1);
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

    public void UpdateClientChief()
    {
        if (ToggleChief.isOn)
        {
            PlayerPrefs.SetInt("UserIsChief", 1);
        }
        else
        {
            PlayerPrefs.SetInt("UserIsChief", 0);
        }
    }
}

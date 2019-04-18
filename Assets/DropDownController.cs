using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownController : MonoBehaviour
{
    // Start is called before the first frame update
   Dropdown.OptionData m_NewData, m_NewData2;
    
    List<Dropdown.OptionData> m_Messages = new List<Dropdown.OptionData>();
    
    Dropdown DroneSelect;
    string m_MyString;
    int m_Index;

    public WSNetworkManager wsManager;

    int droneCount = 0;
    
    //This is the string that stores the current selection m_Text of the Dropdown
    string m_Message;

    int m_DropdownValue;

    void Start()
    {
        //Fetch the Dropdown GameObject the script is attached to
        DroneSelect = GetComponent<Dropdown>();

        


        //Clear the old options of the Dropdown menu
        //DroneSelect.ClearOptions();

         //Create a new option for the Dropdown menu which reads "Option 1" and add to messages List



        //Take each entry in the message List
        // foreach (Dropdown.OptionData message in m_Messages)
        // {
        //     //Add each entry to the Dropdown
        //     DroneSelect.options.Add(message);
        //     //Make the index equal to the total number of entries
        //     m_Index = m_Messages.Count - 1;
        // }
    }

    //Ouput the new value of the Dropdown into Text

    public string getValue(){
        return DroneSelect.options[DroneSelect.value].text;
    }

    // Update is called once per frame
    void Update()
    {
        //if there is a new drone in the list, then add it.  We need a 
        //length number of the list so we arent updating this all the time

        if(wsManager.drones.Count > droneCount){
            DroneSelect.ClearOptions();
            
            m_NewData = new Dropdown.OptionData();
                m_NewData.text = "No Drone";
                DroneSelect.options.Add(m_NewData);
                DroneSelect.value = 0;

            foreach( Device_Status d in wsManager.drones){
                m_NewData = new Dropdown.OptionData();
                m_NewData.text = d.id;
                DroneSelect.options.Add(m_NewData);
                droneCount++;

            }
            DroneSelect.RefreshShownValue();
        }

    }

}

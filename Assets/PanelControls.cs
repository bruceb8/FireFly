using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelControls : MonoBehaviour
{
    public Vector2 away = new Vector2(1, (float) 0.5) ;
    public Vector2 onScreen = new Vector2(0, (float) 0.5);
    
    bool isActive = false;
 
    public void ToggleMenu() {
        if(isActive == true){
            GetComponent<RectTransform>().pivot = away;
            isActive = false;
        }else{
            GetComponent<RectTransform>().pivot = onScreen;
            isActive = true;
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartVisualizer : MonoBehaviour {

    public GameObject LoadingPrefab;
    public GameObject UICanvas;

    public void Startup()//This is called from the Start button in the Menu Scene
    {
		//This calls a loading screen as a transition from Menu scene to Visualizer scene
        GameObject LoadingScreen = Instantiate(LoadingPrefab); //Supposedly makes a copy of a GameObject 

        LoadingScreen.transform.parent = UICanvas.transform; //
        LoadingScreen.transform.localScale = new Vector3(1, 1, 1);

        SceneManager.LoadScene("Visualizer", LoadSceneMode.Single);
    }
}

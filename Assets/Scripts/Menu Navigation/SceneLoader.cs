using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour {

    public Scene SceneToLoad;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void LoadScene()
    {
        SceneManager.LoadScene(SceneToLoad.name);
    }

}

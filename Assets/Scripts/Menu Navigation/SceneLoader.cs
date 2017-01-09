using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadSceneFromIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void LoadSceneFromString(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}

using UnityEngine;

public class PlayLevelManager : MonoBehaviour {

    private static PlayLevelManager _instance;

    public static PlayLevelManager Instance
    {
        get { return _instance; }
    }

    [HideInInspector]
    public string filename;

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        _instance = this;
    }

}

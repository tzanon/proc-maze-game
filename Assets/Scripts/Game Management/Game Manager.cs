using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;

    public static GameManager Instance
    {
        get { return _instance; }
    }

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

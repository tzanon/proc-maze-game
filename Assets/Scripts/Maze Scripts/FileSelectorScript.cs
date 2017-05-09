using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FileSelectorScript : MonoBehaviour
{
    public Button FileButton;


	void Start ()
    {
        string[] filenames = Directory.GetFiles(@"Assets\MazeFiles\", "*.txt");
        /*
        foreach (string filename in filenames)
        {
            Button button = Instantiate(FileButton, SelectorContent.transform) as Button;
            button.onClick.AddListener(() => LoadFile(filename));
            Text buttonText = button.GetComponentInChildren<Text>();
            buttonText.text = filename.Substring(17);
        }*/
    }
	
}

using UnityEngine;
using System.Collections.Generic;

public class WallScript : MonoBehaviour
{
    private string direction;
    HashSet<string> possibleDirections;

    public string Direction
    {
        get { return direction; }
        set
        {
            if (possibleDirections.Contains(value))
                direction = value;
        }
    }

	void Start ()
    {
        possibleDirections = new HashSet<string>();
        possibleDirections.Add("left");
        possibleDirections.Add("right");
        possibleDirections.Add("up");
        possibleDirections.Add("down");
    }



}

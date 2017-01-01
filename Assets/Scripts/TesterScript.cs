using UnityEngine;
using System.Collections;

public class TesterScript : MonoBehaviour {

    public TileScript leftTile, rightTile;

	void Start ()
    {
        Debug.Log("left walls: " + leftTile.GetWallCode() + ", right walls: " + rightTile.GetWallCode());
    }

    public void DeleteWalls()
    {
        
        leftTile.RemoveWallsBetweenTiles(rightTile);
        Debug.Log("left walls: " + leftTile.GetWallCode() + ", right walls: " + rightTile.GetWallCode());
    }

}

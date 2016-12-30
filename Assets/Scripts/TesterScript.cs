using UnityEngine;
using System.Collections;

public class TesterScript : MonoBehaviour {

    public TileScript leftTile, rightTile;

	// Use this for initialization
	void Start () {
        leftTile.x = -2;
        leftTile.y = 0;

        rightTile.x = 5;
        rightTile.y = 0;
	}

    public void DeleteWalls()
    {
        leftTile.DeleteWallsBetweenTiles(rightTile);
    }

}

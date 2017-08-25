using UnityEngine;

public class PlayerController3D : PlayerController
{

    private const int MaxAmmo = 5; // format for constants?
    private int ammo;

	protected override void Start ()
    {
        movementFactor = 2f;
        rotationFactor = 85f;
	}
    
	protected override void Update ()
    {
        
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            MoveForward();
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            MoveBackward();
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            RotateRight();
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            RotateLeft();
        }
    }

}

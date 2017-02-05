using UnityEngine;

public class PlayerController3D : PlayerController {

    [SerializeField]
    private float movementFactor, rotationFactor;

	void Start ()
    {
	    
	}

	void Update ()
    {

        float currentRot = transform.rotation.eulerAngles.y;
        Vector3 direction = new Vector3(Mathf.Sin(currentRot), 0, Mathf.Cos(currentRot)).normalized;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += transform.forward * movementFactor * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += -transform.forward * movementFactor * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0, rotationFactor * Time.deltaTime, 0);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0, -rotationFactor * Time.deltaTime, 0);
        }
    }

    void FixedUpdate()
    {
        

    }

}

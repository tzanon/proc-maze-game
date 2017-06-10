using UnityEngine;

public class PlayerController3D : MonoBehaviour {

    [SerializeField]
    private float movementFactor, rotationFactor;

	void Start ()
    {
	    
	}

	void Update ()
    {
        
        float currentRot = transform.rotation.eulerAngles.y;
        Vector3 direction = new Vector3(Mathf.Sin(currentRot), 0, Mathf.Cos(currentRot)).normalized;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * movementFactor * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            transform.position += -transform.forward * movementFactor * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, rotationFactor * Time.deltaTime, 0);
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -rotationFactor * Time.deltaTime, 0);
        }
    }

    void FixedUpdate()
    {
        

    }

}

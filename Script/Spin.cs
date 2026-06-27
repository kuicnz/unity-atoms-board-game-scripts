using UnityEngine;

public class Spin : MonoBehaviour 
{
    float speed = 20f;

	void Update () 
    {
        transform.Rotate(Vector3.up, speed*Time.deltaTime);
	}

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}

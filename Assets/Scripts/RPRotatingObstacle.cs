using UnityEngine;

public class RPRotatingObstacle : MonoBehaviour
{
    public float rotationSpeed = 70f;
    public bool alternateDirection = false;

    private void Update()
    {
        float direction = alternateDirection ? -1f : 1f;
        transform.Rotate(Vector3.up, rotationSpeed * direction * Time.deltaTime, Space.World);
    }
}
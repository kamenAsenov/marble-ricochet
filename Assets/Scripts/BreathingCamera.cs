using UnityEngine;

public class BreathingCamera : MonoBehaviour
{
    public float breathingAmount = 0.008f;
    public float breathingSpeed = 1.2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * breathingSpeed) * breathingAmount;
        transform.localPosition = startPosition + new Vector3(0f, offset, 0f);
    }
}
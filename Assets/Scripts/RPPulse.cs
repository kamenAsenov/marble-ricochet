using UnityEngine;

public class RPPulse : MonoBehaviour
{
    public float pulseAmount = 0.045f;
    public float pulseSpeed = 2.2f;

    private Vector3 startScale;

    private void Start()
    {
        startScale = transform.localScale;
    }

    private void Update()
    {
        float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = startScale * scale;
        transform.Rotate(Vector3.up, 28f * Time.deltaTime, Space.World);
    }
}
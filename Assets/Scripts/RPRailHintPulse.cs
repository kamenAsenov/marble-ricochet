using UnityEngine;

public class RPRailHintPulse : MonoBehaviour
{
    public float pulseAmount = 0.035f;
    public float pulseSpeed = 2.0f;

    private Vector3 startScale;

    private void Start()
    {
        startScale = transform.localScale;
    }

    private void Update()
    {
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = new Vector3(startScale.x, startScale.y * pulse, startScale.z);
    }
}
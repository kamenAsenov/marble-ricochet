using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    public Light targetLight;
    public float minDelay = 0.05f;
    public float maxDelay = 0.2f;

    private float nextFlickerTime;

    private void Update()
    {
        if (targetLight == null) return;

        if (Time.time >= nextFlickerTime)
        {
            targetLight.enabled = !targetLight.enabled;
            nextFlickerTime = Time.time + Random.Range(minDelay, maxDelay);
        }
    }
}
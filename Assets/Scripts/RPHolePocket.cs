using UnityEngine;

public class RPHolePocket : MonoBehaviour
{
    public RPHazardManager hazardManager;
    public float pullVisualDown = 0.18f;

    private void OnTriggerEnter(Collider other)
    {
        RPBulletBall ball = other.GetComponent<RPBulletBall>();

        if (ball == null)
        {
            return;
        }

        if (hazardManager == null)
        {
            hazardManager = FindFirstObjectByType<RPHazardManager>();
        }

        if (hazardManager != null)
        {
            hazardManager.OnBallEnteredHole(ball, transform.position);
        }
    }
}
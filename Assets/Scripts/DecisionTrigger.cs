using UnityEngine;

public class DecisionTrigger : MonoBehaviour
{
    public LoopManager loopManager;
    public PlayerDecision decision;

    private float lastTriggerTime = -10f;
    private const float cooldownSeconds = 0.75f;

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - lastTriggerTime < cooldownSeconds)
        {
            return;
        }

        if (other.GetComponent<CharacterController>() == null)
        {
            return;
        }

        lastTriggerTime = Time.time;

        if (loopManager != null)
        {
            loopManager.RegisterDecision(decision);
        }
    }
}
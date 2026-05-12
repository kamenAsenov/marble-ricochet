using UnityEngine;

public class LoopTrigger : MonoBehaviour
{
    public LoopManager loopManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() == null || loopManager == null)
        {
            return;
        }

        loopManager.RegisterDecision(PlayerDecision.ContinueForward);
    }
}
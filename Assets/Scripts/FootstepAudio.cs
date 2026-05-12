using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    public AudioSource footstepSource;
    public float walkThreshold = 0.1f;

    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (controller == null || footstepSource == null) return;

        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0f;

        bool isWalking = horizontalVelocity.magnitude > walkThreshold;

        if (isWalking && !footstepSource.isPlaying) footstepSource.Play();
        else if (!isWalking && footstepSource.isPlaying) footstepSource.Stop();
    }
}
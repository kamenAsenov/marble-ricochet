using System.Collections;
using UnityEngine;

public class RPCameraShake : MonoBehaviour
{
    private Vector3 startPosition;
    private Coroutine shakeRoutine;

    private void Start()
    {
        startPosition = transform.position;
    }

    public void Shake(float duration, float amount)
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }

        shakeRoutine = StartCoroutine(ShakeRoutine(duration, amount));
    }

    private IEnumerator ShakeRoutine(float duration, float amount)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            Vector3 offset = new Vector3(
                Random.Range(-amount, amount),
                0f,
                Random.Range(-amount, amount)
            );

            transform.position = startPosition + offset;

            yield return null;
        }

        transform.position = startPosition;
        shakeRoutine = null;
    }
}
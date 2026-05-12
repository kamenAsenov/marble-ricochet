using UnityEngine;

public class RPEffectAutoDestroy : MonoBehaviour
{
    public float lifetime = 1.4f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}

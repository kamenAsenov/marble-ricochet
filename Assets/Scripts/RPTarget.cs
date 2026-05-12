using UnityEngine;

public class RPTarget : MonoBehaviour
{
    public GameObject popEffectPrefab;

    private RPGameManager gameManager;
    private bool popped = false;

    public void SetGameManager(RPGameManager manager)
    {
        gameManager = manager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (popped)
        {
            return;
        }

        RPBulletBall ball = other.GetComponent<RPBulletBall>();

        if (ball != null && gameManager != null)
        {
            gameManager.OnTargetHit(this);
        }
    }

    public void Pop()
    {
        popped = true;

        if (popEffectPrefab != null)
        {
            GameObject effect = Instantiate(popEffectPrefab, transform.position, Quaternion.identity);
            effect.SetActive(true);
            Destroy(effect, 1.4f);
        }

        Destroy(gameObject);
    }
}
using UnityEngine;

public class RPObstacle : MonoBehaviour
{
    public RPObstacleType type = RPObstacleType.NormalWall;
    public RPGameManager gameManager;
    public GameObject breakEffectPrefab;

    private bool broken = false;

    public void OnBallHit(RPBulletBall ball)
    {
        RPComboSkillManager combo = FindFirstObjectByType<RPComboSkillManager>();

        if (type == RPObstacleType.MetalBumper)
        {
            if (combo != null)
            {
                combo.RegisterBumperHit();
            }

            gameManager?.OnBumperHit();
            return;
        }

        if (type == RPObstacleType.BreakableGlass && !broken)
        {
            broken = true;

            if (combo != null)
            {
                combo.RegisterGlassBreak();
            }

            if (breakEffectPrefab != null)
            {
                GameObject effect = Instantiate(breakEffectPrefab, transform.position, Quaternion.identity);
                effect.SetActive(true);
                Destroy(effect, 1.4f);
            }

            gameManager?.OnObstacleBroken(this);
            Destroy(gameObject);
        }
    }
}
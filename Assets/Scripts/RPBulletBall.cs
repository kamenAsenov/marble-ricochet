using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RPBulletBall : MonoBehaviour
{
    public float maxMovingTime = 6.0f;
    public float stopBelowSpeed = 0.18f;

    private Rigidbody rb;
    private RPGameManager gameManager;
    private float movingTimer;
    private float slowTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void Update()
    {
        float speed = GetVelocity().magnitude;

        if (speed > stopBelowSpeed)
        {
            movingTimer += Time.deltaTime;
            slowTimer = 0f;
        }
        else if (speed > 0.01f)
        {
            slowTimer += Time.deltaTime;

            if (slowTimer >= 0.75f)
            {
                gameManager?.ResetShot();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        RPObstacle obstacle = collision.collider.GetComponent<RPObstacle>();

        if (obstacle != null)
        {
            obstacle.OnBallHit(this);
        }

        string objName = collision.collider.gameObject.name.ToLowerInvariant();

        if (
            objName.Contains("rail") ||
            objName.Contains("wall") ||
            objName.Contains("bounce") ||
            objName.Contains("normalwall")
        )
        {
            RPComboSkillManager combo = FindFirstObjectByType<RPComboSkillManager>();

            if (combo != null)
            {
                combo.RegisterRailBounce();
            }
        }
    }

    public void SetGameManager(RPGameManager manager)
    {
        gameManager = manager;
    }

    public void Launch(Vector3 direction, float power)
    {
        movingTimer = 0f;
        slowTimer = 0f;
        SetVelocity(direction.normalized * power);
    }

    public void ResetBall(Vector3 position)
    {
        transform.position = position;
        StopBall();
    }

    public void StopBall()
    {
        SetVelocity(Vector3.zero);
        rb.angularVelocity = Vector3.zero;
        movingTimer = 0f;
        slowTimer = 0f;
    }

    public bool IsMovingTooLong()
    {
        return movingTimer >= maxMovingTime;
    }

    private Vector3 GetVelocity()
    {
#if UNITY_6000_0_OR_NEWER
        return rb.linearVelocity;
#else
        return rb.velocity;
#endif
    }

    private void SetVelocity(Vector3 velocity)
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = velocity;
#else
        rb.velocity = velocity;
#endif
    }
}
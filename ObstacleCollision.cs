using UnityEngine;

/// <summary>
/// Attach to any obstacle (for example TrashBin prefabs).
/// Applies a penalty or restarts the level when the player collides.
/// </summary>
public class ObstacleCollision : MonoBehaviour
{
    [Tooltip("If true, restarting the level instead of deducting points")]
    public bool restartOnHit = false;

    [Header("Optional feedback")]
    public AudioSource hitSound;
    public ParticleSystem hitParticles;
    public float hitCooldown = 0.35f;

    float _nextAllowedHitTime;

    void OnCollisionEnter(Collision collision)
    {
        NotifyPlayerHit(collision.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        NotifyPlayerHit(other.gameObject);
    }

    public void NotifyPlayerHit(GameObject playerObject)
    {
        if (!IsPlayer(playerObject))
            return;

        if (Time.time < _nextAllowedHitTime)
            return;

        _nextAllowedHitTime = Time.time + hitCooldown;
        HandleHit(playerObject);
    }

    bool IsPlayer(GameObject go)
    {
        if (go.CompareTag("Player"))
            return true;

        if (go.GetComponent<Mover>() != null || go.GetComponent<MoverLevelTwo>() != null)
            return true;

        if (go.GetComponentInParent<Mover>() != null || go.GetComponentInParent<MoverLevelTwo>() != null)
            return true;

        return false;
    }

    void HandleHit(GameObject playerObject)
    {
        Debug.Log("[Obstacle] Player hit: " + gameObject.name);

        if (hitSound != null)
            hitSound.Play();

        if (hitParticles != null)
            hitParticles.Play();

        ApplyPlayerImpact(playerObject);

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[Obstacle] No GameManager found!");
            return;
        }

        if (restartOnHit)
            GameManager.Instance.TimeUp();
        else
            GameManager.Instance.ApplyPenalty();
    }

    void ApplyPlayerImpact(GameObject playerObject)
    {
        Mover mover = playerObject.GetComponentInParent<Mover>();
        if (mover != null)
        {
            mover.ApplyObstacleImpact(transform.position);
            return;
        }

        MoverLevelTwo moverLevelTwo = playerObject.GetComponentInParent<MoverLevelTwo>();
        if (moverLevelTwo != null)
            moverLevelTwo.ApplyObstacleImpact(transform.position);
    }
}

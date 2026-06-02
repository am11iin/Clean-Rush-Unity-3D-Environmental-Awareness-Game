using UnityEngine;

/// <summary>
/// Attach to the player body object that owns the active collider.
/// Mover.cs adds this automatically at runtime.
/// Walks up the parent hierarchy to find TrashItem or obstacle logic.
/// </summary>
public class PlayerCollisionForwarder : MonoBehaviour
{
    float _nextFallbackObstacleTime;

    void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.gameObject);
    }

    void HandleHit(GameObject obj)
    {
        TrashItem trash = obj.GetComponentInParent<TrashItem>();
        if (trash != null)
        {
            trash.ForceCollect();
            return;
        }

        ObstacleCollision obstacle = obj.GetComponentInParent<ObstacleCollision>();
        if (obstacle != null)
        {
            obstacle.NotifyPlayerHit(gameObject);
            return;
        }

        Transform obstacleRoot = FindObstacleByName(obj.transform);
        if (obstacleRoot == null || Time.time < _nextFallbackObstacleTime)
            return;

        _nextFallbackObstacleTime = Time.time + 0.35f;
        ApplyObstacleImpact(obstacleRoot.position);
        GameManager.Instance?.ApplyPenalty();
    }

    Transform FindObstacleByName(Transform start)
    {
        Transform current = start;
        while (current != null)
        {
            if (LooksLikeObstacle(current.gameObject.name))
                return current;

            current = current.parent;
        }

        return null;
    }

    bool LooksLikeObstacle(string objectName)
    {
        string lowered = objectName.ToLowerInvariant();
        return lowered.Contains("bin") || lowered.Contains("barrier") || lowered.Contains("cone");
    }

    void ApplyObstacleImpact(Vector3 obstaclePosition)
    {
        Mover mover = GetComponentInParent<Mover>();
        if (mover != null)
        {
            mover.ApplyObstacleImpact(obstaclePosition);
            return;
        }

        MoverLevelTwo moverLevelTwo = GetComponentInParent<MoverLevelTwo>();
        if (moverLevelTwo != null)
            moverLevelTwo.ApplyObstacleImpact(obstaclePosition);
    }
}

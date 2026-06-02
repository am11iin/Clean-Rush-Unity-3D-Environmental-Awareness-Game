using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns trash items in the scene while keeping them readable and reachable.
/// </summary>
public class TrashSpawner : MonoBehaviour
{
    [Header("Drag your trash objects from the scene hierarchy here")]
    public GameObject[] trashObjects;

    [Tooltip("Total trash items in the scene (6 = Level 1, 12 = Level 2)")]
    public int spawnCount = 6;

    [Tooltip("How far apart to spread trash items (world units)")]
    public float scatterRadius = 8f;

    [Tooltip("Minimum distance between two trash items")]
    public float minSpacing = 2.5f;

    [Tooltip("Keeps trash away from obstacle collisions")]
    public float obstacleBuffer = 2f;

    [Tooltip("Candidate positions tested per trash item")]
    public int placementAttempts = 12;

    void Start()
    {
        if (trashObjects == null || trashObjects.Length == 0)
            trashObjects = AutoFindTrashByName();

        if (trashObjects == null || trashObjects.Length == 0)
        {
            Debug.LogWarning("[TrashSpawner] No trash objects assigned!");
            return;
        }

        Transform playerTransform = FindPlayerTransform();
        List<Transform> obstacleAnchors = FindObstacleAnchors();
        List<Vector3> usedPositions = new List<Vector3>();
        List<GameObject> validOriginals = new List<GameObject>();

        foreach (GameObject trash in trashObjects)
        {
            if (trash != null)
                validOriginals.Add(trash);
        }

        if (validOriginals.Count == 0)
        {
            Debug.LogWarning("[TrashSpawner] Trash list exists but contains no valid objects.");
            return;
        }

        int toSpawn = spawnCount - validOriginals.Count;
        if (toSpawn < 0)
        {
            int excess = -toSpawn;
            for (int i = 0; i < excess; i++)
            {
                GameObject extra = validOriginals[validOriginals.Count - 1];
                validOriginals.RemoveAt(validOriginals.Count - 1);
                extra.SetActive(false);
            }

            toSpawn = 0;
        }

        foreach (GameObject trash in validOriginals)
        {
            EnsurePickupable(trash);
            TuneTrashPlacement(trash, usedPositions, obstacleAnchors, playerTransform, 1.5f);
        }

        for (int i = 0; i < toSpawn; i++)
        {
            GameObject template = validOriginals[Random.Range(0, validOriginals.Count)];
            Vector3 spawnPosition = FindBestTrashPosition(template.transform.position, usedPositions, obstacleAnchors, playerTransform, scatterRadius);

            GameObject clone = Instantiate(template, spawnPosition, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
            clone.name = template.name;
            clone.SetActive(true);

            EnsurePickupable(clone);
            usedPositions.Add(clone.transform.position);
        }

        Debug.Log("[TrashSpawner] " + validOriginals.Count + " originals + " + Mathf.Max(0, toSpawn) + " clones = " + spawnCount + " total trash items.");
    }

    void EnsurePickupable(GameObject go)
    {
        if (go.GetComponent<TrashItem>() == null)
            go.AddComponent<TrashItem>();

        if (go.GetComponentInChildren<Collider>() == null)
        {
            SphereCollider sphereCollider = go.AddComponent<SphereCollider>();
            sphereCollider.radius = 0.5f;
            sphereCollider.isTrigger = true;
        }
    }

    void TuneTrashPlacement(GameObject trash, List<Vector3> usedPositions, List<Transform> obstacleAnchors, Transform playerTransform, float maxOffset)
    {
        Vector3 tunedPosition = FindBestTrashPosition(trash.transform.position, usedPositions, obstacleAnchors, playerTransform, maxOffset);
        trash.transform.position = tunedPosition;
        usedPositions.Add(tunedPosition);
    }

    Vector3 FindBestTrashPosition(Vector3 origin, List<Vector3> usedPositions, List<Transform> obstacleAnchors, Transform playerTransform, float maxOffset)
    {
        Vector3 bestPosition = origin;
        float bestScore = ScorePosition(origin, usedPositions, obstacleAnchors, playerTransform);

        for (int i = 0; i < placementAttempts; i++)
        {
            Vector2 circle = Random.insideUnitCircle * maxOffset;
            Vector3 candidate = origin + new Vector3(circle.x, 0f, circle.y);
            float score = ScorePosition(candidate, usedPositions, obstacleAnchors, playerTransform);

            if (score > bestScore)
            {
                bestScore = score;
                bestPosition = candidate;
            }
        }

        return bestPosition;
    }

    float ScorePosition(Vector3 candidate, List<Vector3> usedPositions, List<Transform> obstacleAnchors, Transform playerTransform)
    {
        float nearestTrash = FindNearestTrashDistance(candidate, usedPositions);
        if (nearestTrash < minSpacing)
            return -1000f + nearestTrash;

        float nearestObstacle = FindNearestObstacleDistance(candidate, obstacleAnchors);
        if (nearestObstacle < obstacleBuffer)
            return -500f + nearestObstacle;

        float score = 0f;

        if (nearestTrash < float.MaxValue)
            score += Mathf.Min(nearestTrash, minSpacing * 2f);

        if (nearestObstacle < float.MaxValue)
            score += Mathf.Min(nearestObstacle, obstacleBuffer * 2f) * 1.5f;

        if (playerTransform != null)
        {
            float playerDistance = Vector3.Distance(candidate, playerTransform.position);
            if (playerDistance < 2f)
                score -= 5f;
            else
                score += Mathf.Min(playerDistance, scatterRadius * 1.5f) * 0.1f;
        }

        return score;
    }

    float FindNearestTrashDistance(Vector3 candidate, List<Vector3> usedPositions)
    {
        if (usedPositions.Count == 0)
            return float.MaxValue;

        float nearestDistance = float.MaxValue;
        for (int i = 0; i < usedPositions.Count; i++)
        {
            float distance = Vector3.Distance(candidate, usedPositions[i]);
            if (distance < nearestDistance)
                nearestDistance = distance;
        }

        return nearestDistance;
    }

    float FindNearestObstacleDistance(Vector3 candidate, List<Transform> obstacleAnchors)
    {
        if (obstacleAnchors.Count == 0)
            return float.MaxValue;

        float nearestDistance = float.MaxValue;
        for (int i = 0; i < obstacleAnchors.Count; i++)
        {
            if (obstacleAnchors[i] == null)
                continue;

            float distance = Vector3.Distance(candidate, obstacleAnchors[i].position);
            if (distance < nearestDistance)
                nearestDistance = distance;
        }

        return nearestDistance;
    }

    Transform FindPlayerTransform()
    {
        Mover mover = FindObjectOfType<Mover>();
        if (mover != null)
            return mover.rb != null ? mover.rb.transform : mover.transform;

        MoverLevelTwo moverLevelTwo = FindObjectOfType<MoverLevelTwo>();
        if (moverLevelTwo != null)
            return moverLevelTwo.rb != null ? moverLevelTwo.rb.transform : moverLevelTwo.transform;

        return null;
    }

    List<Transform> FindObstacleAnchors()
    {
        List<Transform> result = new List<Transform>();

        ObstacleCollision[] scriptedObstacles = FindObjectsOfType<ObstacleCollision>();
        foreach (ObstacleCollision obstacle in scriptedObstacles)
        {
            if (obstacle != null && obstacle.gameObject.activeInHierarchy)
                result.Add(obstacle.transform);
        }

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject current in allObjects)
        {
            if (!current.activeInHierarchy)
                continue;

            if (!LooksLikeObstacle(current.name))
                continue;

            if (current.GetComponentInParent<TrashItem>() != null)
                continue;

            if (!result.Contains(current.transform))
                result.Add(current.transform);
        }

        return result;
    }

    bool LooksLikeObstacle(string objectName)
    {
        string lowered = objectName.ToLowerInvariant();
        return lowered.Contains("bin") || lowered.Contains("barrier") || lowered.Contains("cone");
    }

    GameObject[] AutoFindTrashByName()
    {
        string[] keywords = { "cheeseburger", "sodacan", "fishbone", "applecore", "chickenbone", "beercan", "bottle", "wine", "vodka" };
        List<GameObject> result = new List<GameObject>();

        foreach (GameObject current in FindObjectsOfType<GameObject>())
        {
            if (!current.activeInHierarchy)
                continue;

            string objectName = current.name.ToLowerInvariant().Replace(" ", "");
            for (int i = 0; i < keywords.Length; i++)
            {
                if (!objectName.Contains(keywords[i]))
                    continue;

                result.Add(current);
                break;
            }
        }

        return result.ToArray();
    }
}

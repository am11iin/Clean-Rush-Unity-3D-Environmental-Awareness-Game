using UnityEngine;

/// <summary>
/// Makes obstacles (bins) move automatically to make the game harder!
/// </summary>
public class ObstaclePatrol : MonoBehaviour
{
    private Vector3 startPos;
    private float offset;
    private bool moveAxisX;

    void Start()
    {
        startPos = transform.position;
        offset = Random.Range(0f, 10f); // Make them all out of sync
        moveAxisX = Random.value > 0.5f; // Radomly move along X or Z axis
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.timeLeft <= 0) return;

        // Move back and forth in a 3 unit range
        float movement = Mathf.Sin(Time.time * 2f + offset) * 3f;

        if (moveAxisX)
            transform.position = startPos + new Vector3(movement, 0, 0);
        else
            transform.position = startPos + new Vector3(0, 0, movement);
    }
}

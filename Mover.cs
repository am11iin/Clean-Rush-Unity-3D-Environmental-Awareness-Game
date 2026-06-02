using System.Collections;
using UnityEngine;

/// <summary>
/// Level 1 player controller.
/// Handles WASD movement and QE rotation.
/// Auto-bootstraps GameManager and TrashSpawner so no Unity Editor setup is needed.
/// </summary>
public class Mover : MonoBehaviour
{
    [Tooltip("The player character root GameObject to move/rotate")]
    public GameObject rb;

    [Header("Movement Settings")]
    public float moveSpeed = 0.05f;
    public float rotateSpeed = 0.25f;

    Vector3 _impactVelocity;
    Coroutine _impactRoutine;
    Vector3 _baseScale = Vector3.one;

    void Awake()
    {
        if (FindObjectOfType<GameManager>() == null)
        {
            GameObject gameManagerObject = new GameObject("GameManager_Auto");
            GameManager gameManager = gameManagerObject.AddComponent<GameManager>();
            gameManager.timeLeft = 60f;
            gameManager.trashTarget = 6;
        }

        if (FindObjectOfType<TrashSpawner>() == null)
        {
            GameObject spawnerObject = new GameObject("TrashSpawner_Auto");
            TrashSpawner trashSpawner = spawnerObject.AddComponent<TrashSpawner>();
            trashSpawner.spawnCount = 6;
        }
    }

    void Start()
    {
        if (rb == null)
            return;

        if (rb.GetComponent<PlayerCollisionForwarder>() == null)
            rb.AddComponent<PlayerCollisionForwarder>();

        _baseScale = rb.transform.localScale;
    }

    void Update()
    {
        if (rb == null)
            return;

        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            currentSpeed *= 2f;

        if (Input.GetKey(KeyCode.W))
            rb.transform.Translate(0f, 0f, currentSpeed);
        if (Input.GetKey(KeyCode.S))
            rb.transform.Translate(0f, 0f, -currentSpeed);
        if (Input.GetKey(KeyCode.A))
            rb.transform.Translate(-currentSpeed, 0f, 0f);
        if (Input.GetKey(KeyCode.D))
            rb.transform.Translate(currentSpeed, 0f, 0f);

        if (Input.GetKey(KeyCode.Q))
            rb.transform.Rotate(0f, rotateSpeed, 0f);
        if (Input.GetKey(KeyCode.E))
            rb.transform.Rotate(0f, -rotateSpeed, 0f);

        ApplyImpactMotion();
    }

    public void ApplyObstacleImpact(Vector3 obstaclePosition)
    {
        if (rb == null)
            return;

        Vector3 away = rb.transform.position - obstaclePosition;
        away.y = 0f;

        if (away.sqrMagnitude < 0.001f)
            away = -rb.transform.forward;

        _impactVelocity = away.normalized * 0.18f;

        if (_impactRoutine != null)
            StopCoroutine(_impactRoutine);

        _impactRoutine = StartCoroutine(ImpactPulseRoutine());
    }

    void ApplyImpactMotion()
    {
        if (_impactVelocity.sqrMagnitude < 0.000001f)
            return;

        rb.transform.position += _impactVelocity;
        _impactVelocity = Vector3.Lerp(_impactVelocity, Vector3.zero, Time.deltaTime * 12f);
    }

    IEnumerator ImpactPulseRoutine()
    {
        float duration = 0.18f;

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float wave = Mathf.Sin(Mathf.Clamp01(t / duration) * Mathf.PI);
            rb.transform.localScale = new Vector3(
                _baseScale.x * (1f - wave * 0.06f),
                _baseScale.y * (1f - wave * 0.06f),
                _baseScale.z * (1f + wave * 0.12f));
            yield return null;
        }

        rb.transform.localScale = _baseScale;
        _impactRoutine = null;
    }
}

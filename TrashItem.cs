using System.Collections;
using UnityEngine;

/// <summary>
/// Attach to any trash root object. TrashSpawner adds this automatically.
/// Detects pickup by the player character and notifies GameManager.
/// Works even if the collider is on a child mesh via PlayerCollisionForwarder.
/// </summary>
public class TrashItem : MonoBehaviour
{
    [Tooltip("Points awarded when collected")]
    public int scoreValue = 1;

    [Header("Simple visibility feedback")]
    public float hoverHeight = 0.08f;
    public float hoverSpeed = 2.2f;
    public float spinSpeed = 28f;

    bool _collected;
    Vector3 _baseLocalPosition;
    Vector3 _baseLocalScale;
    Quaternion _baseLocalRotation;
    float _idleSeed;

    void Start()
    {
        _baseLocalPosition = transform.localPosition;
        _baseLocalScale = transform.localScale;
        _baseLocalRotation = transform.localRotation;
        _idleSeed = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        if (_collected)
            return;

        float wave = Mathf.Sin(Time.time * hoverSpeed + _idleSeed);
        transform.localPosition = _baseLocalPosition + Vector3.up * (wave * hoverHeight);
        transform.localScale = _baseLocalScale * (1f + wave * 0.04f);
        transform.localRotation = _baseLocalRotation * Quaternion.Euler(0f, (Time.time + _idleSeed) * spinSpeed, 0f);
    }

    public void ForceCollect()
    {
        if (_collected)
            return;

        Collect();
    }

    void OnTriggerEnter(Collider other)
    {
        if (_collected || !IsPlayer(other.gameObject))
            return;

        Collect();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_collected || !IsPlayer(collision.gameObject))
            return;

        Collect();
    }

    bool IsPlayer(GameObject go)
    {
        if (go.CompareTag("Player"))
            return true;

        if (go.GetComponentInParent<Mover>() != null)
            return true;

        if (go.GetComponentInParent<MoverLevelTwo>() != null)
            return true;

        return go.GetComponent<PlayerCollisionForwarder>() != null;
    }

    void Collect()
    {
        _collected = true;
        DisableColliders();

        Debug.Log("[TrashItem] Collected: " + gameObject.name);
        GameManager.Instance?.TrashCollected(scoreValue);

        StartCoroutine(CollectRoutine());
    }

    IEnumerator CollectRoutine()
    {
        Vector3 startPosition = transform.localPosition;
        Vector3 startScale = transform.localScale;
        Quaternion startRotation = transform.localRotation;
        float duration = 0.22f;

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalized = Mathf.Clamp01(t / duration);
            float scaleFactor = normalized < 0.35f
                ? Mathf.Lerp(1f, 1.18f, normalized / 0.35f)
                : Mathf.Lerp(1.18f, 0f, (normalized - 0.35f) / 0.65f);

            transform.localScale = startScale * scaleFactor;
            transform.localPosition = startPosition + Vector3.up * (0.42f * normalized);
            transform.localRotation = startRotation * Quaternion.Euler(0f, 220f * normalized, 0f);
            yield return null;
        }

        Destroy(gameObject);
    }

    void DisableColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider currentCollider in colliders)
            currentCollider.enabled = false;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Central singleton: score, countdown, level transitions, and lightweight feedback.
/// Auto-creates the gameplay UI at runtime if nothing is wired in the Inspector.
/// Place on one empty GameObject in each gameplay scene.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI (auto-created if left empty)")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text statusText;
    public TMP_Text messageText;

    [Header("Timer")]
    public float timeLeft = 60f;

    [Header("Level")]
    [Tooltip("6 for LevelOne, 12 for LevelTwo")]
    public int trashTarget = 6;

    [Header("Audio (optional)")]
    public AudioSource softMusic;
    public AudioSource timerTickSound;

    int _score;
    int _trashCollected;
    bool _levelComplete;
    bool _timerRunning = true;
    bool _endingSequenceStarted;
    Coroutine _statusRoutine;
    Coroutine _messageRoutine;
    Coroutine _scorePulseRoutine;
    Coroutine _timerPulseRoutine;
    AudioClip _pickupClip;
    AudioClip _obstacleClip;
    Color _scoreBaseColor = Color.white;
    Color _timerBaseColor = Color.white;
    Vector3 _scoreBaseScale = Vector3.one;
    Vector3 _timerBaseScale = Vector3.one;

    int TrashRemaining
    {
        get { return Mathf.Max(0, trashTarget - _trashCollected); }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        AutoCreateUI();
        AutoWireAudio();
        CacheUiState();
    }

    void Start()
    {
        RefreshScoreUI();
        RefreshTimerUI(Mathf.CeilToInt(timeLeft));
        ShowStatusMessage("Ramassez tous les dechets et evitez les obstacles.", new Color(0.86f, 0.96f, 1f), 2.25f);

        if (softMusic != null && !softMusic.isPlaying)
            softMusic.Play();
    }

    void Update()
    {
        if (_levelComplete || !_timerRunning)
            return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            _timerRunning = false;
            RefreshTimerUI(0);
            TimeUp();
            return;
        }

        RefreshTimerUI(Mathf.CeilToInt(timeLeft));
    }

    public void TrashCollected(int points = 1)
    {
        if (_endingSequenceStarted)
            return;

        _score += Mathf.Max(0, points);
        _trashCollected++;

        RefreshScoreUI();
        PulseScore(new Color(0.72f, 1f, 0.72f), 1.1f);
        PlayFeedbackClip(_pickupClip, 0.18f);

        Debug.Log("[GM] Trash collected! " + _trashCollected + "/" + trashTarget + " score=" + _score);

        if (_trashCollected >= trashTarget)
        {
            LevelComplete();
            return;
        }

        if (TrashRemaining == 1)
            ShowStatusMessage("Plus qu'un dechet !", new Color(0.78f, 1f, 0.82f), 1.1f);
        else
            ShowStatusMessage(TrashRemaining + " dechets restants.", new Color(0.78f, 1f, 0.82f), 1.1f);
    }

    public void ApplyPenalty()
    {
        if (_endingSequenceStarted)
            return;

        _score = Mathf.Max(0, _score - 1);
        timeLeft = Mathf.Max(0f, timeLeft - 5f);

        RefreshScoreUI();
        RefreshTimerUI(Mathf.CeilToInt(timeLeft));
        PulseScore(new Color(1f, 0.6f, 0.6f), 1.08f);
        PulseTimer(1.08f);
        PlayFeedbackClip(_obstacleClip, 0.32f);
        ShowStatusMessage("Obstacle ! -1 score, -5 s.", new Color(1f, 0.74f, 0.52f), 1f);

        if (timeLeft <= 0f)
        {
            _timerRunning = false;
            TimeUp();
        }
    }

    public void AddTimeBonus(float bonus)
    {
        if (_endingSequenceStarted)
            return;

        timeLeft += bonus;
        RefreshTimerUI(Mathf.CeilToInt(timeLeft));
        ShowStatusMessage("Bonus de temps : +" + bonus.ToString("0") + " s.", new Color(0.74f, 0.9f, 1f), 1f);
        Debug.Log("[GameManager] Bonus Time: +" + bonus + "s");
    }

    public void TimeUp()
    {
        if (_endingSequenceStarted || _levelComplete)
            return;

        _endingSequenceStarted = true;
        _timerRunning = false;

        if (timerTickSound != null && timerTickSound.isPlaying)
            timerTickSound.Stop();

        ShowEndMessage("Temps ecoule !", new Color(1f, 0.42f, 0.42f));
        ShowStatusMessage("Nouvel essai...", new Color(1f, 0.82f, 0.82f), 1.2f);
        StartCoroutine(RestartAfterDelay(2.5f));
    }

    void LevelComplete()
    {
        if (_endingSequenceStarted)
            return;

        _endingSequenceStarted = true;
        _levelComplete = true;
        _timerRunning = false;

        if (timerTickSound != null && timerTickSound.isPlaying)
            timerTickSound.Stop();

        StartCoroutine(LevelCompleteSequence());
    }

    IEnumerator LevelCompleteSequence()
    {
        ShowEndMessage("Niveau termine !", new Color(0.6f, 1f, 0.68f));
        ShowStatusMessage("Tous les dechets sont ramasses.", new Color(0.8f, 1f, 0.84f), 1.1f);
        PlayFeedbackClip(_pickupClip, 0.24f);

        yield return new WaitForSeconds(1.25f);

        bool hasNextScene = HasNextScene();
        ShowStatusMessage(hasNextScene ? "Niveau suivant..." : "Retour au menu...", Color.white, 1.1f);

        yield return new WaitForSeconds(1.2f);

        LoadNextScene();
    }

    IEnumerator RestartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void LoadNextScene()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(next < SceneManager.sceneCountInBuildSettings ? next : 0);
    }

    bool HasNextScene()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        return next < SceneManager.sceneCountInBuildSettings;
    }

    void RefreshScoreUI()
    {
        if (scoreText == null)
            return;

        scoreText.text = "Collectes: " + _trashCollected + " / " + trashTarget
                       + "\nRestants: " + TrashRemaining
                       + "\nScore: " + _score;
    }

    void RefreshTimerUI(int seconds)
    {
        if (timerText == null)
            return;

        int clampedSeconds = Mathf.Max(0, seconds);
        int minutes = clampedSeconds / 60;
        int remainingSeconds = clampedSeconds % 60;

        timerText.text = "Temps: " + minutes.ToString("00") + ":" + remainingSeconds.ToString("00");
        ApplyTimerStyle();
    }

    void ShowStatusMessage(string text, Color color, float duration)
    {
        if (statusText == null)
            return;

        if (_statusRoutine != null)
            StopCoroutine(_statusRoutine);

        _statusRoutine = StartCoroutine(StatusMessageRoutine(text, color, duration));
    }

    IEnumerator StatusMessageRoutine(string text, Color color, float duration)
    {
        statusText.gameObject.SetActive(true);
        statusText.text = text;
        statusText.transform.localScale = Vector3.one * 0.92f;

        float fadeIn = 0.12f;
        float fadeOut = 0.16f;
        float hold = Mathf.Max(0f, duration - fadeIn - fadeOut);

        for (float t = 0f; t < fadeIn; t += Time.unscaledDeltaTime)
        {
            float normalized = Mathf.Clamp01(t / fadeIn);
            Color c = color;
            c.a = normalized;
            statusText.color = c;
            statusText.transform.localScale = Vector3.one * Mathf.Lerp(0.92f, 1f, normalized);
            yield return null;
        }

        statusText.color = color;
        statusText.transform.localScale = Vector3.one;

        if (hold > 0f)
            yield return new WaitForSecondsRealtime(hold);

        for (float t = 0f; t < fadeOut; t += Time.unscaledDeltaTime)
        {
            float normalized = Mathf.Clamp01(t / fadeOut);
            Color c = color;
            c.a = 1f - normalized;
            statusText.color = c;
            yield return null;
        }

        statusText.gameObject.SetActive(false);
        _statusRoutine = null;
    }

    void ShowEndMessage(string text, Color color)
    {
        if (messageText == null)
            return;

        if (_messageRoutine != null)
            StopCoroutine(_messageRoutine);

        messageText.gameObject.SetActive(true);
        messageText.text = text;
        messageText.color = color;
        _messageRoutine = StartCoroutine(AnimateMessage());
    }

    IEnumerator AnimateMessage()
    {
        float t = 0f;
        Vector3 startScale = Vector3.one * 0.72f;
        Vector3 endScale = Vector3.one;
        messageText.transform.localScale = startScale;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 3f;
            float eased = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f);
            messageText.transform.localScale = Vector3.LerpUnclamped(startScale, endScale, eased);
            yield return null;
        }

        messageText.transform.localScale = endScale;
        _messageRoutine = null;
    }

    void PulseScore(Color flashColor, float peakScale)
    {
        if (scoreText == null)
            return;

        if (_scorePulseRoutine != null)
            StopCoroutine(_scorePulseRoutine);

        _scorePulseRoutine = StartCoroutine(ScorePulseRoutine(flashColor, peakScale));
    }

    IEnumerator ScorePulseRoutine(Color flashColor, float peakScale)
    {
        float duration = 0.18f;
        scoreText.color = flashColor;

        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            float normalized = Mathf.Clamp01(t / duration);
            float scale = normalized < 0.5f
                ? Mathf.Lerp(1f, peakScale, normalized / 0.5f)
                : Mathf.Lerp(peakScale, 1f, (normalized - 0.5f) / 0.5f);

            scoreText.transform.localScale = _scoreBaseScale * scale;
            yield return null;
        }

        scoreText.transform.localScale = _scoreBaseScale;
        scoreText.color = _scoreBaseColor;
        _scorePulseRoutine = null;
    }

    void PulseTimer(float peakScale)
    {
        if (timerText == null)
            return;

        if (_timerPulseRoutine != null)
            StopCoroutine(_timerPulseRoutine);

        _timerPulseRoutine = StartCoroutine(TimerPulseRoutine(peakScale));
    }

    IEnumerator TimerPulseRoutine(float peakScale)
    {
        float duration = 0.18f;

        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            float normalized = Mathf.Clamp01(t / duration);
            float scale = normalized < 0.5f
                ? Mathf.Lerp(1f, peakScale, normalized / 0.5f)
                : Mathf.Lerp(peakScale, 1f, (normalized - 0.5f) / 0.5f);

            timerText.transform.localScale = _timerBaseScale * scale;
            yield return null;
        }

        timerText.transform.localScale = _timerBaseScale;
        ApplyTimerStyle();
        _timerPulseRoutine = null;
    }

    void ApplyTimerStyle()
    {
        if (timerText == null)
            return;

        timerText.color = timeLeft <= 15f && !_levelComplete
            ? new Color(1f, 0.8f, 0.34f)
            : _timerBaseColor;
    }

    void CacheUiState()
    {
        if (scoreText != null)
        {
            _scoreBaseColor = scoreText.color;
            _scoreBaseScale = scoreText.transform.localScale;
        }

        if (timerText != null)
        {
            _timerBaseColor = timerText.color;
            _timerBaseScale = timerText.transform.localScale;
        }
    }

    void AutoWireAudio()
    {
        if (softMusic == null)
            softMusic = FindAudioSource("soft", "background_soft");

        if (timerTickSound == null)
            timerTickSound = FindAudioSource("countdown", "tick");

        AudioSource dangerAudio = FindAudioSource("danger", "suspend");

        if (dangerAudio != null && dangerAudio.clip != null)
        {
            _pickupClip = dangerAudio.clip;
            _obstacleClip = dangerAudio.clip;
            return;
        }

        if (timerTickSound != null && timerTickSound.clip != null)
        {
            _pickupClip = timerTickSound.clip;
            _obstacleClip = timerTickSound.clip;
        }
    }

    AudioSource FindAudioSource(params string[] keywords)
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in sources)
        {
            if (source == null || source.gameObject == null)
                continue;

            string objectName = source.gameObject.name.ToLowerInvariant();
            for (int i = 0; i < keywords.Length; i++)
            {
                if (objectName.Contains(keywords[i].ToLowerInvariant()))
                    return source;
            }
        }

        return null;
    }

    void PlayFeedbackClip(AudioClip clip, float volumeScale)
    {
        if (clip == null)
            return;

        Vector3 position = transform.position;
        if (Camera.main != null)
            position = Camera.main.transform.position;

        AudioSource.PlayClipAtPoint(clip, position, volumeScale);
    }

    void AutoCreateUI()
    {
        foreach (TMP_Text text in FindObjectsOfType<TextMeshProUGUI>())
        {
            if (text.text.Trim() == "New Text")
                Destroy(text.gameObject);
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("GameCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        if (scoreText == null)
        {
            Image scorePanel = MakePanel(canvas.transform, "ScorePanel_Auto",
                new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(18f, -18f), new Vector2(280f, 118f),
                new Color(0f, 0f, 0f, 0.38f));

            scoreText = MakeLabel(scorePanel.transform, "ScoreText_Auto",
                new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(16f, -16f), new Vector2(248f, 88f),
                24f, TextAlignmentOptions.TopLeft, true);
        }

        if (timerText == null)
        {
            Image timerPanel = MakePanel(canvas.transform, "TimerPanel_Auto",
                new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-18f, -18f), new Vector2(190f, 64f),
                new Color(0f, 0f, 0f, 0.38f));

            timerText = MakeLabel(timerPanel.transform, "TimerText_Auto",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(160f, 42f),
                25f, TextAlignmentOptions.Center, false);
        }

        if (statusText == null)
        {
            statusText = MakeLabel(canvas.transform, "StatusText_Auto",
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -26f), new Vector2(720f, 60f),
                28f, TextAlignmentOptions.Center, true);
        }
        statusText.gameObject.SetActive(false);

        if (messageText == null)
        {
            messageText = MakeLabel(canvas.transform, "MessageText_Auto",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, 24f), new Vector2(760f, 170f),
                64f, TextAlignmentOptions.Center, true);
        }
        messageText.gameObject.SetActive(false);
    }

    Image MakePanel(Transform parent, string name,
                    Vector2 anchorMin, Vector2 anchorMax,
                    Vector2 position, Vector2 size, Color color)
    {
        GameObject panelObject = new GameObject(name);
        panelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = panelObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = anchorMin;
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;

        Image image = panelObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;

        return image;
    }

    TMP_Text MakeLabel(Transform parent, string name,
                       Vector2 anchorMin, Vector2 anchorMax,
                       Vector2 position, Vector2 size, float fontSize,
                       TextAlignmentOptions alignment, bool wrap)
    {
        GameObject labelObject = new GameObject(name);
        labelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = labelObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = anchorMin;
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;

        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        label.fontSize = fontSize;
        label.fontStyle = FontStyles.Bold;
        label.alignment = alignment;
        label.color = Color.white;
        label.outlineWidth = 0.25f;
        label.outlineColor = new Color(0f, 0f, 0f, 0.92f);
        label.enableWordWrapping = wrap;
        label.raycastTarget = false;

        return label;
    }
}

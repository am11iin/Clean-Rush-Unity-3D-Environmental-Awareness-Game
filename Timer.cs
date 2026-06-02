using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Countdown timer with audio feedback.
/// The UI text and audio are driven here; the GameManager owns the
/// actual countdown so this script is kept for audio/legacy SFX only.
/// On time-up it delegates to GameManager.TimeUp() for the level restart.
/// </summary>
public class Timer : MonoBehaviour
{
    public TMP_Text textDisplay;
    public int timeLeft = 30;
    public bool timerOn = false;
    public AudioSource timerSound;
    public AudioSource softMusic;

    void Start()
    {
        // Hide the legacy text label so GameManager's UI is the only one visible
        if (textDisplay != null)
            textDisplay.gameObject.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        float current = GameManager.Instance.timeLeft;

        // Audio cues when time is running out (< 15 seconds)
        if (current < 15f && current > 0f)
        {
            if (!timerSound.isPlaying)
                timerSound.Play();
                
            timerSound.volume = Mathf.Lerp(timerSound.volume, 1.0f, 0.25f);
            
            if (softMusic != null)
                softMusic.volume = Mathf.Lerp(softMusic.volume, 0.0f, 0.25f);
        }
        else
        {
            timerSound.Stop();
        }
    }
}

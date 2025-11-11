using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelStarter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup uiElement;   // The UI element to fade out
    [SerializeField] private string startingSong;     // The music track name (matches keys in SFXManager)
    [SerializeField] private float fadeDuration = 3f; // Seconds for fade and volume ramp
    [SerializeField] private float targetMusicVolume = 1f; // Final music volume

    [Header("Options")]
    [SerializeField] private bool startOnAwake = true;

    private void Start()
    {
        if (startOnAwake)
        {
            StartFadeAndMusic();
        }
    }

    public void StartFadeAndMusic()
    {
        if (uiElement == null)
        {
            Debug.LogWarning("UIFadeAndMusicRamp: Missing UI element reference.");
            return;
        }

        if (SFXManager.Instance == null)
        {
            Debug.LogWarning("UIFadeAndMusicRamp: No SFXManager found in scene.");
            return;
        }

        StartCoroutine(FadeOutAndIncreaseMusic());
    }

    private IEnumerator FadeOutAndIncreaseMusic()
    {
        // Load and prepare the selected music track
        if (!string.IsNullOrEmpty(startingSong))
        {
            SFXManager.Instance.LoadMusic(startingSong);
        }

        // Ensure the music source starts from low volume
        SFXManager.Instance.SetMusicVolume(0f);
        SFXManager.Instance.PlayMusic();

        float startAlpha = uiElement.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // Fade out UI alpha
            uiElement.alpha = Mathf.Lerp(startAlpha, 0f, t);

            // Fade in music volume
            SFXManager.Instance.SetMusicVolume(Mathf.Lerp(0f, targetMusicVolume, t));

            yield return null;
        }

        // Clamp to final values
        uiElement.alpha = 0f;
        SFXManager.Instance.SetMusicVolume(targetMusicVolume);
    }
}

using UnityEngine;
using System.Collections;

public class CanvasFader : MonoBehaviour
{
    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup canvasToFadeOut;
    [SerializeField] private CanvasGroup canvasToFadeIn;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private bool deactivateOldCanvas = false;

    private void Awake()
    {
        // Make sure the "incoming" canvas can't be clicked before it's visible
        if (canvasToFadeIn != null)
        {
            canvasToFadeIn.alpha = 0f;
            canvasToFadeIn.interactable = false;
            canvasToFadeIn.blocksRaycasts = false;
        }
    }

    public void StartFade()
    {
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        if (canvasToFadeOut != null)
            yield return FadeCanvas(canvasToFadeOut, 1f, 0f);

        if (deactivateOldCanvas && canvasToFadeOut != null)
            canvasToFadeOut.gameObject.SetActive(false);

        if (canvasToFadeIn != null)
            yield return FadeCanvas(canvasToFadeIn, 0f, 1f);
    }

    private IEnumerator FadeCanvas(CanvasGroup canvas, float start, float end)
    {
        float t = 0f;
        canvas.alpha = start;

        // Interaction settings during fade
        bool willBeInteractable = end > 0.9f;

        canvas.interactable = willBeInteractable;
        canvas.blocksRaycasts = willBeInteractable;

        // If we’re fading in, ensure the object is active
        if (!canvas.gameObject.activeSelf)
            canvas.gameObject.SetActive(true);

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Lerp(start, end, t / fadeDuration);
            canvas.alpha = lerp;
            yield return null;
        }

        // Snap to final
        canvas.alpha = end;

        // Set interaction rules once finished
        canvas.interactable = willBeInteractable;
        canvas.blocksRaycasts = willBeInteractable;
    }
}

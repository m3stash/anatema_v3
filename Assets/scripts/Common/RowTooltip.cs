using System.Collections;
using TMPro;
using UnityEngine;

public class RowTooltip : MonoBehaviour {

    [SerializeField] private GameObject textGO;

    private TextMeshProUGUI text;
    private float displayDuration = 3f;
    private float fadeInDuration = 0.5f;
    private float fadeOutDuration = 0.5f;
    private CanvasGroup canvasGroup;
    public void Setup(string message) {
        if (textGO == null) {
            Debug.LogError("RowTooltip: textGO is null");
            return;
        }
        text = textGO.GetComponent<TextMeshProUGUI>();
        text.text = message;
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(DisplayTooltip());
    }

    IEnumerator DisplayTooltip() {
        // Fade in when appearing
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration) {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Wait for display time
        yield return new WaitForSeconds(displayDuration);

        // Fade in and out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration) {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;

        // Destroy the tooltip after the fade by disappearing
        Destroy(gameObject);
    }
}

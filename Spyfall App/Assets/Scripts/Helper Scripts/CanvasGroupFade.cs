using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFade : MonoBehaviour {
    [SerializeField, Range(0f, 1f)] private float fadeSpeed = 0.1f;
    public delegate void AfterEnumerator();

    public enum Fade {
        fadeIn,
        fadeOut
    }

    public void DoFade(Fade fadeType, AfterEnumerator callAfter = null) {
        StartCoroutine(GroupFade(fadeType == Fade.fadeIn, callAfter));
    }

    private IEnumerator GroupFade(bool fadeIn, AfterEnumerator callAfter = null) {
        float alpha = fadeIn ? 0 : 1;
        while (fadeIn ? alpha < 1 : alpha > 0) {
            alpha = Mathf.Clamp(fadeIn ? alpha + fadeSpeed : alpha - fadeSpeed, 0, 1);
            GetComponent<CanvasGroup>().alpha = alpha;
            yield return 0;
        }
        callAfter?.Invoke();
    }
}

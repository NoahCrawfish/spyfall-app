using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFade : MonoBehaviour {
    [SerializeField, Range(0f, 1f)] private float fadeSpeed = 0.1f;
    public float Alpha { get; private set; }
    public delegate void AfterEnumerator();

    public enum Fade {
        fadeIn,
        fadeOut
    }

    public void DoFade(Fade fadeType, AfterEnumerator callAfter = null) {
        StartCoroutine(GroupFade(fadeType == Fade.fadeIn, callAfter));
    }

    private IEnumerator GroupFade(bool fadeIn, AfterEnumerator callAfter = null) {
        Alpha = fadeIn ? 0 : 1;
        while (fadeIn ? Alpha < 1 : Alpha > 0) {
            Alpha = Mathf.Clamp(fadeIn ? Alpha + fadeSpeed : Alpha - fadeSpeed, 0, 1);
            GetComponent<CanvasGroup>().alpha = Alpha;
            yield return 0;
        }
        callAfter?.Invoke();
    }
}

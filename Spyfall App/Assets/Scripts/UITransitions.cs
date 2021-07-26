using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UITransitions : MonoBehaviour
{
    private const float crossFadeSpeed = 0.25f;
    private const float dipToColorDissolveSpeed = 0.175f;
    private const float defaultDipToColorWaitTime = 0f;
    [SerializeField] private GameObject clickBlocker; // panel with a canvas group that blocks raycasts; must be the lowest of sibling panels

    private enum TransitionModes {
        CrossFade,
        DipToColorDissolve
    }

    public void CrossFadeBetweenPanels(CanvasGroup startPanel, CanvasGroup finishPanel, float speed = crossFadeSpeed, Color? bg = null) {
        GetComponent<Image>().color = bg ?? Color.black;
        clickBlocker.SetActive(true);
        Task fadeOutPanel = new Task(FadeOutPanel(startPanel, speed, TransitionModes.CrossFade));
        Task fadeInPanel = new Task(FadeInPanel(finishPanel, speed, TransitionModes.CrossFade));
        StartCoroutine(DeactivateClickBlocker(fadeOutPanel, fadeInPanel));
    }

    public void DipToColorDissolveBetweenPanels(CanvasGroup startPanel, CanvasGroup finishPanel, float speed = dipToColorDissolveSpeed, float dipToColorWaitTime = defaultDipToColorWaitTime, Color? bg = null) {
        GetComponent<Image>().color = bg ?? Color.black;
        clickBlocker.SetActive(true);
        StartCoroutine(FadeOutPanel(startPanel, speed, TransitionModes.DipToColorDissolve, finishPanel, dipToColorWaitTime));
    }

    private IEnumerator FadeOutPanel(CanvasGroup panel, float fadeTime, TransitionModes mode, CanvasGroup finishPanel = null, float dipToColorWaitTime = defaultDipToColorWaitTime) {
        float t = 0f;
        panel.alpha = 1;

        while (t < 1) {
            yield return new WaitForFixedUpdate();
            t += Time.fixedDeltaTime / fadeTime;
            t = Mathf.Clamp(t, float.MinValue, 1);
            panel.alpha = Mathf.Lerp(1, 0, t);
        }

        panel.gameObject.SetActive(false);

        if (mode == TransitionModes.DipToColorDissolve) {
            yield return new WaitForSeconds(dipToColorWaitTime);
            StartCoroutine(FadeInPanel(finishPanel, fadeTime, mode));
        }
    }

    private IEnumerator FadeInPanel(CanvasGroup panel, float fadeTime, TransitionModes mode) {
        float t = 0f;
        panel.alpha = 0;
        panel.gameObject.SetActive(true);

        while (t < 1) {
            yield return new WaitForFixedUpdate();
            t += Time.fixedDeltaTime / fadeTime;
            t = Mathf.Clamp(t, float.MinValue, 1);
            panel.alpha = Mathf.Lerp(0, 1, t);
        }

        if (mode == TransitionModes.DipToColorDissolve) {
            clickBlocker.SetActive(false);
        }
    }

    private IEnumerator DeactivateClickBlocker(Task fadeOutPanel, Task fadeInPanel) {
        while (true) {
            yield return 0;
            if (!fadeOutPanel.Running && !fadeInPanel.Running) {
                clickBlocker.SetActive(false);
                yield break;
            }
        }
    }
}

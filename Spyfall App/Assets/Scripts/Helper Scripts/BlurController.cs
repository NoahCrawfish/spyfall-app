using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BlurController : MonoBehaviour
{
    [SerializeField] private float blurStartAmount = 0f;
    [SerializeField] private float blurFinishAmount = 3f;
    [SerializeField] private Color32 tintStart = new Color32(255, 255, 255, 255);
    [SerializeField] private Color32 tintFinish = new Color32(118, 118, 118, 255);
    [SerializeField, Range(0f, 1f)] private float blurFadeSpeed = 0.1f;

    public float Alpha { get; private set; }
    public delegate void AfterEnumerator();

    public enum Fade {
        fadeIn,
        fadeOut
    }

    public void DoBlur(Fade fadeType, AfterEnumerator callAfter = null) {
        StartCoroutine(BlurFade(fadeType == Fade.fadeIn, callAfter));
    }

    private IEnumerator BlurFade(bool fadeIn, AfterEnumerator callAfter = null) {
        Alpha = fadeIn ? 0 : 1;
        Material blur = GetComponent<Image>().material;
        while (fadeIn ? Alpha < 1 : Alpha > 0) {
            Alpha = Mathf.Clamp(fadeIn ? Alpha + blurFadeSpeed : Alpha - blurFadeSpeed, 0, 1);
            blur.SetFloat("_Size", Mathf.Lerp(blurStartAmount, blurFinishAmount, Alpha));
            blur.SetColor("_MultiplyColor", LerpColor(tintStart, tintFinish, Alpha));
            yield return null;
        }

        callAfter?.Invoke();
    }

    private Color LerpColor(Color start, Color finish, float t) {
        Vector4 startRGBA = new Vector4(start.r, start.g, start.b, start.a);
        Vector4 finishRGBA = new Vector4(finish.r, finish.g, finish.b, finish.a);
        Vector4 newColor = Vector4.Lerp(startRGBA, finishRGBA, t);
        return new Color(newColor[0], newColor[1], newColor[2], newColor[3]);
    }
}

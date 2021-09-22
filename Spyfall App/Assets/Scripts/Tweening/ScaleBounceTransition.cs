using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class ScaleBounceTransition : MonoBehaviour
{
    private RectTransform rect;
    private Vector2 startingSize;
    private LayoutElement layout;

    [SerializeField] private float duration = 0.4f;
    [SerializeField] private Ease ease = Ease.InOutBack;
    [SerializeField] private ScaleMode scaleMode;
    [SerializeField] private bool isSelectable = false;

    private enum ScaleMode {
        rectSize,
        layoutMinSize
    }

    private void Awake() {
        rect = GetComponent<RectTransform>();
        layout = GetComponent<LayoutElement>();
        startingSize = layout != null && scaleMode == ScaleMode.layoutMinSize ? new Vector2(layout.minWidth, layout.minHeight) : rect.sizeDelta;
    }

    private void OnEnable() {
        Tween();
    }

    private void Tween() {
        if (layout != null && scaleMode == ScaleMode.layoutMinSize) {
            layout.minWidth = 0f;
            layout.minHeight = 0f;

            if (!isSelectable) {
                layout.DOMinSize(startingSize, duration).SetEase(ease);
            } else {
                layout.DOMinSize(startingSize, duration).SetEase(ease)
                    .OnStart(() => GetComponent<Selectable>().interactable = false)
                    .OnComplete(() => GetComponent<Selectable>().interactable = true);
            }
        } else {
            rect.sizeDelta = Vector2.zero;

            if (!isSelectable) {
                rect.DOSizeDelta(startingSize, duration).SetEase(ease);
            } else {
                rect.DOSizeDelta(startingSize, duration).SetEase(ease)
                    .OnStart(() => GetComponent<Selectable>().interactable = false)
                    .OnComplete(() => GetComponent<Selectable>().interactable = true);
            }
        }
    }

    public void Reanimate() {
        StartCoroutine(DoReanimate());
    }

    private IEnumerator DoReanimate() {
        PauseAllTweens();
        rect.sizeDelta = Vector2.zero;

        enabled = false;
        yield return new WaitForEndOfFrame();
        enabled = true;
    }

    private void OnDestroy() {
        PauseAllTweens();
    }

    private void PauseAllTweens() {
        rect.DOPause();
        layout.DOPause();
    }
}

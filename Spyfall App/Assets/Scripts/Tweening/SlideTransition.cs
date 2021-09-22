using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class SlideTransition : MonoBehaviour
{
    private RectTransform rect;
    private Vector2 initialPos;
    private static readonly System.Random rand = new System.Random();

    [SerializeField] private float duration = 0.4f;
    [SerializeField] private Ease ease = Ease.InOutBack;
    [SerializeField] private EnterSide enterFrom;
    [SerializeField] private bool isSelectable = false;

    private enum EnterSide {
        left,
        right,
        top,
        bottom,
        randomize,
        randomHorz
    }

    private void Awake() {
        rect = GetComponent<RectTransform>();
        initialPos = rect.anchoredPosition;
    }

    private void OnEnable() {
        Tween();
    }

    private void Tween() {
        Vector2 fromPos = Vector2.zero;
        EnterSide enterSide = enterFrom;

        switch (enterFrom) {
            case EnterSide.randomize:
                enterSide = new List<EnterSide> { EnterSide.left, EnterSide.right, EnterSide.bottom, EnterSide.top }[rand.Next(4)];
                break;
            case EnterSide.randomHorz:
                enterSide = new List<EnterSide> { EnterSide.left, EnterSide.right }[rand.Next(2)];
                break;
        }

        switch (enterSide) {
            case EnterSide.left:
                fromPos = Vector2.up * initialPos + Vector2.left * (Screen.width + rect.sizeDelta.x) * 0.5f;
                break;
            case EnterSide.right:
                fromPos = Vector2.up * initialPos + Vector2.right * (Screen.width + rect.sizeDelta.x) * 0.5f;
                break;
            case EnterSide.top:
                fromPos = Vector2.right * initialPos + Vector2.up * (Screen.height + rect.sizeDelta.y) * 0.5f;
                break;
            case EnterSide.bottom:
                fromPos = Vector2.right * initialPos + Vector2.down * (Screen.height + rect.sizeDelta.y) * 0.5f;
                break;
        }
        rect.anchoredPosition = fromPos;

        if (!isSelectable) {
            rect.DOAnchorPos(initialPos, duration).SetEase(ease);
        } else {
            rect.DOAnchorPos(initialPos, duration).SetEase(ease)
                .OnStart(() => GetComponent<Selectable>().interactable = false)
                .OnComplete(() => GetComponent<Selectable>().interactable = true);
        }
    }

    private void OnDestroy() {
        rect.DOPause();
    }
}

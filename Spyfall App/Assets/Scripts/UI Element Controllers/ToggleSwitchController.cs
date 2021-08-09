using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.UI.ProceduralImage;

public class ToggleSwitchController : MonoBehaviour
{
    [SerializeField] RectTransform handleRect;
    [SerializeField] ProceduralImage bg;
    [SerializeField] Color handleDefaultColor;
    [SerializeField] Color handleActiveColor;

    private Toggle toggle;
    private Vector2 handleStartingPos;
    private float startingBorderWidth;

    private void Awake() {
        toggle = GetComponent<Toggle>();
        handleStartingPos = handleRect.anchoredPosition;
        startingBorderWidth = bg.BorderWidth;

        toggle.onValueChanged.AddListener(OnSwitch);
        if (toggle.isOn) {
            OnSwitch(true);
        }
    }

    private void OnSwitch(bool on) {
        handleRect.DOAnchorMin(on ? new Vector2(1f, 0.5f) : new Vector2(0f, 0.5f), .4f).SetEase(Ease.InOutBack);
        handleRect.DOAnchorMax(on ? new Vector2(1f, 0.5f) : new Vector2(0f, 0.5f), .4f).SetEase(Ease.InOutBack);
        handleRect.DOAnchorPos(on ? Vector2.zero : handleStartingPos, .4f).SetEase(Ease.InOutBack);
        DOTween.To(()=> bg.BorderWidth, x=> bg.BorderWidth = x, on ? handleRect.rect.height / 2f : startingBorderWidth, .6f);
        transform.GetChild(0).GetComponent<ProceduralImage>().DOColor(on ? handleActiveColor : handleDefaultColor, .4f);
    }

    private void OnDestroy() {
        toggle.onValueChanged.RemoveAllListeners();
    }
}

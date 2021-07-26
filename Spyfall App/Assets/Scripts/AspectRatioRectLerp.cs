using UnityEngine;

public class AspectRatioRectLerp : MonoBehaviour
{
    [SerializeField] private Vector3 positionTall;
    [SerializeField] private Vector2 sizeTall;
    [SerializeField] private Vector3 positionWide;
    [SerializeField] private Vector2 sizeWide;
    [SerializeField] private Vector2 tallestAspect = new Vector2(1125, 2436);
    [SerializeField] private Vector2 widestAspect = new Vector2(3, 4);

    private Camera mainCamera;
    private RectTransform rectTransform;

    private void Awake() {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable() {
        PositionElement();
    }

    public void PositionElement() {
        float tallestRatio = tallestAspect.x / tallestAspect.y;
        float widestRatio = widestAspect.x / widestAspect.y;
        float currentRatio = Mathf.Clamp(mainCamera.aspect, tallestRatio, widestRatio);
        float t = (currentRatio - tallestRatio) / (widestRatio - tallestRatio);

        rectTransform.anchoredPosition = Vector3.Lerp(positionTall, positionWide, t);
        rectTransform.sizeDelta = Vector2.Lerp(sizeTall, sizeWide, t);
    }
}
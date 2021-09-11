using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.ProceduralImage;
using UnityEngine.UI;

public class MoveButtonPress : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerDownHandler, IPointerUpHandler {
    RectTransform rectTransform;
    RectTransform shadowRect;
    ProceduralImage rendererImage;
    Vector2 startingOffsetMin;
    Vector2 startingOffsetMax;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color pressedColor;
    [SerializeField] private bool holdDownWhenSelected;

    private Selectable selectable;

    private void Awake() {
        rectTransform = transform.Find("Renderer").GetComponent<RectTransform>();
        shadowRect = transform.Find("Shadow").GetComponent<RectTransform>();
        rendererImage = transform.Find("Renderer").GetComponent<ProceduralImage>();
        selectable = GetComponent<Selectable>();
    }

    private void Start() {
        startingOffsetMin = new Vector2(rectTransform.offsetMin.x, rectTransform.offsetMin.y);
        startingOffsetMax = new Vector2(rectTransform.offsetMax.x, rectTransform.offsetMax.y);
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (!holdDownWhenSelected && selectable.interactable) {
            Down();
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (!holdDownWhenSelected && selectable.interactable) {
            Up();
        }
    }

    public void OnSelect(BaseEventData eventData) {
        if (holdDownWhenSelected && selectable.interactable) {
            Down();
        }
    }

    public void OnDeselect(BaseEventData eventData) {
        if (holdDownWhenSelected && selectable.interactable) {
            Up();
        }
    }

    private void Down() {
        rendererImage.color = pressedColor;
        rectTransform.offsetMin = new Vector2(shadowRect.offsetMin.x, shadowRect.offsetMin.y);
        rectTransform.offsetMax = new Vector2(shadowRect.offsetMax.x, shadowRect.offsetMax.y);
    }

    private void Up() {
        rendererImage.color = defaultColor;
        rectTransform.offsetMin = startingOffsetMin;
        rectTransform.offsetMax = startingOffsetMax;
    }
}

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ResetScroll : MonoBehaviour
{
    private ScrollRect scrollRect;

    private void Awake() {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void OnEnable() {
        scrollRect.verticalNormalizedPosition = 1;
    }
}

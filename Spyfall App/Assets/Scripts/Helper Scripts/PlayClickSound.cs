using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayClickSound : MonoBehaviour, IPointerUpHandler {
    private bool wasClicked;
    private Toggle toggle;

    private void Awake() {
        toggle = GetComponent<Toggle>();
        if (toggle != null) {
            toggle.onValueChanged.AddListener(OnSwitch);
        }
    }

    private void OnSwitch(bool on) {
        if (wasClicked) {
            ManageAudio.Instance.PlayVariedPitch("click", 0.15f);
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        StartCoroutine(WasClicked());
    }

    private IEnumerator WasClicked() {
        wasClicked = true;
        yield return new WaitForEndOfFrame();
        wasClicked = false;
    }


    // called from inspector in palette buttons
    public void PlayClick() {
        ManageAudio.Instance.PlayVariedPitch("click", 0.15f);
    }
}
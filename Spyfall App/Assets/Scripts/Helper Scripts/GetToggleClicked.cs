using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GetToggleClicked : MonoBehaviour, IPointerClickHandler
{
    private Toggle toggle;

    private void Awake() {
        toggle = GetComponent<Toggle>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        StartCoroutine(SendToggleChange());
    }

    private IEnumerator SendToggleChange() {
        // wait until end of frame for toggle isOn status to update
        yield return new WaitForEndOfFrame();

        switch (name) {
            case "SetToggle":
                GameObject set = transform.parent.parent.parent.gameObject;
                set.GetComponent<LocationSetController>().OnSetToggle(toggle);
                break;

            case "LocationToggle":
                GameObject location = transform.parent.parent.gameObject;
                set = transform.parent.parent.parent.parent.gameObject;
                int index = int.Parse(location.name.Split('_')[1]);
                set.GetComponent<LocationSetController>().LocationToggles[index].OnToggleChanged();
                break;
        }
    }
}

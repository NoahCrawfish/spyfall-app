using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocationSetController : MonoBehaviour {
    [SerializeField] private GameObject locationTogglePrefab;
    [SerializeField] private Toggle setToggle;

    private bool expanded = false;
    public bool Expanded {
        get => expanded;
        set {
            if (expanded != value) {
                ExpandedChanged(value);
            }
            expanded = value;
        }
    }
    public LocationSet ThisSet { get; set; }
    public List<bool> ToggleStates { get; private set; } = new List<bool>();
    public List<LocationToggle> LocationToggles { get; private set; } = new List<LocationToggle>();

    private const float autoScrollSpeed = 0.0015f;
    private RectTransform rectTransform;
    private Task currentAutoscroll;
   

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable() {
        StartCoroutine(Initialize());
    }

    private void OnDisable() {
        DestroyChildren();
    }

    private IEnumerator Initialize() {
        yield return new WaitForEndOfFrame();
        LoadToggleStates();
        RefreshSetToggle();
        RefreshLocationToggles();
    }

    private void DestroyChildren() {
        foreach (Transform child in transform.Find("Children")) {
            Destroy(child.gameObject);
        }
    }

    private void LoadToggleStates() {
        ToggleStates.Clear();
        foreach (var location in ThisSet.Locations) {
            ToggleStates.Add(location.enabled);
        }
    }

    private void SetAllToggleStatesTo(bool value) {
        int statesLength = ToggleStates.Count;
        ToggleStates.Clear();
        for (int i = 0; i < statesLength; i++) { 
            ToggleStates.Add(value);
        }
    }

    private void ExpandedChanged(bool isExpanded) {
        if (isExpanded) {
            int i = 0;
            LocationToggles.Clear();
            foreach (var location in ThisSet.Locations) {
                // create locationToggle gameObjects
                GameObject locationToggle = Instantiate(locationTogglePrefab);
                locationToggle.transform.SetParent(transform.Find("Children"), false);
                locationToggle.transform.Find("LocationButton/Renderer").GetChild(0).GetComponent<TextMeshProUGUI>().text = location.name;
                locationToggle.name = $"Location_{i}";

                // create locationToggle class to handle visualization of ToggleStates
                Toggle toggle = locationToggle.transform.Find("ToggleFrame/LocationToggle").GetComponent<Toggle>();
                LocationToggle toggleClass = new LocationToggle(this, toggle, i);
                toggleClass.RefreshToggle(true);
                LocationToggles.Add(toggleClass);

                i++;
            }

            GameObject scrollList = transform.root.Find("SettingsScreen/Frame/ScrollList").gameObject;
            ScrollRect scrollRect = scrollList.GetComponent<ScrollRect>();
            RectTransform scrollRectTransform = scrollList.GetComponent<RectTransform>();
            currentAutoscroll = new Task(ScrollToShowSet(scrollRect, scrollRectTransform, autoScrollSpeed));
        } else {
            currentAutoscroll?.Stop();
            DestroyChildren();
        }
    }

    private IEnumerator ScrollToShowSet(ScrollRect scrollRect, RectTransform scrollRectTransform, float scrollSpeed) {
        yield return new WaitForEndOfFrame();
        // previousScrollPos is used to test if the player has scrolled, canceling the auto scroll
        float previousScrollPos = scrollRect.verticalNormalizedPosition;

        Vector3[] rectCorners = new Vector3[4];
        Vector3[] scrollCorners = new Vector3[4];
        rectTransform.GetWorldCorners(rectCorners);
        scrollRectTransform.GetWorldCorners(scrollCorners);

        float epsilon = 0.0001f;
        float pixelToNormalized = scrollRectTransform.sizeDelta.y * 0.04f;
        while (NotWithinEpsilon() && previousScrollPos == scrollRect.verticalNormalizedPosition && scrollRect.gameObject.activeSelf) {
            // autoscroll using asymptotic averaging
            // scroll speed is calibrated for pixels, not normalized
            float scrollAmount = Mathf.Min(scrollRect.verticalNormalizedPosition * scrollSpeed * pixelToNormalized, (scrollCorners[1].y - rectCorners[1].y) * scrollSpeed);
            scrollRect.verticalNormalizedPosition -= scrollAmount;
            previousScrollPos = scrollRect.verticalNormalizedPosition;
            yield return new WaitForFixedUpdate();

            rectTransform.GetWorldCorners(rectCorners);
            scrollRectTransform.GetWorldCorners(scrollCorners);
        }

        bool NotWithinEpsilon() {
            return scrollRect.verticalNormalizedPosition > epsilon * pixelToNormalized && Mathf.Abs(scrollCorners[1].y - rectCorners[1].y) * scrollSpeed > epsilon;
        }
    }

    // set all location toggles to match set toggle
    public void OnSetToggle(Toggle toggle) {
        SetAllToggleStatesTo(toggle.isOn);
        RefreshLocationToggles();
    }

    // turn set toggle off if all location toggles are off
    public void RefreshSetToggle() {
        foreach (bool state in ToggleStates) {
            if (state) {
                setToggle.isOn = true;
                return;
            }
        }
        setToggle.isOn = false;
    }

    private void RefreshLocationToggles(bool supressTween = false) {
        foreach (var locationToggle in LocationToggles) {
            locationToggle.RefreshToggle(supressTween);
        }
    }

    public void SaveLocationStates() {
        int i = 0;
        foreach (bool state in ToggleStates) {
            ThisSet.Locations[i].enabled = state;
            i++;
        }
    }
}

// visual representation of ToggleStates list
public class LocationToggle {
    public readonly Toggle toggle;
    public readonly int locationIndex;
    private readonly LocationSetController parent;

    public LocationToggle(LocationSetController parent, Toggle toggle, int locationIndex) {
        this.toggle = toggle;
        this.locationIndex = locationIndex;
        this.parent = parent;
    }

    public void OnToggleChanged() {
        parent.ToggleStates[locationIndex] = toggle.isOn;
        parent.RefreshSetToggle();
    }

    public void RefreshToggle(bool supressTween = false) {
        toggle.isOn = parent.ToggleStates[locationIndex];
        if (supressTween) {
            toggle.GetComponent<ToggleSwitchController>().SupressTween(parent.ToggleStates[locationIndex]);
        }
    }
}

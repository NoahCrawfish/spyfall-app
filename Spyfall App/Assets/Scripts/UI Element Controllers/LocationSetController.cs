using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocationSetController : MonoBehaviour {
    [SerializeField] protected GameObject locationTogglePrefab;
    [SerializeField] protected Toggle setToggle;

    protected bool expanded = false;
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

    protected const float autoScrollSpeed = 0.0015f;
    protected RectTransform rectTransform;
    protected Task currentAutoscroll;
   

    protected virtual void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void OnEnable() {
        expanded = false;
        StartCoroutine(Initialize());
    }

    protected virtual void OnDisable() {
        DestroyChildren();
    }

    protected virtual IEnumerator Initialize() {
        yield return new WaitForEndOfFrame();
        if (ThisSet.locked) {
            InitializeLocked();
        } else {
            InitializeUnlocked();
        }
    }

    public virtual void InitializeLocked() {
        setToggle.enabled = false;
    }

    public virtual void InitializeUnlocked() {
        setToggle.enabled = true;

        // initialize SettingsUI for each location
        foreach (var location in ThisSet.Locations) {
            location.SettingsUI = new Location.SettingsUIComponent(this) {
                toggleValue = location.enabled
            };
        }

        RefreshSetToggle(true);
        RefreshLocationToggles();
    }


    public virtual void ExpandedChanged(bool isExpanded) {
        if (isExpanded) {
            int i = 0;
            foreach (var location in ThisSet.Locations) {
                // create prefab and set placement in heiarchy
                GameObject locationToggle = Instantiate(locationTogglePrefab);
                locationToggle.transform.SetParent(transform.Find("Children"), false);
                locationToggle.transform.Find("LocationButton/Renderer").GetChild(0).GetComponent<TextMeshProUGUI>().text = location.name;
                locationToggle.name = $"Location_{i}";

                // create and assign toggle
                Toggle toggle = locationToggle.transform.Find("ToggleFrame/LocationToggle").GetComponent<Toggle>();
                location.SettingsUI.AssignToggle(toggle);
                location.SettingsUI.RefreshToggle(true); // supresses tweening on location creation

                i++;
            }

            AutoScroll();
        } else {
            currentAutoscroll?.Stop();
            DestroyChildren();
        }
    }

    protected virtual void DestroyChildren() {
        foreach (Transform child in transform.Find("Children")) {
            Destroy(child.gameObject);
        }
    }

    public void AutoScroll() {
        GameObject scrollList = transform.root.Find("SettingsScreen/Frame/ScrollList").gameObject;
        ScrollRect scrollRect = scrollList.GetComponent<ScrollRect>();
        RectTransform scrollRectTransform = scrollList.GetComponent<RectTransform>();
        currentAutoscroll = new Task(ScrollToShowSet(scrollRect, scrollRectTransform, autoScrollSpeed));
    }

    protected virtual IEnumerator ScrollToShowSet(ScrollRect scrollRect, RectTransform scrollRectTransform, float scrollSpeed) {
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
    public virtual void OnSetToggle(Toggle toggle) {
        foreach (var location in ThisSet.Locations) {
            location.SettingsUI.toggleValue = toggle.isOn;
        }
        RefreshLocationToggles();
    }

    // turn set toggle off if all location toggles are off
    public virtual void RefreshSetToggle(bool supressTween = false) {
        bool turnOn = false;
        foreach (var location in ThisSet.Locations) {
            turnOn |= location.SettingsUI.toggleValue;
        }
        setToggle.isOn = turnOn;

        if (supressTween) {
            setToggle.GetComponent<ToggleSwitchController>().SupressTween(setToggle.isOn);
        }
    }

    protected virtual void RefreshLocationToggles(bool supressTween = false) {
        if (Expanded) {
            foreach (var location in ThisSet.Locations) {
                location.SettingsUI.RefreshToggle(supressTween);
            }
        }
    }

    public virtual void SaveLocationStates() {
        if (!ThisSet.locked) {
            foreach (var location in ThisSet.Locations) {
                location.enabled = location.SettingsUI.toggleValue;
            }
        }
    }
}
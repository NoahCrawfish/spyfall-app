using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
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
        ThisSet.Locations.ForEach(location => location.SettingsUI ??= new Location.SettingsUIComponent(this, location.enabled));
        RefreshSetToggle(true);
    }


    public virtual void ExpandedChanged(bool isExpanded, bool doAutoScroll = true) {
        if (isExpanded) {
            foreach (var location in ThisSet.Locations) {
                // create prefab and set placement in heiarchy
                GameObject locationToggle = Instantiate(locationTogglePrefab);
                locationToggle.transform.SetParent(transform.Find("Children"), false);
                locationToggle.transform.Find("LocationButton/Renderer").GetChild(0).GetComponent<TextMeshProUGUI>().text = location.Name;
                locationToggle.name = $"Location_{ThisSet.Locations.IndexOf(location)}";

                // create and assign toggle
                Toggle toggle = locationToggle.transform.Find("ToggleFrame/LocationToggle").GetComponent<Toggle>();
                location.SettingsUI.AssignToggle(toggle);
                location.SettingsUI.RefreshToggle(true); // supresses tweening on location creation
            }

            if (doAutoScroll) {
                AutoScroll();
            }
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

        Camera cam = FindObjectOfType<Camera>();
        float scrollRectHeight = cam.WorldToScreenPoint(scrollCorners[1]).y - cam.WorldToScreenPoint(scrollCorners[0]).y;

        float epsilon = 0.0001f;
        float pixelToNormalized = scrollRectHeight * 0.04f;
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
        ThisSet.Locations.Where(location => location.SettingsUI != null).ToList().ForEach(location => location.SettingsUI.toggleValue = toggle.isOn);
        if (Expanded) {
            ThisSet.Locations.Where(location => location.SettingsUI != null).ToList().ForEach(location => location.SettingsUI.RefreshToggle());
        }
    }

    // turn set toggle off if all location toggles are off
    public virtual void RefreshSetToggle(bool supressTween = false) {
        bool turnOn = false;
        ThisSet.Locations.ForEach(location => turnOn |= location.SettingsUI.toggleValue);
        setToggle.isOn = turnOn;

        if (supressTween) {
            setToggle.GetComponent<ToggleSwitchController>().SupressTween(setToggle.isOn);
        }
    }

    public virtual void SaveLocationStates() {
        if (!ThisSet.locked) {
            ThisSet.Locations.Where(location => location.SettingsUI != null).ToList().ForEach(location => location.enabled = location.SettingsUI.toggleValue);
            ThisSet.Locations.ForEach(location => location.SettingsUI = null);
        }
    }
}
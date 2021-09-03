using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomLocationSetController : LocationSetController {
    [SerializeField] private GameObject addCustomLocationPrefab;
    private int startingLocationsCount;

    protected override void Awake() {
        base.Awake();
    }

    protected override void OnEnable() {
        base.OnEnable();
    }

    protected override void OnDisable() {
        base.OnDisable();
    }

    public override void InitializeLocked() {
        setToggle.transform.parent.gameObject.SetActive(false);
    }

    public override void InitializeUnlocked() {
        setToggle.transform.parent.gameObject.SetActive(true);
        startingLocationsCount = ThisSet.Locations.Count;
        base.InitializeUnlocked();
    }

    public override void ExpandedChanged(bool isExpanded) {
        if (isExpanded) {
            int i = 0;
            foreach (CustomLocation customLocation in ThisSet.Locations) {
                // create prefab and set placement in heiarchy
                GameObject customLocationToggle = Instantiate(locationTogglePrefab);
                customLocationToggle.transform.SetParent(transform.Find("Children"), false);
                customLocationToggle.transform.Find("LocationButton/Renderer").GetChild(0).GetComponent<TextMeshProUGUI>().text = customLocation.name;
                customLocationToggle.name = $"CustomLocation_{i}";

                Toggle toggle = customLocationToggle.transform.Find("ToggleFrame/LocationToggle").GetComponent<Toggle>();
                customLocation.SettingsUI.AssignToggle(toggle);
                customLocation.SettingsUI.RefreshToggle(true); // supresses tweening on location creation

                i++;
            }

            CreateAddButton();
            AutoScroll();
        } else {
            currentAutoscroll?.Stop();
            DestroyChildren();
        }
    }

    private void CreateAddButton() {
        GameObject spacer = new GameObject("Spacer");
        spacer.AddComponent<RectTransform>();
        spacer.transform.SetParent(transform.Find("Children"), false);

        GameObject addButton = Instantiate(addCustomLocationPrefab);
        addButton.transform.SetParent(transform.Find("Children"), false);
        addButton.name = $"AddCustomLocation";
    }

    public void AddCustomLocation() {
        // create ui element for custom location
        GameObject customLocationToggle = Instantiate(locationTogglePrefab);
        Transform parent = transform.Find("Children");
        customLocationToggle.transform.SetParent(parent, false);
        customLocationToggle.transform.SetSiblingIndex(parent.childCount - 2);
        Toggle toggle = customLocationToggle.transform.Find("ToggleFrame/LocationToggle").GetComponent<Toggle>();

        // add new custom location to set class
        CustomLocation customLocation = new CustomLocation("New Custom Location", "Placeholder Role", null);
        ((CustomLocationSet)ThisSet).AddLocation(customLocation);
        // initalize SettingsUI subclass
        customLocation.SettingsUI = new Location.SettingsUIComponent(this, toggle) {
            toggleValue = customLocation.enabled
        };
        customLocation.SettingsUI.RefreshToggle(true); // supresses tweening on location creation


        // set ui element name and text
        customLocationToggle.name = $"CustomLocation_{ThisSet.Locations.IndexOf(customLocation)}";
        customLocationToggle.transform.Find("LocationButton/Renderer").GetChild(0).GetComponent<TextMeshProUGUI>().text = customLocation.name;

        AutoScroll();
    }

    public void DeleteNewCustomLocations() {
        // remove locations from the end of the list until it's length is the same as the start
        while (ThisSet.Locations.Count > startingLocationsCount) {
            ((CustomLocationSet)ThisSet).RemoveLocation((CustomLocation)ThisSet.Locations[ThisSet.Locations.Count - 1]);
        }
    }
}

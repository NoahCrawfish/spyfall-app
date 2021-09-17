using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomLocationSetController : LocationSetController {
    [SerializeField] private GameObject addCustomLocationPrefab;
    public List<Location> NotDeletedLocations => ThisSet.Locations.Where(location => !((CustomLocation)location).Deleted).ToList();
    private HandleButtons handleButtons;
    private UITransitions uiTransitions;

    protected override void Awake() {
        handleButtons = FindObjectOfType<HandleButtons>();
        uiTransitions = FindObjectOfType<UITransitions>();
        base.Awake();
    }

    protected override void OnEnable() {
        if (uiTransitions.PreviousScreen != null) {
            if (uiTransitions.PreviousScreen.gameObject.name == "CustomizeLocationScreen") {
                expanded = true;
                ExpandedChanged(expanded, doAutoScroll: false);
                StartCoroutine(Initialize());
                return;
            }
        }
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

        setToggle.enabled = true;
        ThisSet.Locations.ForEach(location => location.SettingsUI ??= new CustomLocation.CustomSettingsUIComponent((CustomLocation)location, this, location.enabled));
        RefreshSetToggle(true);
    }

    protected CustomLocation.CustomSettingsUIComponent GetCustomSettingsUI(Location location) {
        return (CustomLocation.CustomSettingsUIComponent)((CustomLocation)location).SettingsUI;
    }

    public override void ExpandedChanged(bool isExpanded, bool doAutoScroll = true) {
        if (isExpanded) {
            foreach (CustomLocation customLocation in ThisSet.Locations.Where(location => !((CustomLocation)location).Deleted)) {
                // create prefab and set placement in heiarchy
                GameObject customLocationToggle = Instantiate(locationTogglePrefab);
                customLocationToggle.transform.SetParent(transform.Find("Children"), false);
                customLocationToggle.transform.Find("LocationButton/Renderer").GetChild(0).GetComponent<TextMeshProUGUI>().text = GetCustomSettingsUI(customLocation).TempName;
                customLocationToggle.name = $"CustomLocation_{ThisSet.Locations.IndexOf(customLocation)}";

                Toggle toggle = customLocationToggle.transform.Find("ToggleFrame/LocationToggle").GetComponent<Toggle>();
                customLocation.SettingsUI.AssignToggle(toggle);
                customLocation.SettingsUI.RefreshToggle(true); // supresses tweening on location creation
            }

            CreateAddButton();
            if (doAutoScroll) {
                AutoScroll();
            }
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
        CustomLocation customLocation = new CustomLocation();
        ((CustomLocationSet)ThisSet).AddLocation(customLocation);
        customLocation.SettingsUI = new CustomLocation.CustomSettingsUIComponent(customLocation, this, customLocation.enabled);

        handleButtons.TransitionToCustomizeLocation(customLocation);
    }

    public override void OnSetToggle(Toggle toggle) {
        if (NotDeletedLocations.Count == 0) {
            toggle.isOn = false;
        } else {
            NotDeletedLocations.ForEach(location => location.SettingsUI.toggleValue = toggle.isOn);
            if (Expanded) {
                NotDeletedLocations.ForEach(location => location.SettingsUI.RefreshToggle());
            }
        }
    }

    public override void RefreshSetToggle(bool supressTween = false) {
        bool turnOn = false;
        NotDeletedLocations.ForEach(location => turnOn |= location.SettingsUI.toggleValue);
        setToggle.isOn = turnOn;

        if (supressTween) {
            setToggle.GetComponent<ToggleSwitchController>().SupressTween(setToggle.isOn);
        }
    }

    public override void SaveLocationStates() {
        ThisSet.Locations.RemoveAll(location => ((CustomLocation)location).Deleted);
        ThisSet.Locations.ForEach(location => ((CustomLocation)location).JustAdded = false);

        if (!ThisSet.locked) {
            ThisSet.Locations.Where(location => GetCustomSettingsUI(location) != null).ToList().ForEach(location => {
                location.Name = GetCustomSettingsUI(location).GetRealName();
                location.Roles = GetCustomSettingsUI(location).GetRealRoles();
                ((CustomLocation)location).SetImage(GetCustomSettingsUI(location).TempImage);
            });
        }

        base.SaveLocationStates();
    }
}

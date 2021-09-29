using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using TMPro;

public class ManageCustomizeLocationScreen : MonoBehaviour
{
    public CustomLocation ThisLocation { get; private set; }
    public CustomLocation.CustomSettingsUIComponent SettingsUI => (CustomLocation.CustomSettingsUIComponent)ThisLocation.SettingsUI;

    [SerializeField] private GameObject rolesFrame;
    [SerializeField] private GameObject roleFieldPrefab;
    [SerializeField] private GameObject scrollList;
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private ProceduralImage imagePreview;
    [SerializeField] private GameObject drawingElements;

    private const float autoScrollSpeed = 0.1f;
    private const int objectsAfterPlayersInPlayerFields = 4;

    private Texture2D defaultImage;
    public Texture2D PreviousImage { get; set; }

    private void Awake() {
        defaultImage = Resources.Load("custom_location_default") as Texture2D;
    }

    public IEnumerator Initialize(CustomLocation thisLocation) {
        ThisLocation = thisLocation;
        scrollList.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
        PreviousImage = SettingsUI.GetTempImage();
        yield return new WaitForEndOfFrame();

        ResetRoleFields();
        LoadRoleFields();

        nameField.text = SettingsUI.GetRealName() == null ? "" : SettingsUI.TempName;
        RefreshPreviewImage();
    }

    public void RefreshPreviewImage() {
        if (SettingsUI.TempImage != null) {
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(SettingsUI.TempImage);
            imagePreview.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
        } else {
            imagePreview.sprite = Sprite.Create(defaultImage, new Rect(0, 0, defaultImage.width, defaultImage.height), new Vector2(defaultImage.width / 2, defaultImage.height / 2));
        }
    }

    private void ResetRoleFields() {
        foreach (Transform role in rolesFrame.transform) {
            if (role.name.Split('_').Length > 1) {
                if (int.Parse(role.name.Split('_')[1]) > 3) {
                    Destroy(role.gameObject);
                } else {
                    role.GetChild(0).GetComponent<TMP_InputField>().text = "";
                }
            }
        }
    }

    private void LoadRoleFields() {
        if (SettingsUI.GetRealRoles() != null) {
            int i = 1;
            foreach (string role in SettingsUI.GetRealRoles()) {
                if (i > 3) {
                    AddRole(role);
                } else {
                    rolesFrame.transform.Find($"Role_{i}").GetChild(0).GetComponent<TMP_InputField>().text = role;
                }
                i++;
            }
        }
    }

    public void AddRole(string value = null) {
        // create field and format in vertical layout group
        GameObject newField = Instantiate(roleFieldPrefab);
        newField.transform.SetParent(rolesFrame.transform, false);
        newField.transform.SetSiblingIndex(rolesFrame.transform.childCount - (objectsAfterPlayersInPlayerFields + 1));

        // set name and default text
        newField.name = $"Role_{newField.transform.GetSiblingIndex() + 1}";
        UpdateFieldPlaceholder(newField);

        if (value != null) {
            newField.transform.GetChild(0).GetComponent<TMP_InputField>().text = value;
        } else {
            // scroll down to show new field
            StartCoroutine(ScrollToBottom(scrollList.GetComponent<ScrollRect>(), autoScrollSpeed));
        }
    }

    private IEnumerator ScrollToBottom(ScrollRect scrollRect, float scrollSpeed) {
        yield return new WaitForEndOfFrame();
        // previousScrollPos is used to test if the player has scrolled, canceling the auto scroll
        float previousScrollPos = scrollRect.verticalNormalizedPosition;

        while (scrollRect.verticalNormalizedPosition > 0f && previousScrollPos == scrollRect.verticalNormalizedPosition && scrollRect.gameObject.activeSelf) {
            // autoscroll using asymptotic averaging
            scrollRect.verticalNormalizedPosition -= scrollRect.verticalNormalizedPosition * scrollSpeed;
            previousScrollPos = scrollRect.verticalNormalizedPosition;
            yield return new WaitForFixedUpdate();
        }
    }

    private void UpdateFieldPlaceholder(GameObject field) {
        TextMeshProUGUI placeholder = (TextMeshProUGUI)field.transform.GetChild(0).GetComponent<TMP_InputField>().placeholder;
        placeholder.text = field.name.Replace("_", " ");
    }

    public void Trash(GameObject caller) {
        int callerPlace = caller.transform.parent.GetSiblingIndex();
        // update name and placeholders for all fields under callerPlace in the sibling heiarchy
        foreach (Transform child in rolesFrame.transform) {
            GameObject childObject = child.gameObject;
            if (childObject.name.Split('_')[0] == "Role" && child.GetSiblingIndex() > callerPlace) {
                childObject.name = $"Role_{child.GetSiblingIndex()}";
                UpdateFieldPlaceholder(childObject);
            }
        }

        Destroy(caller.transform.parent.gameObject);
    }

    public void SaveLocationSettings() {
        SettingsUI.TempName = nameField.text == "" ? null : nameField.text;

        SettingsUI.TempRoles.Clear();
        List<string> rolesToAdd = new List<string>();
        foreach (Transform role in rolesFrame.transform) {
            if (role.name.Split('_')[0] == "Role") {
                string text = role.GetChild(0).GetComponent<TMP_InputField>().text;
                TextMeshProUGUI placeholder = (TextMeshProUGUI)role.GetChild(0).GetComponent<TMP_InputField>().placeholder;
                string placeholderText = placeholder.text;

                string newRole = string.IsNullOrWhiteSpace(text) ? placeholderText : text;
                rolesToAdd.Add(newRole);
            }
        }
        SettingsUI.TempRoles = rolesToAdd;
    }

    public void DeleteLocation() {
        ThisLocation.Deleted = true;
    }

    public void ShowEditImageCanvas() {
        drawingElements.SetActive(true);
        drawingElements.GetComponent<DrawingElementsController>().Manager = this;
        drawingElements.GetComponent<DrawingElementsController>().Initialize();
    }
}

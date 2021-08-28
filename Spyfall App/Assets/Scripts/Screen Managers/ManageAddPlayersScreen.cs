using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManageAddPlayersScreen : MonoBehaviour
{
    [SerializeField] private GameObject playerFields;
    [SerializeField] private GameObject playerFieldPrefab;
    [SerializeField] private GameObject scrollList;
    [SerializeField] private TextMeshProUGUI beginText;

    private const float autoScrollSpeed = 0.1f;
    private const int objectsAfterPlayersInPlayerFields = 3;

    public void Initialize() {
        scrollList.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
    }

    public void AddField() {
        // create field and format in vertical layout group
        GameObject newField = Instantiate(playerFieldPrefab);
        newField.transform.SetParent(playerFields.transform, false);
        newField.transform.SetSiblingIndex(playerFields.transform.childCount - (objectsAfterPlayersInPlayerFields + 1));

        // set name and default text
        newField.name = $"Player_{newField.transform.GetSiblingIndex() + 1}";
        UpdateFieldPlaceholder(newField);

        // scroll down to show new field
        StartCoroutine(ScrollToBottom(scrollList.GetComponent<ScrollRect>(), autoScrollSpeed));
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
        foreach (Transform child in playerFields.transform) {
            GameObject childObject = child.gameObject;
            if (childObject.name.Split('_')[0] == "Player" && child.GetSiblingIndex() > callerPlace) {
                childObject.name = $"Player_{child.GetSiblingIndex()}";
                UpdateFieldPlaceholder(childObject);
            }
        }

        Destroy(caller.transform.parent.gameObject);
    }

    public void NoLocationsEnabled() {
        beginText.text = "NO LOCATIONS ENABLED";
        beginText.GetComponent<FlashText>().ResetT();
        beginText.GetComponent<FlashText>().flashSpeed = 2.5f;
    }

    public void ResetBeginButton() {
        beginText.text = "BEGIN";
        beginText.GetComponent<FlashText>().ResetT();
        beginText.GetComponent<FlashText>().flashSpeed = 0;
    }
}

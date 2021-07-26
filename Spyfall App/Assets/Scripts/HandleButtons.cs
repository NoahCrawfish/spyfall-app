using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HandleButtons : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject play1;
    [SerializeField] private GameObject roundScreen;
    [SerializeField] private GameObject play2;

    [SerializeField] private GameObject playerFields;
    [SerializeField] private GameObject playerFieldPrefab;
    [SerializeField] private GameObject scrollList;

    private const float autoScrollSpeed = 0.1f;
    private const float roundScreenTime = 1f;

    private ManageGame manageGame;
    private UITransitions uiTransitions;

    private void Awake() {
        manageGame = FindObjectOfType<ManageGame>();
        uiTransitions = FindObjectOfType<UITransitions>();
    }

    public void Play() {
        uiTransitions.CrossFadeBetweenPanels(mainMenu.GetComponent<CanvasGroup>(), play1.GetComponent<CanvasGroup>());
    }

    public void Back_Play1() {
        uiTransitions.CrossFadeBetweenPanels(play1.GetComponent<CanvasGroup>(), mainMenu.GetComponent<CanvasGroup>());
    }

    public void AddField() {
        // create field and format in vertical layout group
        GameObject newField = Instantiate(playerFieldPrefab);
        newField.transform.SetParent(playerFields.transform, false);
        newField.transform.SetSiblingIndex(playerFields.transform.childCount - 2);

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
            yield return new WaitForEndOfFrame();
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

    public void Begin() {
        // unique first-round setup
        manageGame.CreatePlayerList();
        manageGame.InitializeLocationsUsing();
        manageGame.ResetRounds();

        StartCoroutine(NextRound());
    }

    private IEnumerator NextRound() {
        manageGame.UpdateRound();
        manageGame.AssignLocationAndRoles();

        // briefly show round screen
        if (manageGame.CurrentRound == 1) {
            uiTransitions.CrossFadeBetweenPanels(play1.GetComponent<CanvasGroup>(), roundScreen.GetComponent<CanvasGroup>());
        }
        yield return new WaitForSeconds(roundScreenTime);
        uiTransitions.CrossFadeBetweenPanels(roundScreen.GetComponent<CanvasGroup>(), play2.GetComponent<CanvasGroup>());
    }

    public void QuitGame() {
        uiTransitions.CrossFadeBetweenPanels(FindObjectOfType<CanvasGroup>(), mainMenu.GetComponent<CanvasGroup>());
    }
}

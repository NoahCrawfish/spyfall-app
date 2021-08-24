using UnityEngine;
using TMPro;

public class ManagePlayerScoring2 : MonoBehaviour
{
    [SerializeField] private GameObject playerButtonPrefab;
    public GameObject playerList;

    private const string buttonNames = "PlayerButton";
    private ManageGame manageGame;

    private void Awake() {
        manageGame = FindObjectOfType<ManageGame>();
    }

    public void RefreshPlayerButtons() {
        foreach (Transform child in playerList.transform) {
            if (child.name == buttonNames) {
                Destroy(child.gameObject);
            }
        }

        foreach (var player in manageGame.Players) {
            if (player.Role != "Spy") {
                GameObject button = Instantiate(playerButtonPrefab, playerList.transform);
                button.name = buttonNames;
                button.transform.Find("Renderer").GetChild(0).GetComponent<TextMeshProUGUI>().text = player.Name;
            }
        }
    }
}

using UnityEngine;
using TMPro;

public class ManageLeaderboard : MonoBehaviour
{
    [SerializeField] private GameObject playerScorePrefab;
    [SerializeField] private GameObject leaderboard;
    [SerializeField] private GameObject nextRoundButton;

    private const string rowName = "PlayerScore";
    private const int elementsBeforeRows = 2;
    private ManageGame manageGame;

    private void Awake() {
        manageGame = FindObjectOfType<ManageGame>();
    }

    public void RefreshLeaderboard() {
        foreach (Transform child in leaderboard.transform) {
            if (child.name == rowName) {
                Destroy(child.gameObject);
            }
        }

        foreach (var player in manageGame.Players) {
            GameObject row = Instantiate(playerScorePrefab, leaderboard.transform);
            row.name = rowName;
            row.transform.GetChild(0).Find("Renderer").GetChild(0).GetComponent<TextMeshProUGUI>().text = player.Name;
            row.transform.GetChild(1).Find("Renderer").GetChild(0).GetComponent<TextMeshProUGUI>().text = player.Score.ToString();
        }

        RefreshNextRoundButton();
        OrderLeaderboard();
    }

    private void RefreshNextRoundButton() {
        nextRoundButton.transform.Find("Renderer").GetChild(0).GetComponent<TextMeshProUGUI>().text = (manageGame.CurrentRound < manageGame.MaxRounds) ? "NEXT ROUND" : "FINISH";
    }

    private void OrderLeaderboard() {
        foreach (Transform child in leaderboard.transform) {
            if (child.name == rowName && child.GetSiblingIndex() > elementsBeforeRows) {
                while (GetRowScore(child) > GetRowScore(leaderboard.transform.GetChild(child.GetSiblingIndex() - 1))) {
                    child.SetSiblingIndex(child.GetSiblingIndex() - 1);
                    if (child.GetSiblingIndex() == elementsBeforeRows) {
                        break;
                    }
                }
            }
        }
    }

    private float GetRowScore(Transform row) {
        return float.Parse(row.GetChild(1).Find("Renderer").GetChild(0).GetComponent<TextMeshProUGUI>().text);
    }
}

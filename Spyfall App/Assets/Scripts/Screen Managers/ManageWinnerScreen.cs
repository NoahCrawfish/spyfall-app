using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManageWinnerScreen : MonoBehaviour
{
    [SerializeField] private GameObject winnerScreen;
    [SerializeField] private GameObject addPlayersScreen;
    [SerializeField] private TextMeshProUGUI winnerText;

    private ManageGame manageGame;

    private void Awake() {
        manageGame = FindObjectOfType<ManageGame>();
    }

    public void GetWinners() {
        float highestScore = float.MinValue;
        List<string> winners = new List<string>();

        foreach (var player in manageGame.Players) {
            if (player.Score > highestScore) {
                winners.Clear();
                winners.Add(player.Name);
                highestScore = player.Score;
            } else if (player.Score == highestScore) {
                winners.Add(player.Name);
            }
        }

        SetWinnerText(winners);
    }

    private void SetWinnerText(List<string> winners) {
        string text = (winners.Count > 1) ? "Winners:\n" : "Winner:\n";
        foreach (var winner in winners) {
            text += $"{winner}\n";
        }
        text = text.Remove(text.Length - 1);
        winnerText.text = text;
    }
}

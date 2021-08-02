using System;
using UnityEngine;
using TMPro;

public class HandleButtons : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject addPlayersScreen;
    [SerializeField] private GameObject drawCardsScreen;
    [SerializeField] private GameObject gameplayScreen;
    [SerializeField] private GameObject scoringScreen1;
    [SerializeField] private GameObject scoringScreen2;
    [SerializeField] private GameObject leaderboardScreen;
    [SerializeField] private GameObject winnerScreen;

    private ManageGame manageGame;
    private UITransitions uiTransitions;
    private ManageAddPlayersScreen manageAddPlayers;
    private ManageDrawCardsScreen manageDrawCards;
    private ManageGameplayScreen manageGameplay;
    private ManagePlayerScoring manageScoring;
    private ManagePlayerScoring2 manageScoring2;
    private ManageLeaderboard manageLeaderboard;
    private ManageWinnerScreen manageWinner;

    private void Awake() {
        manageGame = FindObjectOfType<ManageGame>();
        uiTransitions = FindObjectOfType<UITransitions>();
        manageAddPlayers = FindObjectOfType<ManageAddPlayersScreen>();
        manageDrawCards = FindObjectOfType<ManageDrawCardsScreen>();
        manageGameplay = FindObjectOfType<ManageGameplayScreen>();
        manageScoring = FindObjectOfType<ManagePlayerScoring>();
        manageScoring2 = FindObjectOfType<ManagePlayerScoring2>();
        manageLeaderboard = FindObjectOfType<ManageLeaderboard>();
        manageWinner = FindObjectOfType<ManageWinnerScreen>();
    }

    private CanvasGroup GetCurrentPanel() {
        foreach (Transform child in transform) {
            if (child.gameObject.activeSelf) {
                try {
                    return child.GetComponent<CanvasGroup>();
                } catch (MissingComponentException) {
                    throw new Exception($"Canvas group not found for {child.name}");
                }
            }
        }
        return null;
    }

    public void Play() {
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), addPlayersScreen.GetComponent<CanvasGroup>());
    }

    public void Back_Play1() {
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), mainMenu.GetComponent<CanvasGroup>());
    }

    public void AddField() {
        manageAddPlayers.AddField();
    }

    public void Trash(GameObject caller) {
        manageAddPlayers.Trash(caller);
    }

    public void Begin() {
        // unique first-round setup
        manageGame.CreatePlayerList();
        manageGame.InitializeLocationsUsing();
        manageGame.ResetRounds();

        StartCoroutine(manageGame.StartNextRound(GetCurrentPanel()));
    }

    public void QuitGame() {
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), mainMenu.GetComponent<CanvasGroup>());
    }

    public void NextCardButton() {
        if (manageDrawCards.CardCount < manageGame.Players.Count - 1) {
            manageDrawCards.NextCard();
        } else {
            manageDrawCards.nextCardButton.gameObject.SetActive(false);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), gameplayScreen.GetComponent<CanvasGroup>());
            manageGameplay.InitializeScreen();
        }
    }

    public void DoneWithRound() {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), scoringScreen1.GetComponent<CanvasGroup>());
        manageScoring.SetPreviousScores();
    }

    public void SkipScoring() {
        StartCoroutine(manageGame.StartNextRound(GetCurrentPanel()));
    }

    public void SpyFound() {
        manageScoring2.RefreshPlayerButtons();
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), scoringScreen2.GetComponent<CanvasGroup>());
    }

    public void SpyGuessedWrong() {
        manageScoring2.RefreshPlayerButtons();
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), scoringScreen2.GetComponent<CanvasGroup>());
    }

    public void SpyGuessedRight() {
        manageScoring.UpdateScores(ManagePlayerScoring.RoundEndings.spyGuessedRight);
        manageLeaderboard.RefreshLeaderboard();
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), leaderboardScreen.GetComponent<CanvasGroup>());
    }

    public void InnocentVoted() {
        manageScoring.UpdateScores(ManagePlayerScoring.RoundEndings.innocentVoted);
        manageLeaderboard.RefreshLeaderboard();
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), leaderboardScreen.GetComponent<CanvasGroup>());
    }

    public void TimeRanOut() {
        manageScoring.UpdateScores(ManagePlayerScoring.RoundEndings.timeRanOut);
        manageLeaderboard.RefreshLeaderboard();
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), leaderboardScreen.GetComponent<CanvasGroup>());
    }

    public void Back_Scoring2() {
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), scoringScreen1.GetComponent<CanvasGroup>());
    }

    public void NoOneVoted() {
        manageScoring.UpdateScores(ManagePlayerScoring.RoundEndings.spyFoundOrGuessedWrong);
        manageLeaderboard.RefreshLeaderboard();
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), leaderboardScreen.GetComponent<CanvasGroup>());
    }

    public void PlayerButton(GameObject caller) {
        Player firstToVote = null;
        foreach (var player in manageGame.Players) {
            if (player.Name == caller.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text) {
                firstToVote = player;
            }
        }

        manageScoring.UpdateScores(ManagePlayerScoring.RoundEndings.spyFoundOrGuessedWrong, firstToVote);
        manageLeaderboard.RefreshLeaderboard();
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), leaderboardScreen.GetComponent<CanvasGroup>());
    }

    public void Back_Leaderboard() {
        manageScoring.ResetScoresToPrevious();
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), scoringScreen1.GetComponent<CanvasGroup>());
    }

    public void NextRoundButton() {
        if (manageGame.CurrentRound < manageGame.MaxRounds) {
            StartCoroutine(manageGame.StartNextRound(GetCurrentPanel()));
        } else {
            uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), winnerScreen.GetComponent<CanvasGroup>());
            manageWinner.GetWinners();
        }
    }

    public void GameFinishedButton() {
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), addPlayersScreen.GetComponent<CanvasGroup>());
    }
}

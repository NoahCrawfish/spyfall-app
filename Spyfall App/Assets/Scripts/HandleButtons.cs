using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
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
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject customizeLocationScreen;
    [SerializeField] private GameObject instructionsScreen;
    public GameObject purchasePopup;

    private ManageGame manageGame;
    private UITransitions uiTransitions;
    private ManageAddPlayersScreen manageAddPlayers;
    private ManageDrawCardsScreen manageDrawCards;
    private ManageGameplayScreen manageGameplay;
    private ManagePlayerScoring manageScoring;
    private ManagePlayerScoring2 manageScoring2;
    private ManageLeaderboard manageLeaderboard;
    private ManageWinnerScreen manageWinner;
    private ManageSettingsScreen manageSettings;
    private ManageCustomizeLocationScreen manageCustomizeLocation;
    private DrawingUI ThisDrawingUI => FindObjectOfType<DrawingUI>();
    private ManageAudio manageAudio;

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
        manageSettings = FindObjectOfType<ManageSettingsScreen>();
        manageCustomizeLocation = FindObjectOfType<ManageCustomizeLocationScreen>();
        manageAudio = FindObjectOfType<ManageAudio>();
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
        manageAddPlayers.Initialize();
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
        manageGame.InitializeLocationsUsing();
        if (manageGame.LocationsUsing.Count > 0) {
            // unique first-round setup
            manageGame.CreatePlayerList();
            manageGame.ResetRounds();
            StartCoroutine(manageGame.StartNextRound(GetCurrentPanel()));
        }
    }

    public void QuitGame() {
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), mainMenu.GetComponent<CanvasGroup>());
    }

    public void NextCardButton() {
        manageDrawCards.nextCardButton.SetActive(false);

        if (manageDrawCards.CardCount < manageGame.Players.Count - 1) {
            manageDrawCards.NextCard();
        } else {
            manageDrawCards.UnloadBlurPanel();
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), gameplayScreen.GetComponent<CanvasGroup>());
            manageGameplay.InitializeScreen();
        }
    }

    public void DoneWithRound() {
        //manageGameplay.timerTask.Stop();
        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        if (!manageGame.ScoringDisabled) {
            uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), scoringScreen1.GetComponent<CanvasGroup>());
            manageScoring.SetPreviousScores();
        } else {
            if (manageGame.CurrentRound < manageGame.MaxRounds) {
                StartCoroutine(manageGame.StartNextRound(GetCurrentPanel()));
            } else {
                manageAddPlayers.Initialize();
                uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), addPlayersScreen.GetComponent<CanvasGroup>());
            }
        }
    }

    public void SkipScoring() {
        NextRoundButton();
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
            if (player.Name == caller.transform.Find("Renderer").GetChild(0).GetComponent<TextMeshProUGUI>().text) {
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
        manageAddPlayers.Initialize();
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), addPlayersScreen.GetComponent<CanvasGroup>());
    }

    public void MinusButton(GameObject caller) {
        caller.transform.parent.GetComponent<IncrementController>().Subtract();
    }

    public void PlusButton(GameObject caller) {
        caller.transform.parent.GetComponent<IncrementController>().Add();
    }

    public void TimerModeButton(GameObject caller) {
        caller.GetComponent<TimerModeController>().CycleStage();
    }

    public void Settings() {
        manageSettings.PreviousScreen = GetCurrentPanel();
        manageSettings.Initalize();
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), settingsScreen.GetComponent<CanvasGroup>());
    }

    public void SaveSettings() {
        manageSettings.SaveSettings();
        if (manageSettings.PreviousScreen == null) {
            uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), mainMenu.GetComponent<CanvasGroup>());
        } else {
            uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), manageSettings.PreviousScreen);
        }
    }

    public void DefaultSettings() {
        manageSettings.SetUIToDefault();
    }

    public void CancelSettings() {
        // revert changes to which custom locations were created/destroyed
        manageGame.CustomSet.Locations.RemoveAll(location => ((CustomLocation)location).JustAdded);
        manageGame.CustomSet.Locations.ForEach(location => ((CustomLocation)location).Deleted = false);
        // reset ui toggle data for every location
        manageGame.LocationSets.ForEach(locationSet => locationSet.Locations.ForEach(location => location.SettingsUI = null));
        manageGame.CustomSet.Locations.ForEach(location => location.SettingsUI = null);

        if (manageSettings.PreviousScreen == null) {
            uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), mainMenu.GetComponent<CanvasGroup>());
        } else {
            uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), manageSettings.PreviousScreen);
        }
    }

    public void SetButton(GameObject caller) {
        LocationSetController locationSet = caller.transform.parent.parent.GetComponent<LocationSetController>();
        if (locationSet.ThisSet.locked) {
            // show paid pop-up, also in GetToggleClicked script
            purchasePopup.SetActive(true);
        } else {
            locationSet.Expanded = !locationSet.Expanded;
        }
    }

    public void EditCustomLocationButton(GameObject caller) {
        manageAudio.PlayVariedPitch("click", 0.15f);
        GameObject customLocationToggle = caller.transform.parent.parent.gameObject;
        int index = int.Parse(customLocationToggle.name.Split('_')[1]);
        CustomLocation customLocation = (CustomLocation)manageGame.CustomSet.Locations[index];
        TransitionToCustomizeLocation(customLocation);
    }

    public void TransitionToCustomizeLocation(CustomLocation customLocation) {
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), customizeLocationScreen.GetComponent<CanvasGroup>());
        StartCoroutine(manageCustomizeLocation.Initialize(customLocation));
    }

    public void SaveCustomLocation() {
        manageCustomizeLocation.SaveLocationSettings();
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), settingsScreen.GetComponent<CanvasGroup>());
    }

    public void CancelCustomLocation() {
        manageCustomizeLocation.SettingsUI.SetTempImage(manageCustomizeLocation.PreviousImage);
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), settingsScreen.GetComponent<CanvasGroup>());
    }

    public void DeleteCustomLocation() {
        manageCustomizeLocation.DeleteLocation();
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), settingsScreen.GetComponent<CanvasGroup>());
    }

    public void AddRoleField() {
        manageCustomizeLocation.AddRole();
    }

    public void CreateImageButton() {
        manageCustomizeLocation.ShowEditImageCanvas();
    }

    public void RoleTrash(GameObject caller) {
        manageCustomizeLocation.Trash(caller);
    }

    public void WhiteColor() {
        ThisDrawingUI.CurrentColor = DrawingUI.Colors.white;
    }

    public void BlueColor() {
        ThisDrawingUI.CurrentColor = DrawingUI.Colors.blue;
    }

    public void OrangeColor() {
        ThisDrawingUI.CurrentColor = DrawingUI.Colors.orange;
    }

    public void BlackColor() {
        ThisDrawingUI.CurrentColor = DrawingUI.Colors.black;
    }

    public void Instructions() {
        uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), instructionsScreen.GetComponent<CanvasGroup>());
    }

    public void Back_Instructions() {
        if (uiTransitions.PreviousScreen.gameObject == gameplayScreen) {
            uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), uiTransitions.PreviousScreen, doAfter: manageGameplay.UnpauseScreen);
        } else {
            uiTransitions.CrossFadeBetweenPanels(GetCurrentPanel(), uiTransitions.PreviousScreen);
        }
    }
}

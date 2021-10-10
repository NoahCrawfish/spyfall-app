using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class ManageGameplayScreen : MonoBehaviour
{
    [SerializeField] private GameObject gameplayScreen;
    [SerializeField] private GameObject content;
    [SerializeField] private TextMeshProUGUI locationsList;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private InterstitialAd videoAd;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject doneButton;
    [SerializeField] private GameObject footerButtonFrame;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private TextMeshProUGUI startingPlayer;

    private int startingTime;
    private int elapsedTime = 0;
    public Task timerTask;
    private ManageGame manageGame;
#if UNITY_IOS
    iOSNotification notification;
#endif

    private readonly static System.Random rand = new System.Random();

    private void Awake() {
        manageGame = FindObjectOfType<ManageGame>();
    }

#if UNITY_IOS
    private void Start() {
        var timeTrigger = new iOSNotificationTimeIntervalTrigger() {
            TimeInterval = new TimeSpan(0, 0, 1),
            Repeats = false
        };

        notification = new iOSNotification() {
            Identifier = "timer_done_notification",
            Title = "Time's up!",
            Body = "Press the done button to continue",
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };
    }
#endif

    public void InitializeScreen() {
        SetPossibleLocations();
        timerText.GetComponent<FlashText>().flashSpeed = 0f;
        scrollRect.verticalNormalizedPosition = 1;
        timerTask = null;

        SetStartingPlayerText();

        if (!manageGame.PaidUnlocked) {
            HideButtons();
        }

        timerTask = new Task(TrackElapsedTime(manageGame.TimerMode != ManageGame.TimerModes.disabled));
    }

    private void SetStartingPlayerText() {
        string randPlayer = manageGame.Players[rand.Next(manageGame.Players.Count)].Name;
        startingPlayer.text = $"{randPlayer} asks the first question.";
    }

    private void HideButtons() {
        quitButton.SetActive(false);

        if (manageGame.TimerMode == ManageGame.TimerModes.disabled) {
            footerButtonFrame.SetActive(false);
        } else {
            doneButton.SetActive(false);
        }
    }

    public void ShowButtons() {
        quitButton.SetActive(true);

        if (manageGame.TimerMode == ManageGame.TimerModes.disabled) {
            footerButtonFrame.SetActive(true);
        } else {
            doneButton.SetActive(true);
        }
    }

    private void SetPossibleLocations() {
        // compile list of enabled location names
        List<string> locations = new List<string>();
        foreach (var locationSet in manageGame.LocationSets.Concat(new List<CustomLocationSet> { manageGame.CustomSet }).ToList()) {
            locationSet.Locations.Where(location => location.enabled).ToList().ForEach(location => locations.Add(location.Name));
        }

        string text = "";
        locations.OrderBy(x => x).ToList().ForEach(name => text += $"- {name}\n");
        // remove last new line
        text = text.Remove(text.Length - 1);

        locationsList.text = text;
        // needed to get content fitter to scale properly with content
        content.GetComponent<VerticalLayoutGroup>().spacing += 0.01f;
    }


    private IEnumerator TrackElapsedTime(bool timerEnabled) {
        InitializeTimer(timerEnabled);

        elapsedTime = 0;
        while (gameplayScreen.activeSelf) {
            if (timerEnabled) {
                UpdateTimer();
            }

            if (elapsedTime == 10 && !manageGame.PaidUnlocked) {
                videoAd.StartLoadAd();
            }

            yield return new WaitForSecondsRealtime(1f);
            elapsedTime += 1;
        }
    }

    private void InitializeTimer(bool timerEnabled) {
        startingTime = 0;

        if (timerEnabled) {
            timerText.transform.parent.gameObject.SetActive(true);

            startingTime = manageGame.TimerSeconds;
            if (manageGame.TimerMode == ManageGame.TimerModes.perPlayer) {
                startingTime *= manageGame.Players.Count;
            }
        } else {
            TimerDisabled();
        }
    }

    private void UpdateTimer() {
        int time = startingTime - elapsedTime;

        if (elapsedTime < startingTime) {
            int minutes = time / 60;
            int seconds = time - minutes * 60;
            timerText.text = $"{minutes:D2}:{seconds:D2}";
        } else if (startingTime - elapsedTime == 0) {
            TimerFinished();
        }
    }


    public void UpdateTimerFromPause() {
        if (gameplayScreen.activeSelf && elapsedTime < startingTime) {
            int deltaTime = Mathf.RoundToInt((float)(manageGame.UnpauseTime - manageGame.PauseTime).TotalSeconds);
            elapsedTime += deltaTime;
            UpdateTimer();
        }
    }

    private void TimerFinished() {
        timerText.text = "Time Up!";
        timerText.GetComponent<FlashText>().ResetT();
        timerText.GetComponent<FlashText>().flashSpeed = 2f;

        ManageAudio.Instance.Play("alarm");
#if UNITY_IOS
        iOSNotificationCenter.ScheduleNotification(notification);
#endif
    }

    private void TimerDisabled() {
        timerText.transform.parent.gameObject.SetActive(false);
    }

    // called from inspector
    public void PauseScreen() {
        timerTask?.Pause();
    }

    public void UnpauseScreen() {
        timerTask?.Unpause();
    }
}

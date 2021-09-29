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
    [SerializeField] private ScrollRect scrollRect;

    private int time;
    private ManageGame manageGame;
#if UNITY_IOS
    iOSNotification notification;
#endif
    private Task adBuffer;

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
        // schedule ad if the the full version hasn't been unlocked
        if (!manageGame.PaidUnlocked) {
            HideButtons();
            adBuffer = new Task(ShowAdDelayed());
        }

        if (manageGame.TimerMode != ManageGame.TimerModes.disabled) {
            timerText.transform.parent.gameObject.SetActive(true);

            time = manageGame.TimerSeconds;
            if (manageGame.TimerMode == ManageGame.TimerModes.perPlayer) {
                time *= manageGame.Players.Count;
            }
            StartCoroutine(UpdateTimer());
        } else {
            TimerDisabled();
        }
    }

    private IEnumerator ShowAdDelayed() {
        yield return new WaitForSeconds(5);
        videoAd.StartLoadAd();
    }

    public void CancelAdBuffer() {
        adBuffer?.Stop();
    }

    private void HideButtons() {
        quitButton.SetActive(false);
        doneButton.SetActive(false);
    }

    public void ShowButtons() {
        quitButton.SetActive(true);
        doneButton.SetActive(true);
    }

    private void SetPossibleLocations() {
        string text = "";
        foreach (var locationSet in manageGame.LocationSets.Concat(new List<CustomLocationSet>{ manageGame.CustomSet }).ToList()) {
            foreach (var location in locationSet.Locations) {
                if (location.enabled) {
                    text += $"- {location.Name}\n";
                }
            }
        }
        // remove last new line
        text = text.Remove(text.Length - 1);

        locationsList.text = text;
        // needed to get content fitter to scale properly with content
        content.GetComponent<VerticalLayoutGroup>().spacing += 0.01f;
    }

    private IEnumerator UpdateTimer() {
        yield return 0;
        while (gameplayScreen.activeSelf && time > 0) {
            int minutes = time / 60;
            int seconds = time - minutes * 60;
            timerText.text = $"{minutes:D2}:{seconds:D2}";
            time -= 1;

            yield return new WaitForSecondsRealtime(1f);
            if (time == 0) {
                TimerFinished();
            }
        }
    }

    public void UpdateTimerFromPause() {
        if (gameplayScreen.activeSelf && time > 0) {
            int deltaTime = Mathf.RoundToInt((float)(manageGame.UnpauseTime - manageGame.PauseTime).TotalSeconds);
            time -= deltaTime;
            if (time <= 0) {
                TimerFinished();
            }
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
}

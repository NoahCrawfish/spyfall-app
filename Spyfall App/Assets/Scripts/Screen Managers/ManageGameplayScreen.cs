using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class ManageGameplayScreen : MonoBehaviour
{
    [SerializeField] GameObject gameplayScreen;
    [SerializeField] GameObject content;
    [SerializeField] TextMeshProUGUI locationsList;
    [SerializeField] TextMeshProUGUI timerText;

    private int time;
    private ManageGame manageGame;
#if UNITY_IOS
    iOSNotification notification;
#endif

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

    private void SetPossibleLocations() {
        string text = "";
        foreach (var location in manageGame.Locations.SelectMany(x => x).ToList()) {
            if (location.enabled) {
                text += $"- {location.name}\n";
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
#if UNITY_IOS
        iOSNotificationCenter.ScheduleNotification(notification);
#endif
    }

    private void TimerDisabled() {
        timerText.transform.parent.gameObject.SetActive(false);
    }
}

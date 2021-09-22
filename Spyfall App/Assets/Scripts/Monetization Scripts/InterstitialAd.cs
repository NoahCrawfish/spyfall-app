using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener {
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] string _iOsAdUnitId = "Interstitial_iOS";
    string _adUnitId;
    int reloadAttempts = 0;
    private ManageGameplayScreen manageGameplay;

    void Awake() {
        manageGameplay = FindObjectOfType<ManageGameplayScreen>();

        // Get the Ad Unit ID for the current platform:
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsAdUnitId
            : _androidAdUnitId;
    }


    public void StartLoadAd() {
        reloadAttempts = 0;
        LoadAd();
    }

    // Load content to the Ad Unit:
    private void LoadAd() {
        Advertisement.Load(_adUnitId, this);
    }

    // Show the loaded content in the Ad Unit: 
    private void ShowAd() {
        Advertisement.Show(_adUnitId, this);
    }


    public void OnUnityAdsAdLoaded(string adUnitId) {
        ShowAd();
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message) {
        Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
        AttemptReload();
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message) {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        AttemptReload();
    }

    private void AttemptReload() {
        reloadAttempts++;
        if (reloadAttempts <= 10) {
            LoadAd();
        } else {
            manageGameplay.ShowButtons();
        }
    }

    public void OnUnityAdsShowStart(string adUnitId) {
        manageGameplay.ShowButtons();
    }

    public void OnUnityAdsShowClick(string adUnitId) { }
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState) { }
}
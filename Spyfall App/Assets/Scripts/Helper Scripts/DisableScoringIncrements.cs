using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableScoringIncrements : MonoBehaviour
{
    private Toggle toggle;
    [SerializeField] IncrementController civilianPoints;
    [SerializeField] IncrementController spyPoints;
    [SerializeField] IncrementController lastRoundMultiplier;

    private void Awake() {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn) {
        if (isOn) {
            civilianPoints.Disabled = true;
            spyPoints.Disabled = true;
            lastRoundMultiplier.Disabled = true;
        } else {
            civilianPoints.Disabled = false;
            spyPoints.Disabled = false;
            lastRoundMultiplier.Disabled = false;
        }
    }
}

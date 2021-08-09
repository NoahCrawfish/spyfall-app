using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerModeController : MonoBehaviour
{
    [SerializeField] TimeSelectController timeSelect;
    private readonly List<string> modesText = new List<string>{ "Disabled", "Per Player", "Fixed" };
    public ManageGame.TimerModes Stage { get; set; } = ManageGame.TimerModes.perPlayer;

    public void CycleStage() {
        switch (Stage) {
            case ManageGame.TimerModes.perPlayer:
                Stage = ManageGame.TimerModes.setAmount;
                break;
            case ManageGame.TimerModes.setAmount:
                Stage = ManageGame.TimerModes.disabled;
                break;
            case ManageGame.TimerModes.disabled:
                Stage = ManageGame.TimerModes.perPlayer;
                break;
        }

        SetText();
        timeSelect.TimerModeChanged();
    }

    public void SetText() {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = modesText[(int)Stage];
    }
}

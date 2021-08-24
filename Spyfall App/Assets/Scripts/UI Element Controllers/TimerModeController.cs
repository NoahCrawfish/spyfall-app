using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerModeController : MonoBehaviour
{
    [SerializeField] TimeSelectController timeSelect;
    private readonly List<string> modesText = new List<string>{ "Disabled", "Per Player", "Fixed" };
    private ManageGame.TimerModes stage = ManageGame.TimerModes.perPlayer;
    public ManageGame.TimerModes Stage {
        get { return stage; }
        set {
            stage = value;
            switch (stage) {
                case ManageGame.TimerModes.disabled:
                    timeSelect.Disabled = true;
                    break;
                case ManageGame.TimerModes.perPlayer:
                    timeSelect.Disabled = false;
                    break;
            }
        }
    }

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
    }

    public void SetText() {
        transform.Find("Renderer").GetChild(0).GetComponent<TextMeshProUGUI>().text = modesText[(int)Stage];
    }
}

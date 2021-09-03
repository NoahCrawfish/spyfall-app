using UnityEngine;
using TMPro;

public class ForceAllCaps : MonoBehaviour
{
    private TMP_InputField inputField;

    private void Awake() {
        inputField = GetComponent<TMP_InputField>();
    }

    private void Start() {
        inputField.onValidateInput += delegate (string text, int charIndex, char addedChar) {
            return char.ToUpper(addedChar);
        };
    }
}

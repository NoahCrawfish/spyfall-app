using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonListener : MonoBehaviour
{
    private Button button;
    UnityAction clickFunction;
    [SerializeField] bool sendGameObject;

    private void Awake() {
        button = GetComponent<Button>();
        MethodInfo clickMethod = typeof(HandleButtons).GetMethod(name);

        if (clickMethod != null) {
            object[] args = (sendGameObject) ? new object[] { gameObject } : null;
            clickFunction = () => clickMethod.Invoke(transform.root.GetComponent<HandleButtons>(), args);
        } else {
            clickFunction = NoHandler;
        }
    }

    private void OnEnable() {
        button.onClick.AddListener(clickFunction);
    }

    private void OnDisable() {
        button.onClick.RemoveListener(clickFunction);
    }

    private void NoHandler() {
        Debug.Log($"No handler for {name} button listener.");
    }
}

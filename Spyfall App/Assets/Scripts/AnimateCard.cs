using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimateCard : MonoBehaviour
{
    private const float swipeDistance = 300f;
    private const int framesInAnimation = 12;
    private readonly float[] blurEndpoints = { 0f, 3f };
    private readonly Color[] blurTintEndpoints = { new Color(1, 1, 1), new Color(118f / 255f, 118f / 255f, 118f / 255f)};
    private int cardCount = 0;
    [SerializeField] List<Sprite> framesBeforeAnimation;
    [SerializeField] GameObject blurPanel;
    [SerializeField] TextMeshProUGUI playerPrompt;

    private Vector2 touchStart;
    private Task showCardInfo;

    private Animator animator;
    private Image image;
    private ManageGame manageGame;

    private void Awake() {
        animator = GetComponent<Animator>();
        image = GetComponent<Image>();
        manageGame = FindObjectOfType<ManageGame>();
    }

    private void OnEnable() {
        cardCount = 0;
        UpdatePlayerPrompt();
    }

    private void UpdatePlayerPrompt() {
        playerPrompt.text = $"{manageGame.Players[cardCount].Name}:\nSwipe up";
    }

    private void Update() {
        if (Input.touchCount > 0 && !animator.enabled) {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase) {
                case TouchPhase.Began:
                    touchStart = touch.position;
                    break;
                case TouchPhase.Moved:
                    float deltaY = touch.position.y - touchStart.y;
                    float percentSwiped = deltaY / swipeDistance;
                    int stage = Mathf.FloorToInt(percentSwiped * framesBeforeAnimation.Count);
                    stage = Mathf.Clamp(stage, 0, framesBeforeAnimation.Count - 1);

                    image.sprite = framesBeforeAnimation[stage];
                    if (percentSwiped >= 1) {
                        BeginAnimation();
                    }
                    break;
                case TouchPhase.Ended:
                    image.sprite = framesBeforeAnimation[0];
                    break;
            }
        }

        if (!animator.enabled && Input.GetKeyDown(KeyCode.D)) {
            BeginAnimation();
        }

        blurPanel.SetActive(animator.enabled);
        if (blurPanel.activeSelf && showCardInfo == null) {
            Material blur = blurPanel.GetComponent<Image>().material;
            float animationProgress = Mathf.Clamp(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, float.MinValue, 1);
            blur.SetFloat("_Size", Mathf.Lerp(blurEndpoints[0], blurEndpoints[1], animationProgress));
            blur.SetColor("_MultiplyColor", LerpColor(blurTintEndpoints[0], blurTintEndpoints[1], animationProgress));

            if (animationProgress == 1) {
                showCardInfo = new Task(ShowCardInfo());
            }
        }
    }

    private void BeginAnimation() {
        animator.enabled = true;
        animator.Play("CardFlip", -1, (float)framesBeforeAnimation.Count / framesInAnimation);
    }

    private Color LerpColor(Color start, Color finish, float t) {
        Vector4 startRGBA = new Vector4(start.r, start.g, start.b, start.a);
        Vector4 finishRGBA = new Vector4(finish.r, finish.g, finish.b, finish.a);
        Vector4 newColor = Vector4.Lerp(startRGBA, finishRGBA, t);
        return new Color(newColor[0], newColor[1], newColor[2], newColor[3]);
    }

    private IEnumerator ShowCardInfo() {
        Texture2D cardImage = manageGame.CurrentLocation.GetImage();
        Debug.Log(cardImage);
        yield return new WaitForSeconds(3);
    }
}

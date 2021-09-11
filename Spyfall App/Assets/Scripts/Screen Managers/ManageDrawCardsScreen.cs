using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManageDrawCardsScreen : MonoBehaviour
{
    private const float swipeDistance = 300f;
    private const int framesInAnimation = 12;
    private readonly float[] blurEndpoints = { 0f, 3f };
    private readonly Color[] blurTintEndpoints = { new Color(1, 1, 1), new Color(118f / 255f, 118f / 255f, 118f / 255f)};
    private const float blurSpeed = 0.05f;
    private const float cardInfoFadeSpeed = 0.0275f;
    private const float cardFadeOutSpeed = 0.1f;

    [SerializeField] GameObject drawCardsScreen;
    [SerializeField] List<Sprite> framesBeforeAnimation;
    [SerializeField] GameObject card;
    [SerializeField] GameObject blurPanel;
    [SerializeField] TextMeshProUGUI playerPrompt;
    [SerializeField] GameObject promptTip;
    public GameObject nextCardButton;
    [SerializeField] GameObject cardInfo;
    [SerializeField] Image cardInfoImage;
    [SerializeField] TextMeshProUGUI cardInfoText;
    [SerializeField] Sprite spySprite;

    private Vector2 touchStart;
    public int CardCount { get; private set; }
    private delegate void AfterEnumerator();

    private Animator animator;
    private Image image;
    private ManageGame manageGame;

    private void Awake() {
        animator = card.GetComponent<Animator>();
        image = card.GetComponent<Image>();
        manageGame = FindObjectOfType<ManageGame>();
    }


    //called when the panel is first transitioned to
    public void InitializeScreen() {
        CardCount = 0;
        UpdatePrompts();
        ResetCardAnimation();
        StartCoroutine(CheckTouch());
    }

    public IEnumerator CheckTouch() {
        while (drawCardsScreen.activeSelf) {
            yield return 0;
            if (Input.touchCount > 0 && !animator.enabled) {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase) {
                    case TouchPhase.Began:
                        touchStart = touch.position;
                        break;
                    // card sprite is controlled by swipe distance, with the animation playing after a certain point
                    case TouchPhase.Moved:
                        float deltaY = touch.position.y - touchStart.y;
                        float percentSwiped = deltaY / swipeDistance;
                        int stage = Mathf.FloorToInt(percentSwiped * framesBeforeAnimation.Count);
                        stage = Mathf.Clamp(stage, 0, framesBeforeAnimation.Count - 1);

                        image.sprite = framesBeforeAnimation[stage];
                        if (percentSwiped >= 1) {
                            BeginCardAnimation();
                        }
                        break;
                    case TouchPhase.Ended:
                        image.sprite = framesBeforeAnimation[0];
                        break;
                }
            }

            // computer debugging
            if (!animator.enabled && Input.GetKeyDown(KeyCode.D)) {
                BeginCardAnimation();
            }
        }
    }


    private void BeginCardAnimation() {
        animator.enabled = true;
        animator.Play("CardFlip", -1, (float)framesBeforeAnimation.Count / framesInAnimation);
        blurPanel.SetActive(true);
        // blur background and show card info afterwards
        StartCoroutine(BlurFade(true, blurSpeed, ShowCardInfo));
    }

    private void ShowCardInfo() {
        string currentRole = manageGame.Players[CardCount].Role;
        Texture2D locationTexture = manageGame.CurrentLocation.GetImage();
        Sprite locationSprite = Sprite.Create(locationTexture, new Rect(0, 0, locationTexture.width, locationTexture.height), new Vector2(0.5f, 0.5f));
        cardInfoImage.sprite = (currentRole == "Spy") ? spySprite : locationSprite;
        cardInfoText.text = (currentRole == "Spy") ? "You are the spy." : $"{manageGame.CurrentLocation.Name}\nRole: {currentRole}";

        // fade in card info
        StartCoroutine(CanvasGroupFade(true, cardInfo.GetComponent<CanvasGroup>(), cardInfoFadeSpeed, () => nextCardButton.gameObject.SetActive(true)));
    }

    public void NextCard() {
        nextCardButton.gameObject.SetActive(false);
        CardCount += 1;
        UpdatePrompts();

        // fade out card and info
        StartCoroutine(BlurFade(false, cardFadeOutSpeed, ResetCardAnimation));
        StartCoroutine(CanvasGroupFade(false, card.GetComponent<CanvasGroup>(), cardFadeOutSpeed));
    }


    private IEnumerator CanvasGroupFade(bool fadeIn, CanvasGroup canvasGroup, float speed, AfterEnumerator callAfter = null) {
        float alpha = fadeIn ? 0 : 1;
        while (fadeIn ? alpha < 1 : alpha > 0) {
            alpha = Mathf.Clamp(fadeIn ? alpha + speed : alpha - speed, 0, 1);
            canvasGroup.alpha = alpha;
            yield return 0;
        }
        callAfter?.Invoke();
    }

    private IEnumerator BlurFade(bool fadeIn, float speed, AfterEnumerator callAfter = null) {
        float alpha = fadeIn ? 0 : 1;
        Material blur = blurPanel.GetComponent<Image>().material;
        while (fadeIn ? alpha < 1 : alpha > 0) {
            alpha = Mathf.Clamp(fadeIn ? alpha + speed : alpha - speed, 0, 1);
            blur.SetFloat("_Size", Mathf.Lerp(blurEndpoints[0], blurEndpoints[1], alpha));
            blur.SetColor("_MultiplyColor", LerpColor(blurTintEndpoints[0], blurTintEndpoints[1], alpha));
            yield return 0;
        }
        callAfter?.Invoke();
    }


    private void UpdatePrompts() {
        playerPrompt.text = $"{manageGame.Players[CardCount].Name}:\nSwipe up";
        // ensures that the tip is visible at the start of each new card
        promptTip.GetComponent<FlashText>().ResetT();
    }

    private void ResetCardAnimation() {
        animator.enabled = false;
        blurPanel.SetActive(false);
        image.sprite = framesBeforeAnimation[0];
        card.GetComponent<CanvasGroup>().alpha = 1;
        cardInfo.GetComponent<CanvasGroup>().alpha = 0f;
    }

    private Color LerpColor(Color start, Color finish, float t) {
        Vector4 startRGBA = new Vector4(start.r, start.g, start.b, start.a);
        Vector4 finishRGBA = new Vector4(finish.r, finish.g, finish.b, finish.a);
        Vector4 newColor = Vector4.Lerp(startRGBA, finishRGBA, t);
        return new Color(newColor[0], newColor[1], newColor[2], newColor[3]);
    }
}

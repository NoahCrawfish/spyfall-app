using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using TMPro;

public class ManageDrawCardsScreen : MonoBehaviour
{
    private const float swipeDistance = 300f;
    private const int framesInAnimation = 12;

    [SerializeField] GameObject drawCardsScreen;
    [SerializeField] List<Sprite> framesBeforeAnimation;
    [SerializeField] GameObject card;
    public GameObject blurPanel;
    [SerializeField] TextMeshProUGUI playerPrompt;
    [SerializeField] TextMeshProUGUI playerPromptShadow1;
    [SerializeField] TextMeshProUGUI playerPromptShadow2;
    [SerializeField] GameObject promptTip;
    public GameObject nextCardButton;
    [SerializeField] GameObject cardInfo;
    [SerializeField] ProceduralImage cardInfoImage;
    [SerializeField] TextMeshProUGUI cardInfoText;
    [SerializeField] Sprite spySprite;

    private Vector2 touchStart;
    public int CardCount { get; private set; }

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
            #if UNITY_EDITOR
                if (!animator.enabled && Input.GetKeyDown(KeyCode.D)) {
                    BeginCardAnimation();
                }
            #endif
        }
    }


    private void BeginCardAnimation() {
        animator.enabled = true;
        animator.Play("CardFlip", -1, (float)framesBeforeAnimation.Count / framesInAnimation);

        blurPanel.SetActive(true);
        Task scaleCard = new Task(ScaleCard());
        blurPanel.GetComponent<BlurController>().DoBlur(BlurController.Fade.fadeIn, () => ShowCardInfo(scaleCard));

        ManageAudio.Instance.Play("flip");
    }

    private IEnumerator ScaleCard() {
        // use for loop with i as a fail-safe stop
        for (int i = 0; i < 50; i++) {
            float t = blurPanel.GetComponent<BlurController>().Alpha;
            card.GetComponent<RectTransform>().localScale = Vector3.one * Mathf.SmoothStep(1f, 1.25f, t);
            yield return new WaitForEndOfFrame();
        }
    }

    private void ShowCardInfo(Task scaleCard) {
        scaleCard.Stop();

        string currentRole = manageGame.Players[CardCount].Role;
        Texture2D locationTexture = manageGame.CurrentLocation.GetImage();
        Sprite locationSprite = Sprite.Create(locationTexture, new Rect(0, 0, locationTexture.width, locationTexture.height), new Vector2(0.5f, 0.5f));
        cardInfoImage.sprite = (currentRole == "Spy") ? spySprite : locationSprite;
        cardInfoText.text = (currentRole == "Spy") ? "You are the spy." : $"{manageGame.CurrentLocation.Name}\nRole: {currentRole}";

        ManageAudio.Instance.Play("photo");
        // fade in card info
        cardInfo.GetComponent<CanvasGroupFade>().DoFade(CanvasGroupFade.Fade.fadeIn, () => nextCardButton.gameObject.SetActive(true));
    }

    public void NextCard() {
        nextCardButton.gameObject.SetActive(false);
        CardCount += 1;
        UpdatePrompts();

        // reanimate playerPrompt bounce
        playerPrompt.transform.parent.GetComponent<ScaleBounceTransition>().Reanimate();

        // fade out card and info
        blurPanel.GetComponent<BlurController>().DoBlur(BlurController.Fade.fadeOut, ResetCardAnimation);
        card.GetComponent<CanvasGroupFade>().DoFade(CanvasGroupFade.Fade.fadeOut);
    }

    private void UpdatePrompts() {
        playerPrompt.text = playerPromptShadow1.text = playerPromptShadow2.text = $"{manageGame.Players[CardCount].Name}:\nSwipe up";
        // ensures that the tip is visible at the start of each new card
        promptTip.GetComponent<FlashText>().ResetT();
    }

    private void ResetCardAnimation() {
        animator.enabled = false;
        blurPanel.SetActive(false);
        image.sprite = framesBeforeAnimation[0];
        card.GetComponent<CanvasGroup>().alpha = 1;
        card.GetComponent<RectTransform>().localScale = Vector3.one;
        cardInfo.GetComponent<CanvasGroup>().alpha = 0f;
    }
}

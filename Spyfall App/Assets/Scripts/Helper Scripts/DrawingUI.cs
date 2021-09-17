using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawingUI : MonoBehaviour {
    public static readonly Vector2Int imagePixels = new Vector2Int(800, 800);
    [SerializeField] private int brushSize;
    [SerializeField] private BrushTypes brushType;

    [SerializeField] private List<GameObject> selectionBorders = new List<GameObject>();
    [SerializeField] Camera cam;
    [SerializeField] GameObject drawingCanvas;

    public bool CanDraw { get; set; } = false;

    private Texture2D drawing;
    public Texture2D Drawing {
        get => drawing;
        set {
            drawing = value;
            drawingCanvas.GetComponent<Image>().sprite = Sprite.Create(value, new Rect(0, 0, value.width, value.height), new Vector2(value.width / 2, value.height / 2));
        }
    }

    private Colors currentColor;
    public Colors CurrentColor {
        get => currentColor;
        set {
            currentColor = value;
            SwitchColorSelection(value);
        }
    }

    private Vector2Int previousTexturePos;
    private bool[] modifiedPixels;

    public enum Colors {
        black,
        blue,
        orange,
        white
    }

    public enum BrushTypes {
        square,
        round
    }

    public static Dictionary<Colors, Color32> EnumToColor = new Dictionary<Colors, Color32>() {
        { Colors.white, new Color32(215, 205, 185, 255) },
        { Colors.orange, new Color32(232, 123, 28, 255) },
        { Colors.black, new Color32(47, 47, 45, 255) },
        { Colors.blue, new Color32(79, 134, 127, 255) }
    };

    private void Awake() {
        Drawing = new Texture2D(imagePixels.x, imagePixels.y);
    }

    private void OnEnable() {
        CurrentColor = Colors.black;
        previousTexturePos.Reset();
        modifiedPixels = new bool[imagePixels.x * imagePixels.y];
    }

    private void SwitchColorSelection(Colors color) {
        selectionBorders.ForEach(border => border.SetActive(selectionBorders.IndexOf(border) == (int)color));
    }

    public void CreateBlankImage() {
        Color32 fillColor = EnumToColor[Colors.white];
        Color32[] fillColorArray = new Color32[Drawing.width * Drawing.height];
        for (int i = 0; i < fillColorArray.Length; i++) {
            fillColorArray[i] = fillColor;
        }
        Drawing.SetPixels32(fillColorArray);
        Drawing.Apply();
    }

    // touch controlled drawing
    /*private void Update() {
        if (Input.touchCount > 0) {
            RectTransform drawingRect = drawingCanvas.GetComponent<RectTransform>();
            Vector3[] drawingRectCorners = new Vector3[4];
            drawingRect.GetWorldCorners(drawingRectCorners);
            Vector2 drawingTopLeft = cam.WorldToScreenPoint(drawingRectCorners[1]);

            Vector2 clickPos = (Input.GetTouch(0).position - drawingTopLeft) * new Vector2(1, -1);
            Vector2 normalizedClickPos = new Vector2(clickPos.x / drawingRect.sizeDelta.x, clickPos.y / drawingRect.sizeDelta.y);
            Vector2Int texturePos = new Vector2(normalizedClickPos.x * imagePixels.x, normalizedClickPos.y * imagePixels.y).RoundToInt();

            if (previousTexturePos.IsReset()) {
                previousTexturePos = texturePos;
            }
            if (texturePos.WithinDrawing() && CanDraw) {
                ColorBetween(previousTexturePos, texturePos);
            }

            previousTexturePos = texturePos;
        } else {
            previousTexturePos.Reset();
        }
    }*/

    // computer drawing
    private void Update() {
        if (Input.GetMouseButtonUp(0)) {
            modifiedPixels = new bool[imagePixels.x * imagePixels.y];
        }

        if (Input.GetMouseButton(0)) {
            RectTransform drawingRect = drawingCanvas.GetComponent<RectTransform>();
            Vector3[] drawingRectCorners = new Vector3[4];
            drawingRect.GetWorldCorners(drawingRectCorners);
            Vector2 drawingTopLeft = cam.WorldToScreenPoint(drawingRectCorners[1]);

            Vector2 clickPos = ((Vector2)Input.mousePosition - drawingTopLeft) * new Vector2(1, -1);
            Vector2 normalizedClickPos = new Vector2(clickPos.x / drawingRect.sizeDelta.x, clickPos.y / drawingRect.sizeDelta.y);
            Vector2Int texturePos = new Vector2(normalizedClickPos.x * imagePixels.x, normalizedClickPos.y * imagePixels.y).RoundToInt();

            if (previousTexturePos.IsReset()) {
                previousTexturePos = texturePos;
            }
            if (texturePos.WithinDrawing() && CanDraw) {
                ColorBetween(previousTexturePos, texturePos);
            }

            previousTexturePos = texturePos;
        } else {
            previousTexturePos.Reset();
        }
    }

    private void ColorBetween(Vector2Int startPoint, Vector2Int endPoint) {
        Color32[] currentImage = Drawing.GetPixels32();
        float distance = Vector2.Distance(startPoint, endPoint);

        if (distance > 0f) {

            float stepSize = 1f / distance;
            for (float t = 0; t <= 1; t += stepSize) {
                Vector2Int point = Vector2.Lerp(startPoint, endPoint, t).RoundToInt();
                if (point.WithinDrawing()) {
                    ColorArea(point, ref currentImage);
                }
            }

        } else {
            ColorArea(endPoint, ref currentImage);
        }

        Drawing.SetPixels32(currentImage);
        Drawing.Apply();
    }

    private void ColorArea(Vector2Int origin, ref Color32[] currentImage) {
        for (int x = origin.x - brushSize; x <= origin.x + brushSize; x++) {
            for (int y = origin.y - brushSize; y <= origin.y + brushSize; y++) {
                Vector2Int point = new Vector2Int(x, y);

                if (point.WithinDrawing()) {
                    int flatPoint = point.FlattenPoint();
                    try {
                        if (!modifiedPixels[flatPoint]) {
                            bool squareQualifier = brushType == BrushTypes.square;
                            bool roundQualifier = brushType == BrushTypes.round && Vector2.Distance(point, origin) <= brushSize;

                            if (squareQualifier || roundQualifier) {
                                currentImage[flatPoint] = EnumToColor[CurrentColor];
                                modifiedPixels[flatPoint] = true;
                            }
                        }
                    } catch (System.IndexOutOfRangeException) {
                        Debug.Log(flatPoint);
                    }
                }
                
            }
        }
    }
}

public static class DrawingExtensions {
    static Vector2Int resetValue = -Vector2Int.one;

    public static void Reset(this ref Vector2Int input) {
        input = resetValue;
    }

    public static bool IsReset(this Vector2Int input) {
        return input == resetValue;
    }

    public static Vector2Int RoundToInt(this Vector2 input) {
        return new Vector2Int(Mathf.RoundToInt(input.x), Mathf.RoundToInt(input.y));
    }

    public static bool WithinDrawing(this Vector2Int point) {
        return 0 <= point.x && point.x < DrawingUI.imagePixels.x && 0 <= point.y && point.y < DrawingUI.imagePixels.y;
    }

    public static int FlattenPoint(this Vector2Int point) {
        // texture coordinates start at the lower left of the texture
        return DrawingUI.imagePixels.x * (DrawingUI.imagePixels.y - point.y - 1) + point.x;
    }
}

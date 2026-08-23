using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawingCanvas : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public enum ToolType { Pencil, Eraser, Fill }

    public int textureWidth = 512;
    public int textureHeight = 512;
    public Color brushColor = Color.black;
    public Color eraserColor = Color.white;
    public int brushSize = 5;

    public Toggle pencilToggle;
    public Toggle eraserToggle;
    public Toggle fillToggle;

    private RawImage targetImage;
    private Texture2D drawingTexture;
    private Color32[] pixels;
    
    private ToolType currentTool = ToolType.Pencil;

    private Vector2Int lastPos = new Vector2Int(-1, -1); 

    void Start()
    {
        targetImage = gameObject.GetComponent<RawImage>();
        drawingTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        pixels = new Color32[textureWidth * textureHeight];
        
        ClearCanvas(eraserColor);
        targetImage.texture = drawingTexture;

        pencilToggle.onValueChanged.AddListener(isOn => { if (isOn) SelectTool(ToolType.Pencil); });
        eraserToggle.onValueChanged.AddListener(isOn => { if (isOn) SelectTool(ToolType.Eraser); });
        fillToggle.onValueChanged.AddListener(isOn => { if (isOn) SelectTool(ToolType.Fill); });

        SelectTool(ToolType.Pencil);
    }

    public void SelectTool(ToolType tool)
    {
        currentTool = tool;
        pencilToggle.SetIsOnWithoutNotify(tool == ToolType.Pencil);
        eraserToggle.SetIsOnWithoutNotify(tool == ToolType.Eraser);
        fillToggle.SetIsOnWithoutNotify(tool == ToolType.Fill);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ProcessInput(eventData, isClick: true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ProcessInput(eventData, isClick: false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        lastPos = new Vector2Int(-1, -1);
    }

    void ProcessInput(PointerEventData eventData, bool isClick)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            targetImage.rectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out localPoint
        );

        Rect rect = targetImage.rectTransform.rect;
        float normalizedX = (localPoint.x - rect.x) / rect.width;
        float normalizedY = (localPoint.y - rect.y) / rect.height;

        if (normalizedX < 0f || normalizedX > 1f || normalizedY < 0f || normalizedY > 1f)
        {
            lastPos = new Vector2Int(-1, -1);
            return;
        }

        int x = Mathf.RoundToInt(normalizedX * textureWidth);
        int y = Mathf.RoundToInt(normalizedY * textureHeight);
        Vector2Int currentPos = new Vector2Int(x, y);

        if (currentTool == ToolType.Fill)
        {
            if (isClick) FloodFill(x, y);
        }
        else
        {
            Color32 activeColor = currentTool == ToolType.Eraser ? eraserColor : brushColor;

            if (isClick || lastPos.x == -1)
            {
                PaintBrush(x, y, activeColor);
            }
            else
            {
                DrawLine(lastPos, currentPos, activeColor);
            }
        }

        lastPos = currentPos;

        drawingTexture.SetPixels32(pixels);
        drawingTexture.Apply();
    }

    void DrawLine(Vector2Int start, Vector2Int end, Color32 color)
    {
        float distance = Vector2Int.Distance(start, end);
        for (int i = 0; i <= distance; i++)
        {
            float t = distance == 0 ? 0 : i / distance;
            int x = Mathf.RoundToInt(Mathf.Lerp(start.x, end.x, t));
            int y = Mathf.RoundToInt(Mathf.Lerp(start.y, end.y, t));
            PaintBrush(x, y, color);
        }
    }

    void PaintBrush(int cx, int cy, Color32 paintColor)
    {
        for (int x = -brushSize; x <= brushSize; x++)
        {
            for (int y = -brushSize; y <= brushSize; y++)
            {
                int px = cx + x;
                int py = cy + y;

                if (px >= 0 && px < textureWidth && py >= 0 && py < textureHeight)
                {
                    pixels[py * textureWidth + px] = paintColor;
                }
            }
        }
    }

    void FloodFill(int startX, int startY)
    {
        Color32 targetColor = pixels[startY * textureWidth + startX];
        Color32 replacementColor = brushColor;

        if (IsSameColor(targetColor, replacementColor)) return;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        
        pixels[startY * textureWidth + startX] = replacementColor;

        while (queue.Count > 0)
        {
            Vector2Int p = queue.Dequeue();
            int x = p.x;
            int y = p.y;

            if (x > 0 && IsSameColor(pixels[y * textureWidth + (x - 1)], targetColor))
            {
                pixels[y * textureWidth + (x - 1)] = replacementColor;
                queue.Enqueue(new Vector2Int(x - 1, y));
            }
            if (x < textureWidth - 1 && IsSameColor(pixels[y * textureWidth + (x + 1)], targetColor))
            {
                pixels[y * textureWidth + (x + 1)] = replacementColor;
                queue.Enqueue(new Vector2Int(x + 1, y));
            }
            if (y > 0 && IsSameColor(pixels[(y - 1) * textureWidth + x], targetColor))
            {
                pixels[(y - 1) * textureWidth + x] = replacementColor;
                queue.Enqueue(new Vector2Int(x, y - 1));
            }
            if (y < textureHeight - 1 && IsSameColor(pixels[(y + 1) * textureWidth + x], targetColor))
            {
                pixels[(y + 1) * textureWidth + x] = replacementColor;
                queue.Enqueue(new Vector2Int(x, y + 1));
            }
        }
    }

    bool IsSameColor(Color32 c1, Color32 c2)
    {
        return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b && c1.a == c2.a;
    }

    public void ClearCanvas(Color backgroundColor)
    {
        Color32 bg = backgroundColor;
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = bg;
        }
        drawingTexture.SetPixels32(pixels);
        drawingTexture.Apply();
    }
}
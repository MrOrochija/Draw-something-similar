using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RawImage))]
public class ColorPicker : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public DrawingCanvas drawableCanvas;
    public BrightnessSlider brightnessSlider;

    public Toggle eraserToggle; 

    private RawImage rawImage;
    private RectTransform rectTransform;
    private Texture2D colorTexture;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();
        colorTexture = rawImage.texture as Texture2D;
    }

    public void OnPointerDown(PointerEventData eventData) => PickColor(eventData);
    public void OnDrag(PointerEventData eventData) => PickColor(eventData);

    private void PickColor(PointerEventData eventData)
    {
        if (colorTexture == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            if (eraserToggle != null && eraserToggle.isOn)
            {
                eraserToggle.isOn = false;
            }

            Rect rect = rectTransform.rect;
            float px = (localPoint.x - rect.x) / rect.width * colorTexture.width;
            float py = (localPoint.y - rect.y) / rect.height * colorTexture.height;

            Color rawColor = colorTexture.GetPixel((int)px, (int)py);

            Color actualColor = rawColor * rawImage.color;
            actualColor.a = rawColor.a;

            if (brightnessSlider != null)
            {
                brightnessSlider.SetBaseColor(actualColor);
            }
            else if (drawableCanvas != null)
            {
                drawableCanvas.brushColor = actualColor;
            }
        }
    }
}
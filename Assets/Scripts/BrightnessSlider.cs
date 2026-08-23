using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class BrightnessSlider : MonoBehaviour
{
    public DrawingCanvas drawableCanvas;

    public RawImage colorWheelRawImage;

    public GameObject brightnessSliderBackground;

    private Slider slider;
    private Color baseColor = Color.white;

    void Awake()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    public void SetBaseColor(Color newColor)
    {
        Color.RGBToHSV(newColor, out float h, out float s, out float v);

        baseColor = Color.HSVToRGB(h, s, 1.0f);
        baseColor.a = newColor.a;

        ApplyColorToBrush();
    }

    private void OnSliderChanged(float value)
    {
        ApplyColorToBrush();

        if (colorWheelRawImage != null)
        {
            colorWheelRawImage.color = new Color(value, value, value, 1.0f);
        }
    }

    private void ApplyColorToBrush()
    {
        if (drawableCanvas == null) return;

        Color.RGBToHSV(baseColor, out float h, out float s, out float v);

        Color finalColor = Color.HSVToRGB(h, s, slider.value);
        finalColor.a = baseColor.a;

        drawableCanvas.brushColor = finalColor;

        if (brightnessSliderBackground != null)
        {
            Image bgImage = brightnessSliderBackground.GetComponent<Image>();
            if (bgImage != null)
            {
                Color invertedBgColor = Color.HSVToRGB(h, s, 1f - slider.value);
                invertedBgColor.a = baseColor.a;
                
                bgImage.color = invertedBgColor;
            }
        }
    }
}
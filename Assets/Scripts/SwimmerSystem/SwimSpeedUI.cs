using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SwimSpeedUI : MonoBehaviour
{
    public SwimmerController swimmer;
    public Slider speedSlider;
    public TMP_Text speedLabel;

    void Start()
    {
        if (!swimmer || !speedSlider) return;
        speedSlider.onValueChanged.AddListener(SetSpeed);
        SetSpeed(speedSlider.value);
    }

    void SetSpeed(float v)
    {
        swimmer.ApplySpeedScale(v);
        if (speedLabel) speedLabel.text = $"Speed x{v:0.00}";
    }
}

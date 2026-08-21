using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SwimUI : MonoBehaviour
{
    [Header("Refs")]
    public SwimmerController swimmer;
    public StrokeTutorialUI tutorial;   // <- assign TutorialPanel here in Inspector

    [Header("UI")]
    public Button freestyleBtn, backstrokeBtn, breaststrokeBtn, butterflyBtn;
    public Slider speedSlider;
    public TMP_Text speedLabel;

    void Start()
    {
        // Wire buttons + open tutorial
        if (freestyleBtn)  freestyleBtn.onClick.AddListener(() => { swimmer.ApplyStroke(SwimStroke.Freestyle);   if (tutorial) tutorial.ShowFor(SwimStroke.Freestyle); });
        if (backstrokeBtn) backstrokeBtn.onClick.AddListener(() => { swimmer.ApplyStroke(SwimStroke.Backstroke);  if (tutorial) tutorial.ShowFor(SwimStroke.Backstroke); });
        if (breaststrokeBtn)breaststrokeBtn.onClick.AddListener(() => { swimmer.ApplyStroke(SwimStroke.Breaststroke); if (tutorial) tutorial.ShowFor(SwimStroke.Breaststroke); });
        if (butterflyBtn)   butterflyBtn.onClick.AddListener(() => { swimmer.ApplyStroke(SwimStroke.Butterfly);   if (tutorial) tutorial.ShowFor(SwimStroke.Butterfly); });

        // Speed slider (scales animation + movement if you wired ApplySpeedScale on SwimmerController)
        if (speedSlider)
        {
            speedSlider.onValueChanged.AddListener(SetSpeed);
            speedSlider.minValue = 0.25f;
            speedSlider.maxValue = 1.25f;
            speedSlider.value = 1f;
            UpdateSpeedLabel(speedSlider.value);
        }
    }

    void SetSpeed(float m)
    {
        if (!swimmer) return;
        swimmer.ApplySpeedScale(m);          // uses your SwimmerController method
        UpdateSpeedLabel(m);
    }

    void UpdateSpeedLabel(float m)
    {
        if (speedLabel) speedLabel.text = $"Speed x{m:0.00}";
    }
}

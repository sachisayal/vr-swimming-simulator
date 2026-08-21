using UnityEngine;
using UnityEngine.UI;

public class HandRadiusSlider : MonoBehaviour
{
    public FreestyleStrokeEvaluator evaluator;
    public bool isLeftArm = true;
    public Slider slider;

    private void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        if (slider != null)
            slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnDisable()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void Start()
    {
        if (evaluator != null && slider != null)
        {
            float initial = isLeftArm
                ? evaluator.leftArm.radiusScale
                : evaluator.rightArm.radiusScale;

            slider.value = initial;
        }
    }

    private void OnSliderValueChanged(float value)
    {
        if (evaluator == null) return;

        if (isLeftArm)
            evaluator.SetLeftArmRadiusScale(value);
        else
            evaluator.SetRightArmRadiusScale(value);
    }
}

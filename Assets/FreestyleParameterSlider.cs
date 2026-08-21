using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FreestyleParameterSlider : MonoBehaviour
{
    public enum Parameter
    {
        RadiusForward,
        RadiusDown,
        CenterForwardOffset,
        EllipseRotationDegrees,
        AcceptableError,
        PhaseToleranceDegrees,
        RequiredGoodPhaseTime,
        LeftRadiusScale,
        RightRadiusScale
    }

    [Header("References")]
    public FreestyleStrokeEvaluator evaluator;
    public Parameter parameter;
    public Slider slider;

    [Header("Label")]
    [Tooltip("Optional: prefix used before the value, e.g. 'Phase Tolerance'.")]
    public string labelPrefix;               // e.g. "Phase Tolerance"
    public TextMeshProUGUI valueLabel;       // text to update

    [Tooltip("Numeric format string for the value.")]
    public string format = "0.00";           // e.g. "0", "0.0", "0.00"

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
        if (evaluator == null || slider == null) return;

        float initial = GetCurrentValue();
        slider.value = initial;
        UpdateLabel(initial);
    }

    private void OnSliderValueChanged(float value)
    {
        if (evaluator == null) return;

        SetCurrentValue(value);
        UpdateLabel(value);
    }

    float GetCurrentValue()
    {
        if (evaluator == null) return 0f;

        switch (parameter)
        {
            case Parameter.RadiusForward:
                return evaluator.radiusForward;

            case Parameter.RadiusDown:
                return evaluator.radiusDown;

            case Parameter.CenterForwardOffset:
                return evaluator.centerForwardOffset;

            case Parameter.EllipseRotationDegrees:
                return evaluator.ellipseRotationDegrees;

            case Parameter.AcceptableError:
                return evaluator.acceptableError;

            case Parameter.PhaseToleranceDegrees:
                return evaluator.phaseToleranceDegrees;

            case Parameter.RequiredGoodPhaseTime:
                return evaluator.requiredGoodPhaseTime;

            case Parameter.LeftRadiusScale:
                return evaluator.leftArm != null ? evaluator.leftArm.radiusScale : 1f;

            case Parameter.RightRadiusScale:
                return evaluator.rightArm != null ? evaluator.rightArm.radiusScale : 1f;
        }

        return 0f;
    }

    void SetCurrentValue(float value)
    {
        if (evaluator == null) return;

        switch (parameter)
        {
            case Parameter.RadiusForward:
                evaluator.radiusForward = value;
                break;

            case Parameter.RadiusDown:
                evaluator.radiusDown = value;
                break;

            case Parameter.CenterForwardOffset:
                evaluator.centerForwardOffset = value;
                break;

            case Parameter.EllipseRotationDegrees:
                evaluator.ellipseRotationDegrees = value;
                break;

            case Parameter.AcceptableError:
                evaluator.acceptableError = value;
                break;

            case Parameter.PhaseToleranceDegrees:
                evaluator.phaseToleranceDegrees = value;
                break;

            case Parameter.RequiredGoodPhaseTime:
                evaluator.requiredGoodPhaseTime = value;
                break;

            case Parameter.LeftRadiusScale:
                if (evaluator.leftArm != null)
                    evaluator.leftArm.radiusScale = value;
                break;

            case Parameter.RightRadiusScale:
                if (evaluator.rightArm != null)
                    evaluator.rightArm.radiusScale = value;
                break;
        }
    }

    void UpdateLabel(float value)
    {
        if (valueLabel == null) return;

        string prefix = string.IsNullOrEmpty(labelPrefix)
            ? parameter.ToString()
            : labelPrefix;

        valueLabel.text = $"{prefix}: {value.ToString(format)}";
    }
}

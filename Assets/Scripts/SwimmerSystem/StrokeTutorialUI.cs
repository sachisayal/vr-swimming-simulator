using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StrokeTutorialUI : MonoBehaviour
{
    [Header("Refs")]
    public SwimmerController swimmer;
    public TMP_Text title;
    public TMP_Text stepText;
    public Button prevBtn, nextBtn, closeBtn;

    SwimStroke _stroke;
    string[] _steps;
    int _i;
    float _savedAnimSpeed;
    bool _savedRunning;

    void Awake()
    {
        gameObject.SetActive(false);
        if (prevBtn) prevBtn.onClick.AddListener(() => SetIndex(_i - 1));
        if (nextBtn) nextBtn.onClick.AddListener(() => SetIndex(_i + 1));
        if (closeBtn) closeBtn.onClick.AddListener(Close);
    }

    public void ShowFor(SwimStroke stroke)
    {
        _stroke = stroke;
        _steps = StrokeTutorials.Steps[stroke];
        _i = 0;
        if (title) title.text = $"{stroke} Tutorial";
        SetIndex(0);

        // Pause motion & animation
        if (swimmer)
        {
            _savedRunning = swimmer.running;
            swimmer.SetRunning(false);
            var anim = swimmer.GetComponentInChildren<Animator>();
            if (anim) { _savedAnimSpeed = anim.speed; anim.speed = 0f; }
        }

        gameObject.SetActive(true);
    }

    void SetIndex(int idx)
    {
        if (_steps == null || _steps.Length == 0) return;
        _i = Mathf.Clamp(idx, 0, _steps.Length - 1);
        if (stepText) stepText.text = $"{_i + 1}/{_steps.Length}: {_steps[_i]}";
        if (prevBtn) prevBtn.interactable = _i > 0;
        if (nextBtn) nextBtn.interactable = _i < _steps.Length - 1;
    }

    void Close()
    {
        gameObject.SetActive(false);
        if (swimmer)
        {
            swimmer.SetRunning(_savedRunning);
            var anim = swimmer.GetComponentInChildren<Animator>();
            if (anim) anim.speed = swimmer.speedScale; // resume at current slider speed
        }
    }
}

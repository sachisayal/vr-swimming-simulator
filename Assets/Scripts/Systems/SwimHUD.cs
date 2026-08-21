using UnityEngine;
using TMPro;

public class SwimHUD : MonoBehaviour
{
    public SwimTrainer trainer;
    public TextMeshProUGUI statsText;

    void Update()
    {
        if (trainer == null || statsText == null) return;
        float dist = trainer.DistanceMeters;
        float spm = trainer.StrokeRateSPM;
        int strokes = trainer.Strokes;
        string time = trainer.TimeMMSS();
        float pace = (dist > 0.1f && trainer.SessionTime > 0.1f)
            ? (trainer.SessionTime / 60f) / (dist / 25f) // min per 25 m (pool-ish)
            : 0f;

        statsText.text =
            $"Time   {time}\n" +
            $"Dist   {dist:0.0} m\n" +
            $"Stk    {strokes}  ({spm:0} spm)\n" +
            $"Pace   {(pace>0? pace:0):0.0} min/25m";
    }
}

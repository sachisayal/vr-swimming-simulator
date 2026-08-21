using UnityEngine;
using UnityEngine.Rendering;

public class BreathManager : MonoBehaviour
{
    [Header("Refs")]
    public Transform head;                  // Main Camera
    public float waterLevelY = 0f;          // same as SwimLocomotion
    public Volume hypoxiaVolume;            // the Hypoxia Volume
    public AudioLowPassFilter lowpass;      // on Main Camera
    public AudioSource surfaceLoop;         // ambient above water
    public AudioSource underwaterLoop;      // ambient underwater
    public AudioClip gaspSfx;               // optional, when surfacing low on air

    [Header("Air Settings")]
    public float maxAirSeconds = 25f;
    public float refillPerSec = 10f;        // air regained per second at surface
    public float lowAirThreshold = 8f;      // below this, start heavy effects

    [Header("Audio Settings")]
    public float surfaceCutoff = 5000f;
    public float underwaterCutoff = 800f;
    public float xfadeSpeed = 2.0f;         // how quickly volumes/cutoff change

    float air;
    bool wasUnder;

    void Start()
    {
        air = maxAirSeconds;
        if (hypoxiaVolume != null) hypoxiaVolume.weight = 0f;
        if (underwaterLoop != null) underwaterLoop.volume = 0f;
        if (surfaceLoop != null) surfaceLoop.volume = 0.4f;
        wasUnder = false;
    }

    void Update()
    {
        bool under = head.position.y < waterLevelY - 0.05f;

        // Air logic
        if (under) air -= Time.deltaTime;
        else air = Mathf.Min(maxAirSeconds, air + refillPerSec * Time.deltaTime);

        air = Mathf.Max(0f, air);

        // FX intensity grows as air gets low
        float t = 0f;
        if (air < lowAirThreshold)
            t = Mathf.InverseLerp(lowAirThreshold, 0f, air); // 0..1 as we run out
        if (hypoxiaVolume != null)
            hypoxiaVolume.weight = Mathf.MoveTowards(hypoxiaVolume.weight, t, Time.deltaTime * 1.5f);

        // Audio: lowpass + loops
        if (lowpass != null)
        {
            float targetCut = under ? underwaterCutoff : surfaceCutoff;
            lowpass.cutoffFrequency = Mathf.MoveTowards(lowpass.cutoffFrequency, targetCut, xfadeSpeed * 1000f * Time.deltaTime);
        }

        if (surfaceLoop != null)
        {
            float target = under ? 0f : 0.4f;
            surfaceLoop.volume = Mathf.MoveTowards(surfaceLoop.volume, target, xfadeSpeed * Time.deltaTime);
        }

        if (underwaterLoop != null)
        {
            float target = under ? 0.35f : 0f;
            underwaterLoop.volume = Mathf.MoveTowards(underwaterLoop.volume, target, xfadeSpeed * Time.deltaTime);
        }

        // Surface gasp when coming up with very low air
        if (!under && wasUnder && air < lowAirThreshold * 0.35f && gaspSfx != null)
            AudioSource.PlayClipAtPoint(gaspSfx, head.position, 0.9f);

        wasUnder = under;
    }
}

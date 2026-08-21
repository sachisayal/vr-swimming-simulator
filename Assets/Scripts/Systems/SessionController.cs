using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class SessionController : MonoBehaviour
{
    [Header("Refs")]
    public BreathManager breath;     // drag your BreathSystem object
    public SwimTrainer trainer;      // drag your SwimTrainer (optional)
    public TMP_Text statsText;       // your HUD text (StatsText)

    bool running;

public bool startRunning = true;  // show this in the Inspector

void Start()
{
    running = startRunning;
    if (breath)  breath.enabled  = startRunning;
    if (trainer) trainer.enabled = startRunning;

    Write(startRunning ? "Session running… (Enter = Stop, R = Reset)"
                       : "Press Enter to START");
}


    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.enterKey.wasPressedThisFrame)
        {
            if (running) StopSession();
            else StartSession();
        }
        if (kb.rKey.wasPressedThisFrame)
        {
            ResetSession();
        }
    }

    void StartSession()
    {
        running = true;
        if (breath)  breath.enabled  = true;
        if (trainer) trainer.enabled = true;
        Write("Session running…  (Enter = Stop, R = Reset)");
    }

    void StopSession()
    {
        running = false;
        if (trainer) trainer.enabled = false;
        if (breath)  breath.enabled  = false;
        Write("Paused. (Enter = Resume, R = Reset)");
    }

    void ResetSession()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.buildIndex);
    }

    void Write(string msg)
    {
        if (statsText) statsText.text = msg;
    }
}

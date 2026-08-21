using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class FreestyleStrokeEvaluator : MonoBehaviour
{
    [System.Serializable]
    public class ArmConfig
    {
        [Header("References")]
        public Transform shoulder;          // Shoulder joint (child of this system)
        public Transform hand;              // XR controller / hand
        public LineRenderer lineRenderer;   // Ellipse visualization

        [Header("Markers (optional, for in-game tutorial visuals)")]
        public GameObject projectedHandMarkerPrefab;
        public GameObject ellipsePointMarkerPrefab;

        [Header("Ellipse Adjustment")]
        [Tooltip("Per-arm scaling factor for the ellipse radius.")]
        public float radiusScale = 1.0f;

        [HideInInspector] public GameObject projectedHandMarker;
        [HideInInspector] public GameObject ellipsePointMarker;

        // Runtime outputs
        [HideInInspector] public float angle;       // radians, -π..π
        [HideInInspector] public float planarError; // distance in the ellipse plane
        [HideInInspector] public float error;       // full 3D distance to ellipse
        [HideInInspector] public bool  onStroke;    // within acceptableError?
    }

    [Header("HUD")]
    [Tooltip("Optional: Text element that follows the user's view to show stroke status.")]
    public TextMeshProUGUI statusText;

    [Header("Left / Right Arm Configs")]
    public ArmConfig leftArm;
    public ArmConfig rightArm;

    [Header("Head / Body Tracking")]
    [Tooltip("Assign the Main Camera (XR head) here.")]
    public Transform head;

    [Tooltip("If true, this object will follow the head position.")]
    public bool followHeadPosition = true;

    [Header("Ellipse Shape (applied to both arms, local to each shoulder)")]
    public float radiusForward = 0.4f;
    public float radiusDown = 0.3f;
    public float centerForwardOffset = 0.15f;

    [Tooltip("Rotation of the ellipse within the shoulder plane (deg, + = clockwise from forward).")]
    public float ellipseRotationDegrees = 0f;

    [Header("Evaluation")]
    [Tooltip("Max 3D distance from ellipse to be considered 'on stroke' (m).")]
    public float acceptableError = 0.1f;

    [Tooltip("Max allowed deviation from 'opposite' (deg).")]
    public float phaseToleranceDegrees = 25f;

    [Tooltip("Number of segments used to render each ellipse.")]
    public int ellipseSegments = 64;

    [Header("Tutorial Success Criteria")]
    [Tooltip("How long both arms must be on-stroke and opposite-phase to count as success (seconds).")]
    public float requiredGoodPhaseTime = 3.0f;

    [Header("Events")]
    public UnityEvent onTutorialSuccess;

    // Runtime state
    private float goodPhaseTimer = 0f;
    private bool  tutorialCompleted = false;

    void Start()
    {
        InitArm(leftArm);
        InitArm(rightArm);
    }

    void InitArm(ArmConfig arm)
    {
        if (arm == null) return;

        if (arm.lineRenderer != null)
        {
            arm.lineRenderer.positionCount = ellipseSegments + 1;
            arm.lineRenderer.loop = true;
            arm.lineRenderer.useWorldSpace = true;

            var grad = new Gradient();
            grad.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.blue, 0f),
                    new GradientColorKey(Color.blue, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                });
            arm.lineRenderer.colorGradient = grad;
        }

        if (arm.projectedHandMarkerPrefab != null)
            arm.projectedHandMarker = Instantiate(arm.projectedHandMarkerPrefab);

        if (arm.ellipsePointMarkerPrefab != null)
            arm.ellipsePointMarker = Instantiate(arm.ellipsePointMarkerPrefab);
    }

    void Update()
    {
        UpdateBodyAnchorFromHead();

        EvaluateArm(leftArm);
        EvaluateArm(rightArm);

        EvaluateBilateralPhase();
    }

    // Body anchor follows head position + yaw
    void UpdateBodyAnchorFromHead()
    {
        if (!followHeadPosition || head == null) return;

        transform.position = head.position;

        Vector3 e = head.rotation.eulerAngles;
        float yaw = e.y;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

void EvaluateArm(ArmConfig arm)
{
    if (arm == null || arm.shoulder == null || arm.hand == null) return;

    // Base shoulder axes
    Vector3 rightAxis = arm.shoulder.right.normalized;      // side-to-side
    Vector3 downLocal = (-arm.shoulder.up).normalized;      // downward
    Vector3 fwdLocal  = arm.shoulder.forward.normalized;    // forward

    // Rotate the ellipse PLANE around the side axis (rightAxis),
    // so the stroke tilts "away/toward the body + up/down".
    Quaternion tilt = Quaternion.AngleAxis(ellipseRotationDegrees, rightAxis);

    Vector3 downAxis = tilt * downLocal;     // tilted down vector
    Vector3 fwdAxis  = tilt * fwdLocal;      // tilted forward vector

    // New plane normal (roughly "outward" from the shoulder)
    Vector3 planeNormal = Vector3.Cross(rightAxis, downAxis).normalized;

    // Ellipse center follows the tilted forward direction
    Vector3 center = arm.shoulder.position + fwdAxis * centerForwardOffset;

    // Per-arm scaling
    float rf = radiusForward * arm.radiusScale;
    float rd = radiusDown   * arm.radiusScale;

    // ---------- 1. Draw ellipse (using right / down in the tilted plane) ----------
    if (arm.lineRenderer != null)
    {
        for (int i = 0; i <= ellipseSegments; i++)
        {
            float t   = (float)i / ellipseSegments;
            float ang = t * Mathf.PI * 2f;

            float c = Mathf.Cos(ang);
            float s = Mathf.Sin(ang);

            Vector3 p = center +
                        rightAxis * rf * c +
                        downAxis  * rd * s;

            arm.lineRenderer.SetPosition(i, p);
        }
    }

    // ---------- 2. Project hand onto the tilted plane ----------
    Vector3 handToCenter  = arm.hand.position - center;
    float   distToPlane   = Vector3.Dot(handToCenter, planeNormal);
    Vector3 projectedHand = arm.hand.position - planeNormal * distToPlane;

    Vector3 rel = projectedHand - center;

    // Coordinates in the (rightAxis, downAxis) basis
    float X = Vector3.Dot(rel, rightAxis) / rf;
    float Y = Vector3.Dot(rel, downAxis)  / rd;

    // Parametric angle around the ellipse in this tilted plane
    float angle = Mathf.Atan2(Y, X);
    arm.angle = angle;

    // ---------- 3. Ideal point on ellipse for that angle ----------
    float ec = Mathf.Cos(angle);
    float es = Mathf.Sin(angle);

    Vector3 ellipsePoint =
        center +
        rightAxis * rf * ec +
        downAxis  * rd * es;

    // ---------- 4. Error computation ----------
    arm.planarError = Vector3.Distance(projectedHand, ellipsePoint);
    arm.error       = Vector3.Distance(arm.hand.position, ellipsePoint);
    arm.onStroke    = arm.error <= acceptableError;

    // ---------- 5. Markers ----------
    if (arm.projectedHandMarker != null)
        arm.projectedHandMarker.transform.position = arm.hand.position;

    if (arm.ellipsePointMarker != null)
        arm.ellipsePointMarker.transform.position = ellipsePoint;

    if (arm.projectedHandMarker != null)
        SetMarkerColor(arm.projectedHandMarker, arm.onStroke ? Color.green : Color.red);
    if (arm.ellipsePointMarker != null)
        SetMarkerColor(arm.ellipsePointMarker, Color.yellow);
}


    void SetMarkerColor(GameObject marker, Color c)
    {
        var renderer = marker.GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
            renderer.material.color = c;
    }

    void EvaluateBilateralPhase()
    {
        bool bothOnStroke = leftArm.onStroke && rightArm.onStroke;

        float r = rightArm.angle;
        float l = leftArm.angle;

        float rDeg = r * Mathf.Rad2Deg;
        float lDeg = l * Mathf.Rad2Deg;

        // Because left basis is mirrored, "opposite" physically = "same angle numerically".
        float rawDiffDeg = Mathf.DeltaAngle(rDeg, lDeg);

        bool inPhase = Mathf.Abs(rawDiffDeg) <= phaseToleranceDegrees;

        // -------------------- STATUS HUD --------------------
        if (statusText != null)
        {
            if (bothOnStroke && inPhase)
            {
                statusText.text  = "Great! Arms on path and in phase.";
                statusText.color = Color.green;
            }
            else if (!bothOnStroke)
            {
                statusText.text  = "Stay closer to the stroke paths.";
                statusText.color = Color.yellow;
            }
            else
            {
                statusText.text  = "Keep arms opposite each other.";
                statusText.color = Color.yellow;
            }
        }

        // -------------------- TUTORIAL GATING --------------------
        if (bothOnStroke && inPhase)
            goodPhaseTimer += Time.deltaTime;
        else
            goodPhaseTimer = 0f;

        if (!tutorialCompleted && goodPhaseTimer >= requiredGoodPhaseTime)
        {
            tutorialCompleted = true;
            Debug.Log("Freestyle bilateral tutorial success!");
            onTutorialSuccess?.Invoke();

            if (statusText != null)
            {
                statusText.text  = "Tutorial complete!";
                statusText.color = Color.cyan;
            }
        }
    }

    public void SetGuideVisible(bool visible)
    {
        if (leftArm.lineRenderer != null)  leftArm.lineRenderer.enabled  = visible;
        if (rightArm.lineRenderer != null) rightArm.lineRenderer.enabled = visible;

        if (leftArm.projectedHandMarker != null)  leftArm.projectedHandMarker.SetActive(visible);
        if (leftArm.ellipsePointMarker != null)   leftArm.ellipsePointMarker.SetActive(visible);

        if (rightArm.projectedHandMarker != null) rightArm.projectedHandMarker.SetActive(visible);
        if (rightArm.ellipsePointMarker != null)  rightArm.ellipsePointMarker.SetActive(visible);

        enabled = visible;
    }

    public float GetGoodPhaseTime() => goodPhaseTimer;
    public bool  IsTutorialCompleted() => tutorialCompleted;

    public void SetLeftArmRadiusScale(float scale)
    {
        if (leftArm != null)
            leftArm.radiusScale = scale;
    }

    public void SetRightArmRadiusScale(float scale)
    {
        if (rightArm != null)
            rightArm.radiusScale = scale;
    }
}

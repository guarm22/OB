using UnityEngine;

[ExecuteAlways]
public class SwingingLight : MonoBehaviour
{
    public enum AmplitudeMode
    {
        Angle,
        ArcDistance
    }

    [Header("Timing")]
    [Min(0.01f)]
    [SerializeField] private float swingPeriod = 2.5f;
    [SerializeField] private float phaseOffset = 0f;
    [SerializeField] private bool randomizePhaseOnStart = false;

    [Header("Amplitude")]
    [SerializeField] private AmplitudeMode amplitudeMode = AmplitudeMode.Angle;
    [Min(0f)]
    [SerializeField] private float swingAngleDegrees = 8f;
    [Min(0f)]
    [SerializeField] private float arcDistance = 0.25f;
    [Min(0.01f)]
    [SerializeField] private float pivotToLightDistance = 2f;

    [Header("Direction")]
    [SerializeField] private bool useLocalSpace = true;
    [SerializeField] private Vector3 swingAxis = Vector3.forward;
    [SerializeField] private Vector3 restDirectionFromPivot = Vector3.down;
    [SerializeField] private bool rotateLightWithSwing = false;

    [Header("Editor Preview")]
    [SerializeField] private bool previewInEditMode = true;
    [SerializeField] private bool repaintSceneViewInEditMode = true;

    private Quaternion initialLocalRotation;
    private Quaternion initialWorldRotation;
    private Vector3 initialLocalPosition;
    private Vector3 initialWorldPosition;
    private Vector3 localPivotPoint;
    private Vector3 worldPivotPoint;
    private Vector3 baselineLocalPosition;
    private Quaternion baselineLocalRotation;
    private Vector3 baselineLocalScale;
    private bool hasBaselineTransform;
    private float activePhaseOffset;
    private double editorStartTime;

    private void OnEnable()
    {
        CaptureBaselineTransform();
        CacheInitialTransform();
        activePhaseOffset = randomizePhaseOnStart ? Random.Range(0f, Mathf.PI * 2f) : phaseOffset;

#if UNITY_EDITOR
        editorStartTime = UnityEditor.EditorApplication.timeSinceStartup;
#endif
    }

    private void Awake()
    {
        CaptureBaselineTransform();
        CacheInitialTransform();
    }

    void Update()
    {
        if (!Application.isPlaying && !previewInEditMode)
        {
            return;
        }

        float angle = CalculateCurrentAngle(GetTimeSeconds());
        Vector3 axis = swingAxis.sqrMagnitude < 0.0001f ? Vector3.forward : swingAxis.normalized;
        Vector3 restDirection = GetRestDirection();

        Quaternion swingRotation = Quaternion.AngleAxis(angle, axis);

        if (useLocalSpace)
        {
            Vector3 currentDirection = swingRotation * restDirection;
            transform.localPosition = localPivotPoint + (currentDirection * pivotToLightDistance);

            if (rotateLightWithSwing)
            {
                transform.localRotation = initialLocalRotation * swingRotation;
            }
        }
        else
        {
            Vector3 currentDirection = swingRotation * restDirection;
            transform.position = worldPivotPoint + (currentDirection * pivotToLightDistance);

            if (rotateLightWithSwing)
            {
                transform.rotation = initialWorldRotation * swingRotation;
            }
        }

#if UNITY_EDITOR
        if (!Application.isPlaying && previewInEditMode && repaintSceneViewInEditMode)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
#endif
    }

    private float CalculateCurrentAngle(float elapsedTime)
    {
        if (swingPeriod <= 0.0001f)
        {
            return 0f;
        }

        float omega = Mathf.PI * 2f / swingPeriod;
        float amplitude = GetAmplitudeDegrees();

        return Mathf.Sin((elapsedTime * omega) + activePhaseOffset) * amplitude;
    }

    private float GetTimeSeconds()
    {
        if (Application.isPlaying)
        {
            return Time.time;
        }

#if UNITY_EDITOR
        return (float)(UnityEditor.EditorApplication.timeSinceStartup - editorStartTime);
#else
        return 0f;
#endif
    }

    private float GetAmplitudeDegrees()
    {
        if (amplitudeMode == AmplitudeMode.Angle)
        {
            return Mathf.Max(0f, swingAngleDegrees);
        }

        // Arc length relation: s = r * theta, where theta is in radians.
        float thetaRadians = Mathf.Max(0f, arcDistance) / Mathf.Max(0.01f, pivotToLightDistance);
        return Mathf.Rad2Deg * thetaRadians;
    }

    private void CacheInitialTransform()
    {
        if (!hasBaselineTransform)
        {
            CaptureBaselineTransform();
        }

        initialLocalRotation = baselineLocalRotation;
        initialLocalPosition = baselineLocalPosition;

        if (transform.parent != null)
        {
            initialWorldRotation = transform.parent.rotation * initialLocalRotation;
            initialWorldPosition = transform.parent.TransformPoint(initialLocalPosition);
        }
        else
        {
            initialWorldRotation = initialLocalRotation;
            initialWorldPosition = initialLocalPosition;
        }

        Vector3 restDirection = GetRestDirection();
        localPivotPoint = initialLocalPosition - (restDirection * pivotToLightDistance);
        worldPivotPoint = initialWorldPosition - (restDirection * pivotToLightDistance);
    }

    private void CaptureBaselineTransform()
    {
        if (hasBaselineTransform)
        {
            return;
        }

        baselineLocalPosition = transform.localPosition;
        baselineLocalRotation = transform.localRotation;
        baselineLocalScale = transform.localScale;
        hasBaselineTransform = true;
    }

    private void RestoreBaselineTransform()
    {
        if (!hasBaselineTransform)
        {
            CaptureBaselineTransform();
        }

        transform.localPosition = baselineLocalPosition;
        transform.localRotation = baselineLocalRotation;
        transform.localScale = baselineLocalScale;
    }

    private Vector3 GetRestDirection()
    {
        if (restDirectionFromPivot.sqrMagnitude < 0.0001f)
        {
            return Vector3.down;
        }

        return restDirectionFromPivot.normalized;
    }

    private void OnValidate()
    {
        if (swingAxis.sqrMagnitude < 0.0001f)
        {
            swingAxis = Vector3.forward;
        }

        if (restDirectionFromPivot.sqrMagnitude < 0.0001f)
        {
            restDirectionFromPivot = Vector3.down;
        }

        swingPeriod = Mathf.Max(0.01f, swingPeriod);
        pivotToLightDistance = Mathf.Max(0.01f, pivotToLightDistance);

        if (isActiveAndEnabled)
        {
            CacheInitialTransform();
        }
    }

    public void ResetAnimationAndPositionProperties()
    {
        swingPeriod = 2.5f;
        phaseOffset = 0f;
        randomizePhaseOnStart = false;

        amplitudeMode = AmplitudeMode.Angle;
        swingAngleDegrees = 8f;
        arcDistance = 0.25f;
        pivotToLightDistance = 2f;

        useLocalSpace = true;
        swingAxis = Vector3.forward;
        restDirectionFromPivot = Vector3.down;
        rotateLightWithSwing = false;

        previewInEditMode = true;
        repaintSceneViewInEditMode = true;

        RestoreBaselineTransform();
        activePhaseOffset = phaseOffset;
        CacheInitialTransform();
    }

    public void ResetAnimationStateOnly()
    {
        RestoreBaselineTransform();
        activePhaseOffset = randomizePhaseOnStart ? Random.Range(0f, Mathf.PI * 2f) : phaseOffset;
        CacheInitialTransform();
    }

    private void ApplyRestPose()
    {
        Vector3 restDirection = GetRestDirection();

        if (useLocalSpace)
        {
            transform.localPosition = localPivotPoint + (restDirection * pivotToLightDistance);
            if (rotateLightWithSwing)
            {
                transform.localRotation = initialLocalRotation;
            }
        }
        else
        {
            transform.position = worldPivotPoint + (restDirection * pivotToLightDistance);
            if (rotateLightWithSwing)
            {
                transform.rotation = initialWorldRotation;
            }
        }
    }
}

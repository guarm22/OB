using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
public class LightbulbCable : MonoBehaviour {

	public enum RotationMode {
		FollowSpline,
		FollowSplineWithWorldUp,
		Fixed
	}

	[Header("References")]
	public SplineContainer splineContainer;
	public GameObject prefab;
	public Transform generatedRoot;

	[Header("Distribution")]
	[Tooltip("If > 0, spacing is used directly. If <= 0, spacing is computed from density.")]
	[Min(0f)] public float spacing = 0.5f;
	[Tooltip("Items per world unit when spacing is <= 0.")]
	[Min(0.01f)] public float density = 2f;
	[Min(1)] public int maxInstances = 2000;
	public int splineIndex = 0;
	public bool includeEndPoint = true;

	[Header("Transform")]
	public RotationMode rotationMode = RotationMode.FollowSpline;
	public Vector3 positionOffset;
	public Vector3 rotationOffsetEuler;
	public Vector3 baseScale = Vector3.one;

	[Header("Variation")]
	public bool randomize = false;
	public int randomSeed = 12345;
	public Vector3 randomPositionJitter;
	public Vector3 randomRotationMin;
	public Vector3 randomRotationMax;
	public Vector3 randomScaleMin = Vector3.one;
	public Vector3 randomScaleMax = Vector3.one;

	[Header("Generation")]
	public bool regenerateOnValidate = true;
	public bool generateOnStart = true;
	public bool clearGeneratedOnDisable;

	private const string GeneratedPrefix = "CableBulb_";

	private void Start() {
		if (Application.isPlaying && generateOnStart) {
			Regenerate();
		}
	}

	private void OnValidate() {
		if (!regenerateOnValidate) {
			return;
		}

		if (!isActiveAndEnabled) {
			return;
		}

		Regenerate();
	}

	private void OnDisable() {
		if (!clearGeneratedOnDisable) {
			return;
		}

		ClearGenerated();
	}

	[ContextMenu("Regenerate")]
	public void Regenerate() {
		if (!TryGetTargetSpline(out var targetSpline)) {
			return;
		}

		ClearGenerated();

		if (prefab == null) {
			return;
		}

		var parent = generatedRoot != null ? generatedRoot : transform;
		var length = splineContainer.CalculateLength(splineIndex);
		var resolvedSpacing = spacing > 0f ? spacing : 1f / Mathf.Max(0.01f, density);
		resolvedSpacing = Mathf.Max(0.0001f, resolvedSpacing);

		var count = Mathf.Max(1, Mathf.CeilToInt(length / resolvedSpacing));
		count = Mathf.Min(count, Mathf.Max(1, maxInstances));
		var randomRotationFrame = ComputeEndpointPlaneFrame();

		var randomState = new Unity.Mathematics.Random((uint)math.max(1, randomSeed));

		for (var i = 0; i < count; i++) {
			var t = count == 1 ? 0f : (float)i / (includeEndPoint ? count - 1 : count);
			t = Mathf.Clamp01(t);

			if (!splineContainer.Evaluate(splineIndex, t, out float3 pos, out float3 tangent, out float3 up)) {
				continue;
			}

			var worldPos = (Vector3)pos;
			var worldTangent = ((Vector3)tangent).normalized;
			var worldUp = ((Vector3)up).normalized;

			var rotation = GetRotation(worldTangent, worldUp);
			var localOffset = positionOffset;
			var localEulerOffset = rotationOffsetEuler;
			var randomRotationEuler = Vector3.zero;
			var localScale = baseScale;

			if (randomize) {
				localOffset += RandomRangeVector3(ref randomState, -randomPositionJitter, randomPositionJitter);
				randomRotationEuler = RandomRangeVector3(ref randomState, randomRotationMin, randomRotationMax);
				localScale = Vector3.Scale(localScale, RandomRangeVector3(ref randomState, randomScaleMin, randomScaleMax));
			}

			var instancePosition = worldPos + rotation * localOffset;
			var instanceRotation = rotation * Quaternion.Euler(localEulerOffset);
			if (randomize) {
				var randomRotationWorld = randomRotationFrame * Quaternion.Euler(randomRotationEuler) * Quaternion.Inverse(randomRotationFrame);
				instanceRotation = randomRotationWorld * instanceRotation;
			}

			var instance = InstantiateInstance(instancePosition, instanceRotation, parent);
			instance.name = GeneratedPrefix + i;
			instance.transform.localScale = localScale;
		}
	}

	[ContextMenu("Clear Generated")]
	public void ClearGenerated() {
		var parent = generatedRoot != null ? generatedRoot : transform;

		for (var i = parent.childCount - 1; i >= 0; i--) {
			var child = parent.GetChild(i);
			if (!child.name.StartsWith(GeneratedPrefix)) {
				continue;
			}

			if (Application.isPlaying) {
				Destroy(child.gameObject);
			}
			else {
				DestroyImmediate(child.gameObject);
			}
		}
	}

	private bool TryGetTargetSpline(out Spline targetSpline) {
		targetSpline = null;

		if (splineContainer == null) {
			return false;
		}

		if (splineContainer.Splines == null || splineContainer.Splines.Count == 0) {
			return false;
		}

		if (splineIndex < 0 || splineIndex >= splineContainer.Splines.Count) {
			return false;
		}

		targetSpline = splineContainer.Splines[splineIndex];
		return targetSpline != null;
	}

	private Quaternion GetRotation(Vector3 tangent, Vector3 up) {
		switch (rotationMode) {
			case RotationMode.FollowSpline:
				if (tangent.sqrMagnitude < 0.000001f) {
					return transform.rotation;
				}
				return Quaternion.LookRotation(tangent, up.sqrMagnitude < 0.000001f ? Vector3.up : up);

			case RotationMode.FollowSplineWithWorldUp:
				if (tangent.sqrMagnitude < 0.000001f) {
					return transform.rotation;
				}
				return Quaternion.LookRotation(tangent, Vector3.up);

			default:
				return transform.rotation;
		}
	}

	private GameObject InstantiateInstance(Vector3 position, Quaternion rotation, Transform parent) {
		if (Application.isPlaying) {
			return Instantiate(prefab, position, rotation, parent);
		}

#if UNITY_EDITOR
		var instance = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent);
		instance.transform.SetPositionAndRotation(position, rotation);
		return instance;
#else
		return Instantiate(prefab, position, rotation, parent);
#endif
	}

	private static Vector3 RandomRangeVector3(ref Unity.Mathematics.Random random, Vector3 min, Vector3 max) {
		return new Vector3(
			random.NextFloat(min.x, max.x),
			random.NextFloat(min.y, max.y),
			random.NextFloat(min.z, max.z)
		);
	}

	private Quaternion ComputeEndpointPlaneFrame() {
		if (splineContainer == null) {
			return transform.rotation;
		}

		if (!splineContainer.Evaluate(splineIndex, 0f, out float3 start, out _, out _) ||
			!splineContainer.Evaluate(splineIndex, 1f, out float3 end, out _, out _)) {
			return transform.rotation;
		}

		var direction = ((Vector3)end - (Vector3)start).normalized;
		if (direction.sqrMagnitude < 0.000001f) {
			return transform.rotation;
		}

		var planeNormal = Vector3.Cross(direction, Vector3.up).normalized;
		if (planeNormal.sqrMagnitude < 0.000001f) {
			planeNormal = Vector3.Cross(direction, transform.right).normalized;
		}
		if (planeNormal.sqrMagnitude < 0.000001f) {
			planeNormal = Vector3.Cross(direction, Vector3.forward).normalized;
		}

		var planeUp = Vector3.Cross(planeNormal, direction).normalized;
		if (planeUp.sqrMagnitude < 0.000001f) {
			planeUp = Vector3.up;
		}

		return Quaternion.LookRotation(direction, planeUp);
	}
}

using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaterfallDrapingMesh : MonoBehaviour
{
    [Header("Waterfall Shape")]
    [Tooltip("Ordered points along the water's path.")]
    public Transform[] controlPoints;

    [Min(0.01f)]
    public float width = 3f;

    [Range(1, 32)]
    public int subdivisionsPerSegment = 8;

    [Tooltip("How many times the water pattern repeats along the path.")]
    [Min(0.01f)]
    public float uvTiling = 1f;

    [Header("Mesh")]
    public bool autoUpdate = true;

    private MeshFilter meshFilter;
    private Mesh generatedMesh;
    private int lastShapeHash;

    private void OnEnable()
    {
        Initialize();
        RebuildMesh();
    }

    private void OnValidate()
    {
        width = Mathf.Max(0.01f, width);
        subdivisionsPerSegment = Mathf.Clamp(
            subdivisionsPerSegment, 1, 32);
        uvTiling = Mathf.Max(0.01f, uvTiling);

        if (isActiveAndEnabled)
            RebuildMesh();
    }

    private void Update()
    {
        if (!autoUpdate)
            return;

        int currentHash = GetShapeHash();

        if (currentHash != lastShapeHash)
            RebuildMesh();
    }

    private void Initialize()
    {
        meshFilter = GetComponent<MeshFilter>();

        if (generatedMesh == null)
        {
            generatedMesh = new Mesh();
            generatedMesh.name = "Generated Waterfall Mesh";
        }

        meshFilter.sharedMesh = generatedMesh;
    }

    [ContextMenu("Rebuild Waterfall Mesh")]
    public void RebuildMesh()
    {
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        if (generatedMesh == null)
        {
            generatedMesh = new Mesh();
            generatedMesh.name = "Generated Waterfall Mesh";
        }

        meshFilter.sharedMesh = generatedMesh;

        if (controlPoints == null || controlPoints.Length < 2)
        {
            generatedMesh.Clear();
            lastShapeHash = GetShapeHash();
            return;
        }

        for (int i = 0; i < controlPoints.Length; i++)
        {
            if (controlPoints[i] == null)
            {
                generatedMesh.Clear();
                lastShapeHash = GetShapeHash();
                return;
            }
        }

        int segmentCount = controlPoints.Length - 1;
        int sampleCount = segmentCount * subdivisionsPerSegment + 1;

        Vector3[] vertices = new Vector3[sampleCount * 2];
        Vector2[] uvs = new Vector2[sampleCount * 2];
        int[] triangles = new int[(sampleCount - 1) * 6];

        Vector3 previousCenter = Vector3.zero;
        float totalLength = 0f;

        for (int i = 0; i < sampleCount; i++)
        {
            float pathPosition =
                (float)i / subdivisionsPerSegment;

            int segment = Mathf.Min(
                Mathf.FloorToInt(pathPosition), segmentCount - 1);

            float t = i == sampleCount - 1
                ? 1f
                : pathPosition - segment;

            Vector3 center = GetPoint(segment, t);
            Vector3 tangent = GetTangent(segment, t).normalized;
            Vector3 surfaceNormal = GetSurfaceNormal(segment, t);

            // The point's local Y axis indicates the surface normal.
            Vector3 side = Vector3.Cross(
                tangent, surfaceNormal).normalized;

            // Fallback for poorly aligned control points.
            if (side.sqrMagnitude < 0.001f)
            {
                side = Vector3.Cross(tangent, Vector3.up).normalized;

                if (side.sqrMagnitude < 0.001f)
                    side = Vector3.Cross(
                        tangent, Vector3.right).normalized;
            }

            if (i > 0)
                totalLength += Vector3.Distance(previousCenter, center);

            previousCenter = center;

            Vector3 left = center - side * width * 0.5f;
            Vector3 right = center + side * width * 0.5f;

            vertices[i * 2] =
                transform.InverseTransformPoint(left);

            vertices[i * 2 + 1] =
                transform.InverseTransformPoint(right);

            float v = totalLength * uvTiling;

            // Normalized UVs keep the existing shader's end fades working.
            uvs[i * 2] = new Vector2(0f, v);
            uvs[i * 2 + 1] = new Vector2(1f, v);
        }

        // Normalize length UVs so the shader fades at the ends.
        float uvLength = Mathf.Max(0.001f, totalLength * uvTiling);

        for (int i = 0; i < sampleCount; i++)
        {
            uvs[i * 2].y /= uvLength;
            uvs[i * 2 + 1].y /= uvLength;
        }

        int tri = 0;

        for (int i = 0; i < sampleCount - 1; i++)
        {
            int a = i * 2;
            int b = a + 1;
            int c = a + 2;
            int d = a + 3;

            triangles[tri++] = a;
            triangles[tri++] = c;
            triangles[tri++] = b;

            triangles[tri++] = b;
            triangles[tri++] = c;
            triangles[tri++] = d;
        }

        generatedMesh.Clear();
        generatedMesh.vertices = vertices;
        generatedMesh.uv = uvs;
        generatedMesh.triangles = triangles;
        generatedMesh.RecalculateNormals();
        generatedMesh.RecalculateBounds();

        lastShapeHash = GetShapeHash();
    }

    private Vector3 GetPoint(int segment, float t)
    {
        Vector3 p0 = GetControlPoint(Mathf.Max(0, segment - 1));
        Vector3 p1 = GetControlPoint(segment);
        Vector3 p2 = GetControlPoint(segment + 1);
        Vector3 p3 = GetControlPoint(
            Mathf.Min(controlPoints.Length - 1, segment + 2));

        return CatmullRom(p0, p1, p2, p3, t);
    }

    private Vector3 GetTangent(int segment, float t)
    {
        const float delta = 0.001f;

        Vector3 before = GetPoint(
            segment, Mathf.Clamp01(t - delta));

        Vector3 after = GetPoint(
            segment, Mathf.Clamp01(t + delta));

        if ((after - before).sqrMagnitude < 0.000001f)
            return GetControlPoint(segment + 1)
                - GetControlPoint(segment);

        return after - before;
    }

    private Vector3 GetSurfaceNormal(int segment, float t)
    {
        Vector3 a = controlPoints[segment].up;
        Vector3 b = controlPoints[segment + 1].up;

        return Vector3.Slerp(a, b, t).normalized;
    }

    private Vector3 GetControlPoint(int index)
    {
        return controlPoints[index].position;
    }

    private Vector3 CatmullRom(
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            2f * p1 +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    private int GetShapeHash()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + width.GetHashCode();
            hash = hash * 31 + subdivisionsPerSegment;
            hash = hash * 31 + uvTiling.GetHashCode();

            if (controlPoints != null)
            {
                foreach (Transform point in controlPoints)
                {
                    if (point == null)
                    {
                        hash = hash * 31;
                        continue;
                    }

                    hash = hash * 31 + point.position.GetHashCode();
                    hash = hash * 31 + point.rotation.GetHashCode();
                }
            }

            return hash;
        }
    }

    private void OnDisable()
    {
        // Do not destroy the mesh here; ExecuteAlways can disable
        // and re-enable this component during editor operations.
    }
}

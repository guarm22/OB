using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public class CampfireParticles : MonoBehaviour
{
    [SerializeField]
    private float size = 1f;

    [SerializeField]
    private float emissionMultiplier = 1f;

    [SerializeField]
    private float flameSpeed = 1f;

    [SerializeField]
    private bool createOnAwake = true;

    private GameObject effectRoot;
    private Material flameMaterial;
    private Texture2D flameTexture;

    private void Awake()
    {
        if (createOnAwake)
        {
            CreateFire();
        }
    }

    public void CreateFire()
    {
        if (effectRoot != null)
        {
            return;
        }

        Shader particleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (particleShader == null)
        {
            Debug.LogError("CampfireParticles could not find the URP Particles/Unlit shader.", this);
            return;
        }

        flameTexture = CreateFlameTexture();
        flameMaterial = new Material(particleShader)
        {
            name = "Campfire Flame (Runtime)",
            enableInstancing = true,
            renderQueue = 3000
        };
        flameMaterial.SetTexture("_BaseMap", flameTexture);
        flameMaterial.SetColor("_BaseColor", Color.white);
        flameMaterial.SetFloat("_Surface", 1f);
        flameMaterial.SetFloat("_Blend", 0f);
        flameMaterial.SetFloat("_ZWrite", 0f);
        flameMaterial.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
        flameMaterial.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
        flameMaterial.SetFloat("_SrcBlendAlpha", (float)BlendMode.One);
        flameMaterial.SetFloat("_DstBlendAlpha", (float)BlendMode.OneMinusSrcAlpha);
        flameMaterial.SetFloat("_BlendOp", (float)BlendOp.Add);
        flameMaterial.SetFloat("_BlendOpAlpha", (float)BlendOp.Add);
        flameMaterial.SetOverrideTag("RenderType", "Transparent");
        flameMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

        effectRoot = new GameObject("Campfire Particle Effect");
        effectRoot.transform.SetParent(transform, false);
        effectRoot.transform.localScale = Vector3.one * Mathf.Max(0.01f, size);

        CreateFlameSystem("Main Flames", 7f, 0.38f, 0.62f, 0.55f, 0.95f, 0.45f, 0.9f, 0.28f, 48, CreateFlameGradient());
        CreateFlameSystem("Small Flames", 10f, 0.2f, 0.38f, 0.35f, 0.72f, 0.55f, 1.05f, 0.34f, 40, CreateSmallFlameGradient());
        CreateFlameSystem("Low Flames", 5f, 0.42f, 0.7f, 0.3f, 0.55f, 0.25f, 0.55f, 0.32f, 24, CreateLowFlameGradient());
    }

    private void CreateFlameSystem(
        string systemName,
        float emissionRate,
        float minSize,
        float maxSize,
        float minLifetime,
        float maxLifetime,
        float minSpeed,
        float maxSpeed,
        float radius,
        int maxParticles,
        Gradient colorGradient)
    {
        GameObject systemObject = new GameObject(systemName);
        systemObject.transform.SetParent(effectRoot.transform, false);

        ParticleSystem particleSystem = systemObject.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particleSystem.main;
        main.duration = 5f;
        main.loop = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(minLifetime, maxLifetime);
        main.startSpeed = new ParticleSystem.MinMaxCurve(minSpeed * flameSpeed, maxSpeed * flameSpeed);
        main.startSize = new ParticleSystem.MinMaxCurve(minSize, maxSize);
        main.startColor = Color.white;
        main.maxParticles = maxParticles;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.scalingMode = ParticleSystemScalingMode.Hierarchy;
        main.playOnAwake = false;

        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.rateOverTime = emissionRate * Mathf.Max(0f, emissionMultiplier);

        ParticleSystem.ShapeModule shape = particleSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = radius;
        shape.radiusThickness = 0.85f;
        shape.rotation = new Vector3(90f, 0f, 0f);

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(colorGradient);

        ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = particleSystem.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
            new Keyframe(0f, 0.12f),
            new Keyframe(0.18f, 1f),
            new Keyframe(0.62f, 0.68f),
            new Keyframe(1f, 0f)));

        ParticleSystem.NoiseModule noise = particleSystem.noise;
        noise.enabled = true;
        noise.strength = 0.22f;
        noise.frequency = 0.65f;
        noise.scrollSpeed = 0.2f;
        noise.damping = true;
        noise.octaveCount = 1;

        ParticleSystemRenderer particleRenderer = systemObject.GetComponent<ParticleSystemRenderer>();
        particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
        particleRenderer.alignment = ParticleSystemRenderSpace.View;
        particleRenderer.material = flameMaterial;
        particleRenderer.sortingFudge = 1f;
        particleSystem.Play();
    }

    private Texture2D CreateFlameTexture()
    {
        const int width = 64;
        const int height = 128;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            name = "Campfire Flame Texture (Runtime)",
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        Color[] pixels = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            float vertical = (float)y / (height - 1);
            float taper = Mathf.Pow(1f - vertical, 0.72f);
            float center = 0.5f + Mathf.Sin(vertical * 10f) * 0.035f;

            for (int x = 0; x < width; x++)
            {
                float horizontal = (float)x / (width - 1);
                float noise = Mathf.PerlinNoise(horizontal * 11f, vertical * 8f);
                float edgeNoise = (noise - 0.5f) * 0.12f;
                float flameWidth = 0.07f + taper * 0.38f + edgeNoise;
                float distance = Mathf.Abs(horizontal - center);
                float alpha = 1f - Mathf.SmoothStep(flameWidth * 0.72f, flameWidth, distance);
                alpha *= Mathf.SmoothStep(0f, 0.07f, vertical);
                alpha *= 1f - Mathf.SmoothStep(0.88f, 1f, vertical);

                float core = 1f - Mathf.Clamp01(distance / Mathf.Max(flameWidth, 0.001f));
                Color color = Color.Lerp(new Color(0.38f, 0.025f, 0.002f), new Color(1f, 0.16f, 0.005f), Mathf.SmoothStep(0f, 0.65f, core));
                color = Color.Lerp(color, new Color(1f, 0.72f, 0.12f), Mathf.SmoothStep(0.55f, 0.92f, core));
                color.a = alpha;
                pixels[y * width + x] = color;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply(false, true);
        return texture;
    }

    private Gradient CreateFlameGradient()
    {
        return CreateGradient(
            new Color(1f, 0.22f, 0.025f),
            new Color(1f, 0.56f, 0.08f),
            new Color(1f, 0.13f, 0.015f));
    }

    private Gradient CreateSmallFlameGradient()
    {
        return CreateGradient(
            new Color(1f, 0.38f, 0.06f),
            new Color(1f, 0.78f, 0.2f),
            new Color(1f, 0.2f, 0.02f));
    }

    private Gradient CreateLowFlameGradient()
    {
        return CreateGradient(
            new Color(0.9f, 0.09f, 0.01f),
            new Color(1f, 0.4f, 0.03f),
            new Color(0.8f, 0.06f, 0.005f));
    }

    private Gradient CreateGradient(Color start, Color middle, Color end)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(start, 0f),
                new GradientColorKey(middle, 0.38f),
                new GradientColorKey(end, 1f)
            },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(1f, 0.16f),
                new GradientAlphaKey(0.85f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            });
        return gradient;
    }

    private void OnDestroy()
    {
        if (effectRoot != null)
        {
            Destroy(effectRoot);
        }

        if (flameMaterial != null)
        {
            Destroy(flameMaterial);
        }

        if (flameTexture != null)
        {
            Destroy(flameTexture);
        }
    }
}

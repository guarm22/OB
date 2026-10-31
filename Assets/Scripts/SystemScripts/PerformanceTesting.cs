using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using Unity.Profiling.LowLevel;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

public class PerformanceTesting : MonoBehaviour
{
    [SerializeField]
    private float reportInterval = 5f;

    [SerializeField]
    private float frameBudgetMilliseconds = 16.67f;

    private readonly List<Metric> metrics = new List<Metric>();
    private readonly List<string> unavailableMetrics = new List<string>();
    private readonly HashSet<string> metricNames = new HashSet<string>();
    private float reportTimer;
    private float totalFrameMilliseconds;
    private float worstFrameMilliseconds;
    private int frameCount;
    private int framesOverBudget;

    private sealed class Metric
    {
        public readonly string name;
        public readonly string group;
        public readonly bool isTime;
        public ProfilerRecorder recorder;
        public float total;
        public float maximum;
        public int samples;

        public Metric(string name, string group, bool isTime, ProfilerRecorder recorder)
        {
            this.name = name;
            this.group = group;
            this.isTime = isTime;
            this.recorder = recorder;
        }

        public void Sample()
        {
            if (!recorder.Valid)
            {
                return;
            }

            float value = recorder.LastValue;
            if (value < 0)
            {
                return;
            }

            total += value;
            maximum = Mathf.Max(maximum, value);
            samples++;
        }

        public void Reset()
        {
            total = 0;
            maximum = 0;
            samples = 0;
        }

        public string Format()
        {
            float average = samples > 0 ? total / samples : 0;
            if (isTime)
            {
                return $"{name}: avg {average / 1000000f:F2} ms, max {maximum / 1000000f:F2} ms";
            }

            return $"{name}: avg {average / 1024f:F1} KB/frame, max {maximum / 1024f:F1} KB/frame";
        }
    }

    private void OnEnable()
    {
        AddMetric(ProfilerCategory.Internal, "Main Thread", "Frame overview", true);
        AddMetric(ProfilerCategory.Internal, "Render Thread", "Frame overview", true);
        AddMetric(ProfilerCategory.Internal, "GPU Frame Time", "Frame overview", true);
        AddMetric(ProfilerCategory.Internal, "Gfx.WaitForPresentOnGfxThread", "Frame overview", true);

        AddMetric(ProfilerCategory.Scripts, "BehaviourUpdate", "Scripts", true);
        AddMetric(ProfilerCategory.Scripts, "FixedBehaviourUpdate", "Scripts", true);

        AddMetric(ProfilerCategory.Physics, "Physics.Simulate", "Physics / collisions", true);
        AddMetric(ProfilerCategory.Physics, "Physics.Processing", "Physics / collisions", true);
        AddMetric(ProfilerCategory.Physics, "Physics.ProcessReports", "Physics / collisions", true);
        AddMetric(ProfilerCategory.Physics, "Physics.SyncColliderTransform", "Physics / collisions", true);

        AddMetric(ProfilerCategory.Render, "Camera.Render", "Rendering", true);
        AddMetric(ProfilerCategory.Render, "RenderLoop.Draw", "Rendering", true);
        AddMetric(ProfilerCategory.Render, "RenderPipelineManager.DoRenderLoop_Internal", "Rendering", true);
        AddMetric(ProfilerCategory.Render, "ScriptableRenderContext.Submit", "Rendering", true);
        AddMetric(ProfilerCategory.Render, "Shadows.RenderShadowMap", "Lighting / shadows", true);
        AddMetric(ProfilerCategory.Render, "RenderShadowMaps", "Lighting / shadows", true);
        AddMetric(ProfilerCategory.Render, "MainLightShadowCasterPass", "Lighting / shadows", true);
        AddMetric(ProfilerCategory.Render, "AdditionalLightsShadowCasterPass", "Lighting / shadows", true);

        AddMetric(ProfilerCategory.Memory, "GC Allocated In Frame", "Memory", false);
        AddDiscoveredMetrics();
    }

    private void AddMetric(ProfilerCategory category, string markerName, string group, bool isTime)
    {
        ProfilerRecorder recorder = ProfilerRecorder.StartNew(category, markerName, 1);
        if (!recorder.Valid)
        {
            recorder.Dispose();
            unavailableMetrics.Add(markerName);
            return;
        }

        metrics.Add(new Metric(markerName, group, isTime, recorder));
        metricNames.Add(markerName);
    }

    private void AddDiscoveredMetrics()
    {
        List<ProfilerRecorderHandle> handles = new List<ProfilerRecorderHandle>();
        ProfilerRecorderHandle.GetAvailable(handles);

        Dictionary<string, int> groupCounts = new Dictionary<string, int>();
        foreach (ProfilerRecorderHandle handle in handles)
        {
            ProfilerRecorderDescription description = ProfilerRecorderHandle.GetDescription(handle);
            if (description.UnitType != ProfilerMarkerDataUnit.TimeNanoseconds)
            {
                continue;
            }

            string group = GetGroupForMarker(description.Category.Name, description.Name);
            if (group == null || metricNames.Contains(description.Name))
            {
                continue;
            }

            int count = groupCounts.TryGetValue(group, out int existingCount) ? existingCount : 0;
            if (count >= 6)
            {
                continue;
            }

            ProfilerRecorder recorder = new ProfilerRecorder(handle, 1, ProfilerRecorderOptions.StartImmediately);
            if (!recorder.Valid)
            {
                recorder.Dispose();
                continue;
            }

            metrics.Add(new Metric(description.Name, group, true, recorder));
            metricNames.Add(description.Name);
            groupCounts[group] = count + 1;
        }
    }

    private string GetGroupForMarker(string categoryName, string markerName)
    {
        if (ContainsAny(markerName, "shadow", "lighting", "light"))
        {
            return "Lighting / shadows";
        }

        if (ContainsAny(markerName, "physics", "collision", "collider", "contact"))
        {
            return "Physics / collisions";
        }

        if (categoryName == "Scripts" || ContainsAny(markerName, "behaviourupdate", "scriptrun", "coroutine"))
        {
            return "Scripts";
        }

        if (ContainsAny(markerName, "render", "camera"))
        {
            return "Rendering";
        }

        return null;
    }

    private bool ContainsAny(string value, params string[] terms)
    {
        foreach (string term in terms)
        {
            if (value.IndexOf(term, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
        }

        return false;
    }

    private void Update()
    {
        float frameMilliseconds = Time.unscaledDeltaTime * 1000f;
        totalFrameMilliseconds += frameMilliseconds;
        worstFrameMilliseconds = Mathf.Max(worstFrameMilliseconds, frameMilliseconds);
        frameCount++;
        if (frameMilliseconds > frameBudgetMilliseconds)
        {
            framesOverBudget++;
        }

        foreach (Metric metric in metrics)
        {
            metric.Sample();
        }

        reportTimer += Time.unscaledDeltaTime;
        if (reportTimer >= reportInterval)
        {
            LogReport();
        }
    }

    private void LogReport()
    {
        float averageFrameMilliseconds = frameCount > 0 ? totalFrameMilliseconds / frameCount : 0;
        float averageFps = averageFrameMilliseconds > 0 ? 1000f / averageFrameMilliseconds : 0;

        StringBuilder report = new StringBuilder(512);
        report.AppendLine($"Performance snapshot ({reportTimer:F1}s): average {averageFps:F1} FPS / {averageFrameMilliseconds:F2} ms, worst frame {worstFrameMilliseconds:F2} ms, over {frameBudgetMilliseconds:F2} ms: {framesOverBudget}/{frameCount} frames.");
        report.AppendLine("Likely performance hotspots by area (largest available marker in each group; nested marker times are not additive):");

        string[] groups =
        {
            "Scripts",
            "Physics / collisions",
            "Lighting / shadows",
            "Rendering",
            "Frame overview"
        };

        List<KeyValuePair<string, Metric>> topMetrics = new List<KeyValuePair<string, Metric>>();
        List<string> groupsWithoutMetrics = new List<string>();
        foreach (string group in groups)
        {
            Metric topMetric = null;
            float topAverage = 0;
            foreach (Metric metric in metrics)
            {
                if (!metric.isTime || metric.group != group || metric.samples == 0)
                {
                    continue;
                }

                float average = metric.total / metric.samples;
                if (topMetric == null || average > topAverage)
                {
                    topMetric = metric;
                    topAverage = average;
                }
            }

            if (topMetric != null)
            {
                topMetrics.Add(new KeyValuePair<string, Metric>(group, topMetric));
            }
            else
            {
                groupsWithoutMetrics.Add(group);
            }
        }

        topMetrics.Sort((left, right) =>
        {
            float leftAverage = left.Value.total / left.Value.samples;
            float rightAverage = right.Value.total / right.Value.samples;
            return rightAverage.CompareTo(leftAverage);
        });
        foreach (KeyValuePair<string, Metric> topMetric in topMetrics)
        {
            report.AppendLine($"  {topMetric.Key}: {topMetric.Value.Format()}");
        }
        foreach (string group in groupsWithoutMetrics)
        {
            report.AppendLine($"  {group}: no supported marker was available.");
        }

        report.AppendLine("Available marker details:");
        List<Metric> timeMetrics = new List<Metric>();
        foreach (Metric metric in metrics)
        {
            if (metric.isTime && metric.samples > 0)
            {
                timeMetrics.Add(metric);
            }
        }
        timeMetrics.Sort((left, right) =>
        {
            float leftAverage = left.total / left.samples;
            float rightAverage = right.total / right.samples;
            return rightAverage.CompareTo(leftAverage);
        });
        foreach (Metric metric in timeMetrics)
        {
            report.AppendLine($"  [{metric.group}] {metric.Format()}");
        }

        foreach (Metric metric in metrics)
        {
            if (!metric.isTime && metric.samples > 0)
            {
                report.Append("  ");
                report.AppendLine(metric.Format());
            }
        }

        if (unavailableMetrics.Count > 0)
        {
            report.AppendLine($"Unavailable profiler markers on this platform/configuration: {string.Join(", ", unavailableMetrics)}");
        }

        Debug.Log(report.ToString(), this);
        ResetWindow();
    }

    private void ResetWindow()
    {
        reportTimer = 0;
        totalFrameMilliseconds = 0;
        worstFrameMilliseconds = 0;
        frameCount = 0;
        framesOverBudget = 0;
        foreach (Metric metric in metrics)
        {
            metric.Reset();
        }
    }

    private void OnDisable()
    {
        foreach (Metric metric in metrics)
        {
            metric.recorder.Dispose();
        }

        metrics.Clear();
        metricNames.Clear();
    }
}

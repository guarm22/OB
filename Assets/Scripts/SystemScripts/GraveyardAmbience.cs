using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardAmbience : MonoBehaviour {
    
    List<ParticleSystem> ps;
    private bool paused = false;

    public List<AudioSource> audioSources = new List<AudioSource>();
    public List<AudioClip> audioClips = new List<AudioClip>();
    private List<AudioClip> playedClips = new List<AudioClip>();
    public float audioTimer = 60f;
    public AudioSource backgroundNoise;

    public float audioTimerMax = 90f;

    public float audioRandomOffset = 10f;

    [SerializeField]
    private AudioSource waterfall;
    [SerializeField]
    private float waterfallCrossfadeDuration = 0.8f;

    private AudioSource waterfallOverlap;
    private Coroutine waterfallRoutine;
    private bool warnedAboutWaterfall;
    private float waterfallBaseVolume;

    void Awake() {
        ps = new List<ParticleSystem>(GameObject.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None));
        audioTimer = audioTimer + Random.Range(-audioRandomOffset, audioRandomOffset);
        backgroundNoise.volume = PlayerPrefs.GetInt("AmbienceVolume", 50) / 100f;
        audioSources.ForEach(p => p.volume = (PlayerPrefs.GetInt("AmbienceVolume", 50)/100f) - .2f);
        audioSources.ForEach(p => p.playOnAwake = false);
        audioSources.ForEach(p => p.spatialBlend = 1f);
        WaterfallSound();
        if (waterfall != null) {
            waterfallBaseVolume = PlayerPrefs.GetInt("AmbienceVolume",50) / 100f;
            waterfall.volume = waterfallBaseVolume;
            waterfall.playOnAwake = false;
            waterfall.loop = false;
        }
    }

    private void PlaySound() {
        AudioSource source = audioSources[Random.Range(0, audioSources.Count)];
        AudioClip clip = audioClips[Random.Range(0, audioClips.Count)];

        source.clip = clip;
        source.Play();

        //remove clip from list and add to played clips
        //so we dont have repeating sounds too often
        playedClips.Add(clip);
        audioClips.Remove(clip);
        if(audioClips.Count <= 1) {
            audioClips.AddRange(playedClips);
            playedClips.Clear();
        }
    }

    private void WaterfallSound()
    {
        if (waterfall == null || waterfall.clip == null) {
            if (!warnedAboutWaterfall) {
                Debug.LogError("GraveyardAmbience requires a Waterfall AudioSource with an AudioClip assigned.", this);
                warnedAboutWaterfall = true;
            }
            return;
        }

        if (waterfallRoutine != null) {
            return;
        }

        waterfallOverlap = waterfall.gameObject.AddComponent<AudioSource>();
        waterfallOverlap.clip = waterfall.clip;
        waterfallOverlap.outputAudioMixerGroup = waterfall.outputAudioMixerGroup;
        waterfallOverlap.priority = waterfall.priority;
        waterfallOverlap.volume = waterfallBaseVolume;
        waterfallOverlap.pitch = waterfall.pitch;
        waterfallOverlap.panStereo = waterfall.panStereo;
        waterfallOverlap.spatialBlend = waterfall.spatialBlend;
        waterfallOverlap.spatialize = waterfall.spatialize;
        waterfallOverlap.spatializePostEffects = waterfall.spatializePostEffects;
        waterfallOverlap.dopplerLevel = waterfall.dopplerLevel;
        waterfallOverlap.spread = waterfall.spread;
        waterfallOverlap.rolloffMode = waterfall.rolloffMode;
        waterfallOverlap.minDistance = waterfall.minDistance;
        waterfallOverlap.maxDistance = waterfall.maxDistance;
        waterfallOverlap.SetCustomCurve(AudioSourceCurveType.CustomRolloff, waterfall.GetCustomCurve(AudioSourceCurveType.CustomRolloff));
        waterfallOverlap.SetCustomCurve(AudioSourceCurveType.SpatialBlend, waterfall.GetCustomCurve(AudioSourceCurveType.SpatialBlend));
        waterfallOverlap.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, waterfall.GetCustomCurve(AudioSourceCurveType.ReverbZoneMix));
        waterfallOverlap.SetCustomCurve(AudioSourceCurveType.Spread, waterfall.GetCustomCurve(AudioSourceCurveType.Spread));
        waterfallOverlap.bypassEffects = waterfall.bypassEffects;
        waterfallOverlap.bypassListenerEffects = waterfall.bypassListenerEffects;
        waterfallOverlap.bypassReverbZones = waterfall.bypassReverbZones;
        waterfallOverlap.ignoreListenerPause = waterfall.ignoreListenerPause;
        waterfallOverlap.ignoreListenerVolume = waterfall.ignoreListenerVolume;
        waterfallOverlap.mute = waterfall.mute;
        waterfallOverlap.playOnAwake = false;
        waterfallOverlap.loop = false;
        waterfallRoutine = StartCoroutine(LoopWaterfall());
    }

    private IEnumerator LoopWaterfall()
    {
        AudioSource current = waterfall;
        AudioSource next = waterfallOverlap;
        AudioClip clip = waterfall.clip;
        float crossfadeDuration = Mathf.Min(Mathf.Max(waterfallCrossfadeDuration, 0.1f), clip.length * 0.4f);
        float playbackSpeed = Mathf.Max(Mathf.Abs(waterfall.pitch), 0.01f);
        int crossfadeSamples = Mathf.CeilToInt(crossfadeDuration * playbackSpeed * clip.frequency);
        int safetySamples = Mathf.CeilToInt(0.30f * playbackSpeed * clip.frequency);
        int triggerSample = Mathf.Max(0, clip.samples - crossfadeSamples - safetySamples);

        current.time = 0f;
        current.Play();
        current.volume = waterfallBaseVolume;

        while (true) {
            if(PlayerUI.paused) {
                next.Pause();
                current.Pause();
                yield return null;
                continue;
            }

            while (current.isPlaying && current.timeSamples < triggerSample) {
                yield return null;
            }

            if (!current.isPlaying) {
                current.time = 0f;
                current.volume = waterfallBaseVolume;
                current.Play();
                yield return null;
                continue;
            }

            next.clip = clip;
            next.time = 0f;
            next.volume = 0f;
            next.Play();

            float fadeElapsed = 0f;
            while (fadeElapsed < crossfadeDuration) {
                fadeElapsed += Time.unscaledDeltaTime;
                float blend = Mathf.Clamp01(fadeElapsed / crossfadeDuration);
                current.volume = waterfallBaseVolume * (1f - blend);
                next.volume = waterfallBaseVolume * blend;
                yield return null;
            }

            current.Stop();
            current.volume = 0f;
            current.time = 0f;
            next.volume = waterfallBaseVolume;
            AudioSource previous = current;
            current = next;
            next = previous;
        }
    }

    void Update() {
        if(PlayerUI.paused) {
            paused = true;
            foreach(ParticleSystem p in ps) {
                if(p == null) {return;}
                p.Pause();
            }
            return;
        }

        if(paused) {
            paused = false;
            foreach(ParticleSystem p in ps ) {
                if(p == null) {return;}
                p.Play();
            }
        }

        audioTimer += Time.deltaTime;
        if(audioTimer >= audioTimerMax) {
            audioTimer = 0 + Random.Range(-audioRandomOffset/2, audioRandomOffset);
            PlaySound();
        }

    }

    private void OnDestroy() {
        if (waterfall != null) {
            waterfall.Stop();
        }
        if (waterfallOverlap != null) {
            waterfallOverlap.Stop();
            Destroy(waterfallOverlap);
        }
    }
}

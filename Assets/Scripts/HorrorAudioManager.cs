using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HorrorAudioManager : MonoBehaviour
{
    public AudioSource ambienceSource;
    public AudioSource stingSource;

    public AudioClip ambienceClip;
    public AudioClip correctClip;
    public AudioClip wrongClip;

    [Range(0f, 1f)]
    public float ambienceBaseVolume = 0.30f;

    [Range(0f, 1f)]
    public float stingVolume = 0.82f;

    private void Awake()
    {
        SetupSources();
        GenerateClips();
        StartAmbience();
    }

    private void SetupSources()
    {
        if (ambienceSource == null) ambienceSource = gameObject.AddComponent<AudioSource>();
        if (stingSource == null) stingSource = gameObject.AddComponent<AudioSource>();

        ambienceSource.playOnAwake = false;
        ambienceSource.loop = true;
        ambienceSource.volume = ambienceBaseVolume;
        ambienceSource.spatialBlend = 0f;
        ambienceSource.dopplerLevel = 0f;

        stingSource.playOnAwake = false;
        stingSource.loop = false;
        stingSource.volume = stingVolume;
        stingSource.spatialBlend = 0f;
        stingSource.dopplerLevel = 0f;
    }

    private void GenerateClips()
    {
        ambienceClip = GenerateHumClip("Generated_Ambience_Hum", 8f, 70f, 0.145f);
        correctClip = GenerateToneClip("Generated_Correct", 0.18f, 520f, 0.40f);
        wrongClip = GenerateToneClip("Generated_Wrong", 0.55f, 58f, 0.82f);
    }

    private void StartAmbience()
    {
        if (ambienceSource == null || ambienceClip == null) return;
        ambienceSource.clip = ambienceClip;
        ambienceSource.volume = ambienceBaseVolume;
        ambienceSource.Play();
    }

    public void SetTensionByLoop(int loop, bool anomalyActive)
    {
        if (ambienceSource == null) return;

        float targetVolume = Mathf.Clamp(ambienceBaseVolume + loop * 0.006f, ambienceBaseVolume, 0.54f);
        if (anomalyActive) targetVolume += 0.035f;

        ambienceSource.volume = Mathf.Clamp(targetVolume, 0.22f, 0.60f);
        ambienceSource.pitch = Mathf.Clamp(1f + loop * 0.0015f, 1f, 1.06f);
    }

    public void PlayCorrectDecisionSting()
    {
        if (stingSource != null && correctClip != null) stingSource.PlayOneShot(correctClip, 0.42f);
    }

    public void PlayWrongDecisionSting()
    {
        if (stingSource != null && wrongClip != null) stingSource.PlayOneShot(wrongClip, stingVolume);
    }

    private AudioClip GenerateHumClip(string name, float duration, float baseFrequency, float volume)
    {
        int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float low = Mathf.Sin(2f * Mathf.PI * baseFrequency * t);
            float second = Mathf.Sin(2f * Mathf.PI * (baseFrequency * 1.5f) * t) * 0.35f;
            float electrical = Mathf.Sin(2f * Mathf.PI * 50f * t) * 0.12f;
            float pulse = Mathf.Sin(2f * Mathf.PI * 0.42f * t) * 0.14f;
            float noise = Random.Range(-0.035f, 0.035f);
            data[i] = (low * 0.55f + second + electrical + pulse + noise) * volume;
        }

        AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip GenerateToneClip(string name, float duration, float frequency, float volume)
    {
        int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = 1f - Mathf.Clamp01(t / duration);
            float tone = Mathf.Sin(2f * Mathf.PI * frequency * t);
            float harmonic = Mathf.Sin(2f * Mathf.PI * frequency * 2f * t) * 0.25f;
            data[i] = (tone + harmonic) * volume * envelope;
        }

        AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
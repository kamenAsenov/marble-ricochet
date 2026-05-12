using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RPAudioManager : MonoBehaviour
{
    public AudioSource source;

    private AudioClip shootClip;
    private AudioClip glassClip;
    private AudioClip bumperClip;
    private AudioClip winClip;
    private AudioClip levelStartClip;
    private AudioClip coinClip;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = 0.85f;

        shootClip = CreateTone("Shoot", 0.12f, 220f, 0.25f, 1.0f);
        glassClip = CreateGlass("Glass", 0.22f, 0.42f);
        bumperClip = CreateTone("Bumper", 0.14f, 110f, 0.38f, 0.6f);
        winClip = CreateWin("Win", 0.55f);
        levelStartClip = CreateTone("LevelStart", 0.16f, 420f, 0.16f, 1.2f);
        coinClip = CreateTone("Coins", 0.16f, 860f, 0.22f, 1.25f);
    }

    public void PlayShoot() { Play(shootClip, 0.65f); }
    public void PlayGlassPop() { Play(glassClip, 0.95f); }
    public void PlayBumper() { Play(bumperClip, 0.75f); }
    public void PlayWin() { Play(winClip, 0.9f); Play(coinClip, 0.5f); }
    public void PlayLevelStart() { Play(levelStartClip, 0.42f); }

    private void Play(AudioClip clip, float volume)
    {
        if (source != null && clip != null)
        {
            source.PlayOneShot(clip, volume);
        }
    }

    private AudioClip CreateTone(string name, float duration, float frequency, float volume, float pitchBend)
    {
        int sampleRate = 44100;
        int samples = Mathf.CeilToInt(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = 1f - Mathf.Clamp01(t / duration);
            float freq = frequency * Mathf.Lerp(1f, pitchBend, t / duration);
            data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * volume * envelope;
        }

        AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateGlass(string name, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = Mathf.CeilToInt(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = Mathf.Pow(1f - Mathf.Clamp01(t / duration), 2.4f);
            float toneA = Mathf.Sin(2f * Mathf.PI * 1500f * t);
            float toneB = Mathf.Sin(2f * Mathf.PI * 2400f * t) * 0.55f;
            float noise = Random.Range(-1f, 1f) * 0.6f;
            data[i] = (toneA + toneB + noise) * volume * envelope;
        }

        AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateWin(string name, float duration)
    {
        int sampleRate = 44100;
        int samples = Mathf.CeilToInt(duration * sampleRate);
        float[] data = new float[samples];

        float[] notes = new float[] { 440f, 554f, 660f };

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            int noteIndex = Mathf.Clamp(Mathf.FloorToInt(t / (duration / 3f)), 0, notes.Length - 1);
            float localT = (t - noteIndex * (duration / 3f)) / (duration / 3f);
            float envelope = Mathf.Sin(localT * Mathf.PI);
            data[i] = Mathf.Sin(2f * Mathf.PI * notes[noteIndex] * t) * 0.24f * envelope;
        }

        AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
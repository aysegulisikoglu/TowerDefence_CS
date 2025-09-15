using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public static AudioController I { get; private set; }

    [Header("Mixer Groups")]
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup uiGroup;

    [Header("SFX Clips")]
    public AudioClip shootBlue, shootOrange, shootPurple;
    public AudioClip projectileHit, meleeSwing;
    public AudioClip playerHurt, playerDeath, respawn;
    public AudioClip enemyDie, waveStart, waveCall;

    [Header("Music (optional)")]
    public AudioSource musicSource;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayAt(AudioClip clip, Vector3 pos, float vol = 1f, float pitchJitter = 0f)
    {
        if (!clip) return;
        var go = new GameObject("SFX_3D");
        go.transform.position = pos;
        var src = go.AddComponent<AudioSource>();
        if (sfxGroup) src.outputAudioMixerGroup = sfxGroup;
        src.spatialBlend = 1f;
        if (pitchJitter != 0f) src.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
        src.PlayOneShot(clip, vol);
        Destroy(go, clip.length + 0.1f);
    }

    public void Play2D(AudioClip clip, float vol = 1f, float pitchJitter = 0f)
    {
        if (!clip) return;
        var go = new GameObject("SFX_2D");
        var src = go.AddComponent<AudioSource>();
        if (sfxGroup) src.outputAudioMixerGroup = sfxGroup;
        src.spatialBlend = 0f;
        if (pitchJitter != 0f) src.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
        src.PlayOneShot(clip, vol);
        Destroy(go, clip.length + 0.1f);
    }

    public void PlayUI(AudioClip clip, float vol = 1f)
    {
        if (!clip) return;
        var go = new GameObject("SFX_UI");
        var src = go.AddComponent<AudioSource>();
        if (uiGroup) src.outputAudioMixerGroup = uiGroup;
        src.spatialBlend = 0f;
        src.PlayOneShot(clip, vol);
        Destroy(go, clip.length + 0.1f);
    }

    public void SetMusic(AudioClip clip, bool loop = true, float volume = 0.5f)
    {
        if (!musicSource) musicSource = gameObject.AddComponent<AudioSource>();
        if (musicGroup) musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = volume;
        musicSource.Play();
    }
}

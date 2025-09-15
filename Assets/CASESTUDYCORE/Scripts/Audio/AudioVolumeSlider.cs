using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioVolumeSlider : MonoBehaviour
{
    public AudioMixer mixer;
    public string exposedParam = "MusicVol";
    public Slider slider;

    void Start()
    {
        if (!slider) slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(SetVolume);

        SetVolume(slider.value);
    }

    public void SetVolume(float v)
    {
        if (!mixer) return;
        float db = Mathf.Log10(Mathf.Clamp(v, 0.0001f, 1f)) * 20f;
        mixer.SetFloat(exposedParam, db);
    }
}

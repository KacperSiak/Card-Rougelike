using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider soundSlider;
    public Slider musicSlider;

    [Header("Mixer")]
    public AudioMixer audioMixer;

    private void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("Master", 1f);
        soundSlider.value = PlayerPrefs.GetFloat("Sound", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("Music", 1f);

        SetMasterVolume(masterSlider.value);
        SetSoundVolume(soundSlider.value);
        SetMusicVolume(musicSlider.value);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        soundSlider.onValueChanged.AddListener(SetSoundVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("Master", value);
    }

    public void SetSoundVolume(float value)
    {
        audioMixer.SetFloat("Sound", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("Sound", value);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("Music", value);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
        masterSlider.onValueChanged.RemoveListener(SetMasterVolume);
        soundSlider.onValueChanged.RemoveListener(SetSoundVolume);
        musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
    }
}

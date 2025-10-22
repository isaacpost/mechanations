using UnityEngine;
using UnityEngine.UI;

// Used in the settings menu to set volume of sfx and music
public class VolumeControl : MonoBehaviour
{
    [SerializeField] 
    private Slider musicVolumeSlider; // Reference to the UI slider

    [SerializeField]
    private Slider SFXVolumeSlider; // Reference to the UI slider

    void Start()
    {
        musicVolumeSlider.value = SFXManager.Instance.GetMusicVolume();
        SFXVolumeSlider.value = SFXManager.Instance.GetSFXVolume();

        musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolume);
        SFXVolumeSlider.onValueChanged.AddListener(UpdateSFXVolume);
    }

    void UpdateMusicVolume(float value)
    {
        // Update the audio source's volume
         SFXManager.Instance.SetMusicVolume(value);
    }

    void UpdateSFXVolume(float value)
    {
        // Update the audio source's volume
        SFXManager.Instance.SetSFXVolume(value);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public string volumeName;
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey(volumeName))
        {
            SetVolume(PlayerPrefs.GetFloat(volumeName));
            slider.value = PlayerPrefs.GetFloat(volumeName);
        }
    }

    public void SetVolume(float value)
    {
        audioMixer.SetFloat(volumeName, Mathf.Log10(value)*20);
        PlayerPrefs.SetFloat(volumeName, value);
    }
}

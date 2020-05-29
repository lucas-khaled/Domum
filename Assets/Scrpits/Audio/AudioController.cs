using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioController : MonoBehaviour
{
    [SerializeField]
    private string parametroAudio;
    [SerializeField]
    private AudioMixer mixer;
    [SerializeField]
    private Slider slider;
    private void Start()
    {
        mixer.SetFloat(parametroAudio, slider.value);
    }
    public void Volume(float valorSlider)
    {
        mixer.SetFloat(parametroAudio, valorSlider);
    }
}

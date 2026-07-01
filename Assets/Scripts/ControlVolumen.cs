using UnityEngine;
using UnityEngine.Audio;

public class ControlVolumen : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mezcladorAudio;

    public void CambiarVolumenMusica(float volumen)
    {
        mezcladorAudio.SetFloat(
            "VolumenMusica",
            volumen
        );
    }

    public void CambiarVolumenSFX(float volumen)
    {
        mezcladorAudio.SetFloat(
            "VolumenSFX",
            volumen
        );
    }
}

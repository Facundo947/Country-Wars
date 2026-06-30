using UnityEngine;

public class SonidoVictoria : MonoBehaviour
{
    private void OnEnable()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirSonidoVictoria();
        }
    }
}
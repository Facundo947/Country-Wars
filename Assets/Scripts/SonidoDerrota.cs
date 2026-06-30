using UnityEngine;

public class SonidoDerrota : MonoBehaviour
{
    private void OnEnable()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirSonidoDerrota();
        }
    }
}
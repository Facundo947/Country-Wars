using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SonidoBoton : MonoBehaviour
{
    private Button boton;

    private void Awake()
    {
        boton = GetComponent<Button>();
        boton.onClick.AddListener(ReproducirClick);
    }

    private void ReproducirClick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirSonidoBoton();
        }
    }

    private void OnDestroy()
    {
        if (boton != null)
        {
            boton.onClick.RemoveListener(ReproducirClick);
        }
    }
}
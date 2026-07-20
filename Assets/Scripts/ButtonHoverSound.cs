using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirSonidoHover();
        }
        else
        {
            Debug.LogWarning("No se encontró el AudioManager.");
        }
    }
}
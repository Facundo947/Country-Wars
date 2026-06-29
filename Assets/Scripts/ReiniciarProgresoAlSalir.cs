using UnityEngine;

public class ReiniciarProgresoAlSalir : MonoBehaviour
{
    private const string ClaveProgreso = "PaisDesbloqueado";

    private static ReiniciarProgresoAlSalir instancia;

    private void Awake()
    {
        // Evita que se duplique al cambiar de escena.
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        BorrarProgreso();
    }

    private void BorrarProgreso()
    {
        PlayerPrefs.DeleteKey(ClaveProgreso);
        PlayerPrefs.Save();

        Debug.Log("El progreso de países fue reiniciado.");
    }

    public void SalirDelJuego()
    {
        BorrarProgreso();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
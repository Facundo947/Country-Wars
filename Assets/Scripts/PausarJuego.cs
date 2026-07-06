using UnityEngine;
using UnityEngine.SceneManagement;

public class PausarJuego : MonoBehaviour
{
    [Header("Elementos de pausa")]
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;
    [SerializeField] private GameObject menuOpciones;

    private void Start()
    {
        Time.timeScale = 1f;

        if (botonPausa != null)
        {
            botonPausa.SetActive(true);
        }

        if (menuPausa != null)
        {
            menuPausa.SetActive(false);
        }

        if (menuOpciones != null)
        {
            menuOpciones.SetActive(false);
        }
    }

    public void Pausa()
    {
        Time.timeScale = 0f;

        if (botonPausa != null)
        {
            botonPausa.SetActive(false);
        }

        if (menuPausa != null)
        {
            menuPausa.SetActive(true);
        }

        if (menuOpciones != null)
        {
            menuOpciones.SetActive(false);
        }
    }

    public void Reanudar()
    {
        Debug.Log("BOTÓN REANUDAR EJECUTADO");

        Time.timeScale = 1f;

        if (botonPausa != null)
        {
            botonPausa.SetActive(true);
        }

        if (menuPausa != null)
        {
            menuPausa.SetActive(false);
        }

        if (menuOpciones != null)
        {
            menuOpciones.SetActive(false);
        }
    }

    public void AbrirOpciones()
    {
        if (menuPausa == null || menuOpciones == null)
        {
            Debug.LogError(
                "Falta asignar MenuPausa o MenuOpcionesPausa " +
                "en el componente PausarJuego."
            );

            return;
        }

        menuPausa.SetActive(false);
        menuOpciones.SetActive(true);
    }

    public void VolverAPausa()
    {
        Debug.Log("BOTÓN VOLVER EJECUTADO");

        if (menuOpciones != null)
        {
            menuOpciones.SetActive(false);
        }

        if (menuPausa != null)
        {
            menuPausa.SetActive(true);
        }
    }

    public void Reintentar()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void IrMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu Principal");
    }

    public void Cerrar()
    {
        Debug.Log("Cerrando juego");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class BanderaAlemania : MonoBehaviour
{
    // Cambia "BarraDeVidaJapon" por el nombre de tu script de barra italiano si creaste uno nuevo
    // Si usas el mismo de japon, dejalo como BarraDeVidaJapon o usa un Slider normal.
    [SerializeField] private BarraDeVidaAlemania barraDeVida;

    [Header("Vida")]
    public int vidaMaxima = 100;
    private int vidaActual;

    [Header("UI de Victoria")]
    public GameObject pantallaVictoria;
    public GameObject interfazDeJuego;

    void Start()
    {
        vidaActual = vidaMaxima;

        if (pantallaVictoria != null)
        {
            pantallaVictoria.SetActive(false);
        }

        if (barraDeVida != null)
        {
            // Asegurate de que tu script de barra tenga esta funcion Inicializar
            barraDeVida.InicializarBarraDeVida(vidaMaxima);
        }
    }

    public void RecibirDanio(int danio)
    {
        vidaActual -= danio;

        if (barraDeVida != null)
        {
            barraDeVida.CambiarVidaActual(vidaActual);
        }

        if (vidaActual <= 0)
        {
            Victoria();
        }
    }

    void Victoria()
    {
        if (pantallaVictoria != null)
        {
            pantallaVictoria.SetActive(true);

            if (interfazDeJuego != null)
            {
                interfazDeJuego.SetActive(false);
            }

            Time.timeScale = 0f; // Pausa el juego al ganar
        }
    }

    public void SiguienteNivel()
    {
        Time.timeScale = 1f; // Reanuda el tiempo antes de cambiar nivel

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex + 1
        );
    }
}
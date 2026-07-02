using UnityEngine;
using UnityEngine.SceneManagement;

public class BanderaArgentina : MonoBehaviour
{
    [SerializeField] private BarradeVida barraDeVida;

    [Header("Vida")]
    public int vidaMaxima = 100;
    private int vidaActual;
    private bool derrotaActivada;

    [Header("UI de Derrota")]
    public GameObject pantallaDerrota;

    private void Start()
    {
        vidaActual = vidaMaxima;
        derrotaActivada = false;

        if (pantallaDerrota != null)
        {
            pantallaDerrota.SetActive(false);
        }

        if (barraDeVida != null)
        {
            barraDeVida.InicializarBarraDeVida(vidaMaxima);
        }
    }

    public void RecibirDanio(int danio)
    {
        if (derrotaActivada)
        {
            return;
        }

        vidaActual -= danio;
        vidaActual = Mathf.Max(vidaActual, 0);

        if (barraDeVida != null)
        {
            barraDeVida.CambiarVidaActual(vidaActual);
        }

        if (vidaActual <= 0)
        {
            ActivarDerrota();
        }
    }

    private void ActivarDerrota()
    {
        if (derrotaActivada)
        {
            return;
        }

        derrotaActivada = true;

        AudioManager.Instance?.ReproducirSonidoDerrota();

        if (pantallaDerrota != null)
        {
            pantallaDerrota.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void Reintentar()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
            );
    }
}
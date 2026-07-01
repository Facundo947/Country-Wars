using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Fuentes de audio")]
    [SerializeField] private AudioSource fuenteMusica;
    [SerializeField] private AudioSource fuenteEfectos;

    [Header("Músicas")]
    [SerializeField] private AudioClip musicaMenu;
    [SerializeField] private AudioClip musicaGameplay;

    [Header("Efectos")]
    [SerializeField] private AudioClip sonidoBoton;
    [SerializeField] private AudioClip sonidoVictoria;
    [SerializeField] private AudioClip sonidoDerrota;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded += AlCargarEscena;
        }
    }

    private void Start()
    {
        ConfigurarMusicaSegunEscena(
            SceneManager.GetActiveScene().name
        );
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    private void AlCargarEscena(
        Scene escena,
        LoadSceneMode modo
    )
    {
        ConfigurarMusicaSegunEscena(escena.name);
    }

    private void ConfigurarMusicaSegunEscena(
        string nombreEscena
    )
    {
        if (nombreEscena == "Menu Principal" ||
            nombreEscena == "Selector De Niveles")
        {
            ReproducirMusicaMenu();
        }
        else if (nombreEscena.StartsWith("Nivel "))
        {
            ReproducirMusicaGameplay();
        }
    }

    public void ReproducirMusicaMenu()
    {
        // Detiene victoria o derrota si todavía están sonando.
        if (fuenteEfectos != null)
        {
            fuenteEfectos.Stop();
            fuenteEfectos.clip = null;
        }

        ReproducirMusica(musicaMenu);
    }

    public void ReproducirMusicaGameplay()
    {
        // Detiene victoria o derrota si todavía están sonando.
        if (fuenteEfectos != null)
        {
            fuenteEfectos.Stop();
            fuenteEfectos.clip = null;
        }

        ReproducirMusica(musicaGameplay);
    }

    private void ReproducirMusica(AudioClip nuevaMusica)
    {
        if (fuenteMusica == null || nuevaMusica == null)
        {
            Debug.LogWarning(
                "Falta asignar una fuente o una música."
            );

            return;
        }

        // Si la misma música ya está sonando,
        // continúa y no empieza desde cero.
        if (fuenteMusica.clip == nuevaMusica &&
            fuenteMusica.isPlaying)
        {
            return;
        }

        fuenteMusica.Stop();
        fuenteMusica.clip = nuevaMusica;
        fuenteMusica.loop = true;
        fuenteMusica.Play();
    }

    public void ReproducirSonidoBoton()
    {
        if (fuenteEfectos != null &&
            sonidoBoton != null)
        {
            fuenteEfectos.PlayOneShot(sonidoBoton);
        }
    }

    public void ReproducirSonidoVictoria()
    {
        if (fuenteMusica != null)
        {
            fuenteMusica.Stop();
            fuenteMusica.clip = null;
        }

        if (fuenteEfectos != null)
        {
            fuenteEfectos.Stop();
            fuenteEfectos.clip = null;
        }

        if (fuenteEfectos != null && sonidoVictoria != null)
        {
            fuenteEfectos.clip = sonidoVictoria;
            fuenteEfectos.loop = false;
            fuenteEfectos.Play();
        }
    }

    public void ReproducirSonidoDerrota()
    {
        if (fuenteMusica != null)
        {
            fuenteMusica.Stop();
            fuenteMusica.clip = null;
        }

        if (fuenteEfectos != null)
        {
            fuenteEfectos.Stop();
            fuenteEfectos.clip = null;
        }

        if (fuenteEfectos != null && sonidoDerrota != null)
        {
            fuenteEfectos.clip = sonidoDerrota;
            fuenteEfectos.loop = false;
            fuenteEfectos.Play();
        }
    }
}
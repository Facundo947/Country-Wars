using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Fuentes de audio")]
    [SerializeField] private AudioSource fuenteMusica;
    [SerializeField] private AudioSource fuenteEfectos;

    [Header("Sonidos del menú")]
    [SerializeField] private AudioClip musicaMenu;
    [SerializeField] private AudioClip sonidoBoton;

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

    private void Start()
    {
        ReproducirMusicaMenu();
    }

    public void ReproducirMusicaMenu()
    {
        if (fuenteMusica == null || musicaMenu == null)
        {
            Debug.LogWarning(
                "Falta asignar la fuente o la música del menú."
            );

            return;
        }

        if (fuenteMusica.clip == musicaMenu &&
            fuenteMusica.isPlaying)
        {
            return;
        }

        fuenteMusica.Stop();
        fuenteMusica.clip = musicaMenu;
        fuenteMusica.loop = true;
        fuenteMusica.Play();
    }

    public void ReproducirSonidoBoton()
    {
        if (fuenteEfectos == null || sonidoBoton == null)
        {
            Debug.LogWarning(
                "Falta asignar la fuente o el sonido del botón."
            );

            return;
        }

        fuenteEfectos.PlayOneShot(sonidoBoton);
    }
}
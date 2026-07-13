using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum EstadoJuego
    {
        Planeacion,
        Ejecucion
    }

    [Header("Estado del juego")]
    public EstadoJuego estadoActual = EstadoJuego.Planeacion;

    [Header("UI")]
    [SerializeField] private GameObject botonComenzar;

    [Header("Configuración de tropas")]
    [SerializeField] private int cantidadTropasNecesarias = 6;

    private int tropasColocadas = 0;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        if (botonComenzar != null)
        {
            botonComenzar.SetActive(false);
        }
    }


    // Se llama cada vez que el jugador coloca una tropa
    public void RegistrarTropaColocada()
    {
        tropasColocadas++;

        // Ahora necesita 6 tropas para mostrar el botón
        if (tropasColocadas >= cantidadTropasNecesarias && botonComenzar != null)
        {
            botonComenzar.SetActive(true);
        }
    }


    // Devuelve true si el juego está en Planeación
    public bool EnPlaneacion()
    {
        return estadoActual == EstadoJuego.Planeacion;
    }


    // Devuelve true si el juego está en Ejecución
    public bool EnEjecucion()
    {
        return estadoActual == EstadoJuego.Ejecucion;
    }


    // Se llama desde el botón COMENZAR
    public void ComenzarBatalla()
    {
        estadoActual = EstadoJuego.Ejecucion;

        if (botonComenzar != null)
        {
            botonComenzar.SetActive(false);
        }

        Debug.Log("¡Comenzó la batalla!");
    }
}
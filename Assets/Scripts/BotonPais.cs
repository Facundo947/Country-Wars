using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class BotonPais : MonoBehaviour
{
    private const string ClaveProgreso = "PaisDesbloqueado";

    [Header("Información del país")]
    [SerializeField] private string nombreEscena;
    [SerializeField] private int numeroPais = 1;

    [Header("Candado")]
    [SerializeField] private GameObject candado;

    private Button boton;

    private void Awake()
    {
        boton = GetComponent<Button>();
    }

    private void Start()
    {
        ActualizarEstado();

        boton.onClick.AddListener(AbrirNivel);
    }

    private void ActualizarEstado()
    {
        int paisDesbloqueado =
            PlayerPrefs.GetInt(ClaveProgreso, 1);

        bool estaDesbloqueado =
            numeroPais <= paisDesbloqueado;

        boton.interactable = estaDesbloqueado;

        if (candado != null)
        {
            candado.SetActive(!estaDesbloqueado);
        }
    }

    private void AbrirNivel()
    {
        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogWarning(
                "Falta escribir el nombre de la escena en " +
                gameObject.name
            );

            return;
        }

        SceneManager.LoadScene(nombreEscena);
    }

    private void OnDestroy()
    {
        if (boton != null)
        {
            boton.onClick.RemoveListener(AbrirNivel);
        }
    }
}
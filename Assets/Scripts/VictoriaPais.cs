using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoriaPais : MonoBehaviour
{
    [Header("Progreso")]
    [SerializeField] private int siguientePais = 2;

    [Header("Selector")]
    [SerializeField] private string escenaSelector = "Selector De Niveles";

    public void CompletarNivel()
    {
        int progresoActual =
            PlayerPrefs.GetInt("PaisDesbloqueado", 1);

        if (siguientePais > progresoActual)
        {
            PlayerPrefs.SetInt(
                "PaisDesbloqueado",
                siguientePais
            );

            PlayerPrefs.Save();
        }

        SceneManager.LoadScene(escenaSelector);
    }
}
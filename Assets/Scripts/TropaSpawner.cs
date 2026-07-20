using UnityEngine;

public class SpawnerConMouse : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject prefabGaucho;
    [SerializeField] private GameObject prefabGuarani;
    [SerializeField] private GameObject prefabEspadachinArgentino;
    [SerializeField] private GameObject prefabSenoraEmpanada;

    [Header("Tropas iniciales")]
    [SerializeField] private int limiteAliados = 6;

    private GameObject tropaSeleccionada;
    private bool colocandoTropa;
    private int aliadosColocados;

    public void ActivarColocacionGaucho()
    {
        SeleccionarTropa(prefabGaucho);
    }

    public void ActivarColocacionGuarani()
    {
        SeleccionarTropa(prefabGuarani);
    }

    public void ActivarColocacionEspadachinArgentino()
    {
        SeleccionarTropa(
            prefabEspadachinArgentino
        );
    }

    public void ActivarColocacionSenoraEmpanada()
    {
        SeleccionarTropa(
            prefabSenoraEmpanada
        );
    }

    public bool PuedeColocarMasAliados()
    {
        // Después de comenzar la batalla,
        // permite comprar más tropas.
        if (GameManager.Instance != null &&
            GameManager.Instance.EnEjecucion())
        {
            return true;
        }

        // Antes de comenzar, solo permite 6.
        return aliadosColocados < limiteAliados;
    }

    private void SeleccionarTropa(GameObject prefab)
    {
        if (!PuedeColocarMasAliados())
        {
            Debug.Log(
                "Ya colocaste las 6 tropas iniciales. " +
                "Presioná COMENZAR."
            );

            colocandoTropa = false;
            tropaSeleccionada = null;
            return;
        }

        if (prefab == null)
        {
            Debug.LogWarning(
                "Falta asignar el prefab de la tropa."
            );

            return;
        }

        tropaSeleccionada = prefab;
        colocandoTropa = true;
    }

    private void Update()
    {
        if (!colocandoTropa)
        {
            return;
        }

        if (!PuedeColocarMasAliados())
        {
            colocandoTropa = false;
            tropaSeleccionada = null;
            return;
        }

        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        if (Camera.main == null)
        {
            Debug.LogWarning(
                "No se encontró la cámara principal."
            );

            colocandoTropa = false;
            return;
        }

        Vector3 posicionMouse =
            Camera.main.ScreenToWorldPoint(
                Input.mousePosition
            );

        posicionMouse.z = 0f;

        Instantiate(
            tropaSeleccionada,
            posicionMouse,
            Quaternion.identity
        );

        aliadosColocados++;

        // Solo registra las primeras tropas
        // durante la planeación.
        if (GameManager.Instance != null &&
            GameManager.Instance.EnPlaneacion())
        {
            GameManager.Instance
                .RegistrarTropaColocada();
        }

        tropaSeleccionada = null;
        colocandoTropa = false;
    }
}
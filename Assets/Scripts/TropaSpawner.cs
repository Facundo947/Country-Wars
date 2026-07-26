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

    [Header("Zona permitida para colocar tropas")]
    [SerializeField] private float limiteIzquierdo = -8f;
    [SerializeField] private float limiteDerecho = 1f;
    [SerializeField] private float alturaMinima = -4f;
    [SerializeField] private float alturaMaxima = -1f;

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
        SeleccionarTropa(prefabEspadachinArgentino);
    }

    public void ActivarColocacionSenoraEmpanada()
    {
        SeleccionarTropa(prefabSenoraEmpanada);
    }

    public bool PuedeColocarMasAliados()
    {
        // Durante la ejecución permite comprar más tropas.
        if (GameManager.Instance != null &&
            GameManager.Instance.EnEjecucion())
        {
            return true;
        }

        // Durante la planeación solamente permite 6.
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
            tropaSeleccionada = null;
            return;
        }

        Vector3 posicionMouse =
            Camera.main.ScreenToWorldPoint(
                Input.mousePosition
            );

        posicionMouse.z = 0f;

        bool dentroDeZona =
            posicionMouse.x >= limiteIzquierdo &&
            posicionMouse.x <= limiteDerecho &&
            posicionMouse.y >= alturaMinima &&
            posicionMouse.y <= alturaMaxima;

        if (!dentroDeZona)
        {
            Debug.Log(
                "No podés colocar tropas en esa zona."
            );

            // No cancela la compra.
            // Podés volver a hacer clic en una zona válida.
            return;
        }

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

    private void OnDrawGizmosSelected()
    {
        Vector3 centro = new Vector3(
            (limiteIzquierdo + limiteDerecho) / 2f,
            (alturaMinima + alturaMaxima) / 2f,
            0f
        );

        Vector3 tamanio = new Vector3(
            limiteDerecho - limiteIzquierdo,
            alturaMaxima - alturaMinima,
            0f
        );

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(centro, tamanio);
    }
}
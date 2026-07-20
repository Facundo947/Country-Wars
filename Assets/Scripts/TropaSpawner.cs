using UnityEngine;

public class SpawnerConMouse : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject prefabGaucho;
    [SerializeField] private GameObject prefabGuarani;
    [SerializeField] private GameObject prefabEspadachinArgentino;
    [SerializeField] private GameObject prefabSenoraEmpanada;

    [Header("Límite de aliados")]
    [SerializeField] private int limiteAliados = 6;

    private GameObject tropaSeleccionada;
    private bool colocandoTropa = false;
    private int aliadosColocados = 0;

    // BOTÓN GAUCHO
    public void ActivarColocacionGaucho()
    {
        SeleccionarTropa(prefabGaucho);
    }

    // BOTÓN GUARANÍ
    public void ActivarColocacionGuarani()
    {
        SeleccionarTropa(prefabGuarani);
    }

    // BOTÓN ESPADACHÍN ARGENTINO
    public void ActivarColocacionEspadachinArgentino()
    {
        SeleccionarTropa(prefabEspadachinArgentino);
    }

    // BOTÓN SEÑORA EMPANADA
    public void ActivarColocacionSenoraEmpanada()
    {
        SeleccionarTropa(prefabSenoraEmpanada);
    }

    private void SeleccionarTropa(GameObject prefab)
    {
        if (aliadosColocados >= limiteAliados)
        {
            Debug.Log("Ya alcanzaste el límite de 6 aliados.");
            colocandoTropa = false;
            return;
        }

        if (prefab == null)
        {
            Debug.LogWarning("Falta asignar el prefab de la tropa.");
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

        if (Input.GetMouseButtonDown(0))
        {
            if (aliadosColocados >= limiteAliados)
            {
                Debug.Log("No podés colocar más aliados.");
                colocandoTropa = false;
                return;
            }

            if (Camera.main == null)
            {
                Debug.LogWarning("No se encontró la cámara principal.");
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

            Debug.Log(
                "Aliados colocados: " +
                aliadosColocados +
                " / " +
                limiteAliados
            );

            // Le avisa al GameManager que se colocó una tropa.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegistrarTropaColocada();
            }

            tropaSeleccionada = null;
            colocandoTropa = false;
        }
    }
}
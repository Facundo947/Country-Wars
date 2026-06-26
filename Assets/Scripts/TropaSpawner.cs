using UnityEngine;

public class SpawnerConMouse : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject prefabGaucho;

    [SerializeField] private GameObject prefabMessi;

    [SerializeField] private GameObject prefabEspadachinArgentino;

    [SerializeField] private GameObject prefabSenoraEmpanada;

    private GameObject tropaSeleccionada;

    private bool colocandoTropa = false;

    // BOTÓN GAUCHO
    public void ActivarColocacionGaucho()
    {
        tropaSeleccionada = prefabGaucho;
        colocandoTropa = true;
    }

    // BOTÓN MESSI
    public void ActivarColocacionMessi()
    {
        tropaSeleccionada = prefabMessi;
        colocandoTropa = true;
    }

    // BOTÓN ESPADACHÍN ARGENTINO
    public void ActivarColocacionEspadachinArgentino()
    {
        tropaSeleccionada = prefabEspadachinArgentino;
        colocandoTropa = true;
    }

    // BOTÓN SEÑORA EMPANADA
    public void ActivarColocacionSenoraEmpanada()
    {
        tropaSeleccionada = prefabSenoraEmpanada;
        colocandoTropa = true;
    }

    private void Update()
    {
        if (colocandoTropa)
        {
            if (Input.GetMouseButtonDown(0))
            {
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

                colocandoTropa = false;
            }
        }
    }
}
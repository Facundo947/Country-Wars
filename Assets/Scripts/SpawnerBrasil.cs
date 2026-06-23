using UnityEngine;
public class SpawnerBrasil : MonoBehaviour
{
    [Header("Configuración del Spawner de Brasil")]
    [SerializeField] private GameObject prefabTropaBrasil;
    [SerializeField] private float tiempoEntreSpawns = 5f;
    [SerializeField] private float retrasoInicial = 2f;

    private float cronometro;

    private void Start()
    {
        cronometro = tiempoEntreSpawns - retrasoInicial;
    }

    private void Update()
    {
        cronometro += Time.deltaTime;

        if (cronometro >= tiempoEntreSpawns)
        {
            SpawnearTropa();
            cronometro = 0f;
        }
    }

    private void SpawnearTropa()
    {
        if (prefabTropaBrasil != null)
        {
            Instantiate(
                prefabTropaBrasil,
                transform.position,
                Quaternion.identity
            );
        }
        else
        {
            Debug.LogWarning(
                "¡Falta asignar el prefab de la tropa de Brasil en el Spawner!"
            );
        }
    }
}

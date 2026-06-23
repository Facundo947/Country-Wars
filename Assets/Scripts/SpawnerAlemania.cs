using UnityEngine;
public class SpawnerAlemania : MonoBehaviour
{
    [Header("Configuración del Spawner de Alemania")]
    [SerializeField] private GameObject prefabTropaAlemania;
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
        if (prefabTropaAlemania != null)
        {
            Instantiate(
                prefabTropaAlemania,
                transform.position,
                Quaternion.identity
            );
        }
        else
        {
            Debug.LogWarning(
                "¡Falta asignar el prefab de la tropa de Alemania en el Spawner!"
            );
        }
    }
}
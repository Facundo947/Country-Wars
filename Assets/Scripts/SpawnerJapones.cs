using UnityEngine;

public class SpawnerJapones : MonoBehaviour
{
    [System.Serializable]
    public class EnemigoPorSpawn
    {
        public Transform puntoSpawn;
        public GameObject prefabEnemigo;
    }

    [Header("Un enemigo por cada punto")]
    [SerializeField]
    private EnemigoPorSpawn[] enemigosPorSpawn;

    private bool enemigosCreados;

    private void Start()
    {
        CrearEnemigosUnaVez();
    }

    private void CrearEnemigosUnaVez()
    {
        if (enemigosCreados)
        {
            return;
        }

        foreach (EnemigoPorSpawn dato in enemigosPorSpawn)
        {
            if (dato.puntoSpawn == null ||
                dato.prefabEnemigo == null)
            {
                Debug.LogWarning(
                    "Falta asignar un punto de spawn o una tropa en " +
                    gameObject.name
                );

                continue;
            }

            Instantiate(
                dato.prefabEnemigo,
                dato.puntoSpawn.position,
                dato.puntoSpawn.rotation
            );
        }

        enemigosCreados = true;
    }
}
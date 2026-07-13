using UnityEngine;

public class Florcurativa : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 10f;

    [Header("Curación")]
    [SerializeField] private int curacion = 20;

    [Header("Tiempo de vida")]
    [SerializeField] private float tiempoDeVida = 5f;

    private void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    private void Update()
    {
        transform.Translate(
            Vector2.left * velocidad * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TropaJaponesa aliado =
            other.GetComponent<TropaJaponesa>();

        if (aliado != null &&
            aliado.EstaHerido())
        {
            aliado.Curar(curacion);

            Destroy(gameObject);
        }
    }
}
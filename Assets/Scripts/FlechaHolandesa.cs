using UnityEngine;
public class FlechaHolandesa : MonoBehaviour
{
    [Header("Configuración de la flecha")]
    [SerializeField] private float velocidad = 8f;
    [SerializeField] private float tiempoDeVida = 4f;

    private Vector2 direccion = Vector2.left;
    private int danio = 20;
    private LayerMask capaObjetivo;

    private bool yaImpacto;

    public void Inicializar(
        Vector2 nuevaDireccion,
        int nuevoDanio,
        LayerMask nuevaCapaObjetivo
    )
    {
        direccion = nuevaDireccion.normalized;
        danio = nuevoDanio;
        capaObjetivo = nuevaCapaObjetivo;
    }

    private void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    private void Update()
    {
        transform.Translate(
            direccion * velocidad * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (yaImpacto)
        {
            return;
        }

        bool perteneceACapaObjetivo =
            (capaObjetivo.value & (1 << otro.gameObject.layer)) != 0;

        if (!perteneceACapaObjetivo)
        {
            return;
        }

        yaImpacto = true;

        otro.gameObject.SendMessageUpwards(
            "RecibirDanio",
            danio,
            SendMessageOptions.DontRequireReceiver
        );

        Destroy(gameObject);
    }
}
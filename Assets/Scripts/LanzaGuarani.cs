using UnityEngine;

public class LanzaGuarani : MonoBehaviour
{
    [Header("Configuración de la lanza")]
    [SerializeField] private float velocidad = 9f;
    [SerializeField] private float tiempoDeVida = 4f;

    private Vector2 direccion = Vector2.right;
    private int danio = 30;
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

        OrientarLanza();
    }

    private void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    private void Update()
    {
        transform.Translate(
            direccion * velocidad * Time.deltaTime,
            Space.World
        );
    }

    private void OrientarLanza()
    {
        if (direccion.x < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
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
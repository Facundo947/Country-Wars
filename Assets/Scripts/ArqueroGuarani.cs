using UnityEngine;
using System.Collections;

public class ArqueroGuarani : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;

    [Tooltip("Activar para caminar hacia la derecha. Desactivar para caminar hacia la izquierda.")]
    [SerializeField] private bool avanzarHaciaLaDerecha = true;

    private float velocidadOriginal;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    private int vidaActual;

    [Header("Ataque con lanza")]
    [SerializeField] private GameObject prefabLanza;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private int danioLanza = 30;
    [SerializeField] private float rangoAtaque = 6f;
    [SerializeField] private float tiempoEntreAtaques = 1.8f;

    [Tooltip("Capa de las tropas y banderas enemigas")]
    [SerializeField] private LayerMask capaObjetivo;

    private float cronometroAtaque;
    private bool estaAtacando;
    private bool estaLento;

    private Animator animator;

    private Vector2 Direccion
    {
        get
        {
            return avanzarHaciaLaDerecha
                ? Vector2.right
                : Vector2.left;
        }
    }

    private void Start()
    {
        vidaActual = vidaMaxima;
        velocidadOriginal = velocidad;

        animator = GetComponent<Animator>();

        // Puede atacar inmediatamente al detectar un objetivo.
        cronometroAtaque = tiempoEntreAtaques;
    }

    private void Update()
    {
        DetectarObjetivo();

        if (!estaAtacando)
        {
            Caminar();
        }
    }

    private void DetectarObjetivo()
    {
        Debug.DrawRay(
            transform.position,
            Direccion * rangoAtaque,
            Color.green
        );

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Direccion,
            rangoAtaque,
            capaObjetivo
        );

        if (hit.collider != null)
        {
            estaAtacando = true;
            cronometroAtaque += Time.deltaTime;

            ActualizarAnimacion(false, true);

            if (cronometroAtaque >= tiempoEntreAtaques)
            {
                LanzarLanza();
                cronometroAtaque = 0f;
            }
        }
        else
        {
            estaAtacando = false;
            cronometroAtaque = tiempoEntreAtaques;

            ActualizarAnimacion(true, false);
        }
    }

    private void Caminar()
    {
        transform.Translate(
            Direccion * velocidad * Time.deltaTime
        );

        ActualizarAnimacion(true, false);
    }

    private void LanzarLanza()
    {
        if (prefabLanza == null)
        {
            Debug.LogWarning(
                "Falta asignar el prefab de la lanza en ArqueroGuarani."
            );

            return;
        }

        Vector3 posicionInicial;

        if (puntoDisparo != null)
        {
            posicionInicial = puntoDisparo.position;
        }
        else
        {
            posicionInicial = transform.position;
        }

        GameObject nuevaLanza = Instantiate(
            prefabLanza,
            posicionInicial,
            Quaternion.identity
        );

        LanzaGuarani lanza =
            nuevaLanza.GetComponent<LanzaGuarani>();

        if (lanza != null)
        {
            lanza.Inicializar(
                Direccion,
                danioLanza,
                capaObjetivo
            );
        }
        else
        {
            Debug.LogWarning(
                "El prefab de la lanza no tiene el script LanzaGuarani."
            );
        }

        if (animator != null)
        {
            animator.SetTrigger("LanzarLanza");
        }
    }

    private void ActualizarAnimacion(bool caminando, bool atacando)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool("Caminando", caminando);
        animator.SetBool("Atacando", atacando);
    }

    public void RecibirDanio(int cantidadDanio)
    {
        vidaActual -= cantidadDanio;

        if (vidaActual <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void AplicarLentitud(
        float velocidadLenta,
        float duracion
    )
    {
        if (!estaLento)
        {
            StartCoroutine(
                RutinaLentitud(velocidadLenta, duracion)
            );
        }
    }

    private IEnumerator RutinaLentitud(
        float velocidadLenta,
        float duracion
    )
    {
        estaLento = true;
        velocidad = velocidadLenta;

        yield return new WaitForSeconds(duracion);

        velocidad = velocidadOriginal;
        estaLento = false;
    }

    private void OnDrawGizmos()
    {
        Vector2 direccion = avanzarHaciaLaDerecha
            ? Vector2.right
            : Vector2.left;

        Gizmos.color = Color.green;

        Gizmos.DrawRay(
            transform.position,
            direccion * rangoAtaque
        );
    }
}
using UnityEngine;
using System.Collections;

public class SoldadoEspada : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;

    [Tooltip("Activar si el soldado debe caminar hacia la derecha")]
    [SerializeField] private bool avanzarHaciaLaDerecha = true;

    private float velocidadOriginal;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    private int vidaActual;

    [Header("Ataque con espada")]
    [SerializeField] private int danioEspada = 25;
    [SerializeField] private float rangoAtaque = 1.3f;
    [SerializeField] private float tiempoEntreAtaques = 1.5f;

    [Tooltip("Capa de las tropas y la bandera que debe atacar")]
    [SerializeField] private LayerMask capaObjetivo;

    private float cronometroAtaque;
    private bool estaAtacando;
    private bool estaLento;

    private Animator animator;

    private Vector2 DireccionMovimiento
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

        // Puede atacar inmediatamente al encontrar un objetivo.
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
            DireccionMovimiento * rangoAtaque,
            Color.red
        );

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            DireccionMovimiento,
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
                AtacarConEspada(hit.collider.gameObject);
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
            DireccionMovimiento * velocidad * Time.deltaTime
        );

        ActualizarAnimacion(true, false);
    }

    private void AtacarConEspada(GameObject objetivo)
    {
        if (animator != null)
        {
            animator.SetTrigger("AtaqueEspada");
        }

        objetivo.SendMessageUpwards(
            "RecibirDanio",
            danioEspada,
            SendMessageOptions.DontRequireReceiver
        );
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
            Morir();
        }
    }

    private void Morir()
    {
        Destroy(gameObject);
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

        Gizmos.color = Color.red;

        Gizmos.DrawRay(
            transform.position,
            direccion * rangoAtaque
        );
    }
}
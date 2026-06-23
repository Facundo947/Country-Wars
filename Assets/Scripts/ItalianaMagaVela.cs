using UnityEngine;
using System.Collections;

public class ItalianaMagaVela : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    private float velocidadOriginal;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    private int vidaActual;

    [Header("Detección")]
    [SerializeField] private float rangoAtaque = 5f;

    [Tooltip("Capa de las tropas argentinas y de la Bandera Argentina")]
    [SerializeField] private LayerMask capaAliada;

    [Header("Recarga de magia")]
    [SerializeField] private float tiempoRecargaMagica = 1.5f;
    [SerializeField] private float tiempoEntreAtaques = 2f;
    [SerializeField] private float duracionAnimacionAtaque = 0.5f;

    [Header("Ataque de llamas")]
    [SerializeField] private GameObject prefabLlama;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private int danioLlama = 30;

    private Animator animator;
    private Collider2D objetivoActual;

    private bool realizandoAtaque;
    private bool estaLenta;

    private float proximoAtaque;

    private void Start()
    {
        vidaActual = vidaMaxima;
        velocidadOriginal = velocidad;

        animator = GetComponent<Animator>();

        proximoAtaque = 0f;
    }

    private void Update()
    {
        // Mientras recarga o ataca, no puede caminar.
        if (realizandoAtaque)
        {
            return;
        }

        objetivoActual = BuscarObjetivo();

        if (objetivoActual == null)
        {
            Caminar();
        }
        else
        {
            Detenerse();

            if (Time.time >= proximoAtaque)
            {
                StartCoroutine(RecargarMagiaYAtacar());
            }
        }
    }

    private Collider2D BuscarObjetivo()
    {
        Debug.DrawRay(
            transform.position,
            Vector2.left * rangoAtaque,
            Color.magenta
        );

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.left,
            rangoAtaque,
            capaAliada
        );

        return hit.collider;
    }

    private void Caminar()
    {
        transform.Translate(
            Vector2.left * velocidad * Time.deltaTime
        );

        if (animator != null)
        {
            animator.SetBool("Caminando", true);
            animator.SetBool("Recargando", false);
            animator.SetBool("Atacando", false);
        }
    }

    private void Detenerse()
    {
        if (animator != null)
        {
            animator.SetBool("Caminando", false);
            animator.SetBool("Recargando", false);
            animator.SetBool("Atacando", false);
        }
    }

    private IEnumerator RecargarMagiaYAtacar()
    {
        realizandoAtaque = true;

        // PRIMERA ETAPA: se detiene y recarga magia con la vela.
        if (animator != null)
        {
            animator.SetBool("Caminando", false);
            animator.SetBool("Atacando", false);
            animator.SetBool("Recargando", true);

            animator.SetTrigger("RecargarMagia");
        }

        yield return new WaitForSeconds(tiempoRecargaMagica);

        // Verifica si todavía existe un enemigo adelante.
        objetivoActual = BuscarObjetivo();

        if (objetivoActual == null)
        {
            FinalizarAtaque();
            yield break;
        }

        // SEGUNDA ETAPA: termina la recarga y dispara la llama.
        if (animator != null)
        {
            animator.SetBool("Recargando", false);
            animator.SetBool("Atacando", true);

            animator.SetTrigger("LanzarLlama");
        }

        CrearLlama();

        yield return new WaitForSeconds(duracionAnimacionAtaque);

        FinalizarAtaque();
    }

    private void CrearLlama()
    {
        if (prefabLlama == null)
        {
            Debug.LogWarning(
                "Falta asignar el prefab de la llama en ItalianaMagaVela."
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

        GameObject nuevaLlama = Instantiate(
            prefabLlama,
            posicionInicial,
            Quaternion.identity
        );

        ProyectilLlamaMagica proyectil =
            nuevaLlama.GetComponent<ProyectilLlamaMagica>();

        if (proyectil != null)
        {
            proyectil.Inicializar(
                Vector2.left,
                danioLlama,
                capaAliada
            );
        }
        else
        {
            Debug.LogWarning(
                "El prefab de la llama no tiene el script ProyectilLlamaMagica."
            );
        }
    }

    private void FinalizarAtaque()
    {
        if (animator != null)
        {
            animator.SetBool("Recargando", false);
            animator.SetBool("Atacando", false);
        }

        proximoAtaque = Time.time + tiempoEntreAtaques;
        realizandoAtaque = false;
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
        if (!estaLenta)
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
        estaLenta = true;
        velocidad = velocidadLenta;

        yield return new WaitForSeconds(duracion);

        velocidad = velocidadOriginal;
        estaLenta = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawRay(
            transform.position,
            Vector2.left * rangoAtaque
        );
    }
}
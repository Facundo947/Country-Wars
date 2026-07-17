using UnityEngine;
using System.Collections;

public class CapoeiraBrasil : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2.5f;
    private float velocidadOriginal;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    private int vidaActual;

    [Header("Combate")]
    [SerializeField] private int danioManos = 15;
    [SerializeField] private int danioPiernas = 25;
    [SerializeField] private float largoLineaDeteccion = 1.3f;
    [SerializeField] private float tiempoEntreAtaques = 1.2f;

    [Tooltip("Capa de las tropas argentinas y de la Bandera Argentina")]
    [SerializeField] private LayerMask capaAliada;

    private float cronometroAtaque;
    private Animator animator;

    private bool estaAtacando;
    private bool estaLento;

    // Permite alternar entre manos y piernas.
    private bool siguienteAtaqueEsManos = true;

    private void Start()
    {
        vidaActual = vidaMaxima;
        velocidadOriginal = velocidad;

        animator = GetComponent<Animator>();

        // Puede atacar inmediatamente al encontrar un enemigo.
        cronometroAtaque = tiempoEntreAtaques;
    }

    private void Update()
    {
        // ===============================
        // FASE DE PLANEACIÓN
        // ===============================
        if (GameManager.Instance != null &&
            GameManager.Instance.EnPlaneacion())
        {
            if (animator != null)
            {
                animator.SetBool("Caminando", false);
                animator.SetBool("Atacando", false);
            }

            return;
        }

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
            Vector2.left * largoLineaDeteccion,
            Color.yellow
        );

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.left,
            largoLineaDeteccion,
            capaAliada
        );

        if (hit.collider != null)
        {
            estaAtacando = true;
            cronometroAtaque += Time.deltaTime;

            ActualizarAnimacionMovimiento(false);

            if (cronometroAtaque >= tiempoEntreAtaques)
            {
                Atacar(hit.collider.gameObject);
                cronometroAtaque = 0f;
            }
        }
        else
        {
            estaAtacando = false;

            // Al encontrar un nuevo enemigo podrá atacar inmediatamente.
            cronometroAtaque = tiempoEntreAtaques;

            ActualizarAnimacionMovimiento(true);
        }
    }

    private void Atacar(GameObject objetivo)
    {
        int danioAtaque;

        if (siguienteAtaqueEsManos)
        {
            danioAtaque = danioManos;

            if (animator != null)
            {
                animator.SetTrigger("AtaqueManos");
            }
        }
        else
        {
            danioAtaque = danioPiernas;

            if (animator != null)
            {
                animator.SetTrigger("AtaquePiernas");
            }
        }

        objetivo.SendMessage(
            "RecibirDanio",
            danioAtaque,
            SendMessageOptions.DontRequireReceiver
        );

        siguienteAtaqueEsManos = !siguienteAtaqueEsManos;
    }

    private void Caminar()
    {
        transform.Translate(
            Vector2.left * velocidad * Time.deltaTime
        );

        ActualizarAnimacionMovimiento(true);
    }

    private void ActualizarAnimacionMovimiento(bool caminando)
    {
        if (animator != null)
        {
            animator.SetBool("Caminando", caminando);
            animator.SetBool("Atacando", !caminando);
        }
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
        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(
            transform.position,
            Vector2.left * largoLineaDeteccion
        );
    }
}
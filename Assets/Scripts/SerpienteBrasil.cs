using UnityEngine;
using System.Collections;

public class SerpienteBrasil : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2.5f;
    private float velocidadOriginal;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 70;
    private int vidaActual;

    [Header("Ataque de mordida")]
    [SerializeField] private int danioMordida = 20;
    [SerializeField] private float rangoDeteccion = 1.2f;
    [SerializeField] private float tiempoEntreMordidas = 1.3f;

    [Tooltip("Capa de las tropas argentinas y de la Bandera Argentina")]
    [SerializeField] private LayerMask capaAliada;

    private float cronometroAtaque;
    private bool estaAtacando;
    private bool estaLenta;

    private Animator animator;

    private void Start()
    {
        vidaActual = vidaMaxima;
        velocidadOriginal = velocidad;

        animator = GetComponent<Animator>();

        // Permite atacar inmediatamente al encontrar un enemigo.
        cronometroAtaque = tiempoEntreMordidas;
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
                animator.SetBool("Moviendose", false);
                animator.SetBool("Atacando", false);
            }

            return;
        }

        DetectarObjetivo();

        if (!estaAtacando)
        {
            Moverse();
        }
    }

    private void DetectarObjetivo()
    {
        Debug.DrawRay(
            transform.position,
            Vector2.left * rangoDeteccion,
            Color.green
        );

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.left,
            rangoDeteccion,
            capaAliada
        );

        if (hit.collider != null)
        {
            estaAtacando = true;
            cronometroAtaque += Time.deltaTime;

            if (animator != null)
            {
                animator.SetBool("Moviendose", false);
                animator.SetBool("Atacando", true);
            }

            if (cronometroAtaque >= tiempoEntreMordidas)
            {
                Morder(hit.collider.gameObject);
                cronometroAtaque = 0f;
            }
        }
        else
        {
            estaAtacando = false;

            // Al encontrar un nuevo enemigo podrá atacar inmediatamente.
            cronometroAtaque = tiempoEntreMordidas;

            if (animator != null)
            {
                animator.SetBool("Atacando", false);
                animator.SetBool("Moviendose", true);
            }
        }
    }

    private void Morder(GameObject objetivo)
    {
        objetivo.SendMessageUpwards(
            "RecibirDanio",
            danioMordida,
            SendMessageOptions.DontRequireReceiver
        );

        if (animator != null)
        {
            animator.SetTrigger("Mordida");
        }
    }

    private void Moverse()
    {
        transform.Translate(
            Vector2.left * velocidad * Time.deltaTime
        );

        if (animator != null)
        {
            animator.SetBool("Moviendose", true);
            animator.SetBool("Atacando", false);
        }
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
        float duracion)
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
        float duracion)
    {
        estaLenta = true;
        velocidad = velocidadLenta;

        yield return new WaitForSeconds(duracion);

        velocidad = velocidadOriginal;
        estaLenta = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawRay(
            transform.position,
            Vector2.left * rangoDeteccion
        );
    }
}
using UnityEngine;
using System.Collections;

public class EspadachinArgentino : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 1.5f;
    private float velocidadOriginal;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 250;
    private int vidaActual;

    [Header("Combate")]
    [SerializeField] private int danio = 20;
    [SerializeField] private float rangoAtaque = 1.2f;
    [SerializeField] private float largoLineaDeteccion = 1.2f;
    [SerializeField] private float tiempoEntreAtaques = 1.5f;
    [SerializeField] private LayerMask capaEnemiga;

    private float cronometroAtaque;
    private Animator animator;
    private bool estaAtacando = false;
    private bool estaLento = false;

    private void Start()
    {
        vidaActual = vidaMaxima;
        velocidadOriginal = velocidad;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // ==========================================
        // FASE DE PLANEACIÓN
        // ==========================================
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

        // ==========================================
        // FASE DE EJECUCIÓN
        // ==========================================
        DetectarEnemigo();

        if (!estaAtacando)
        {
            Caminar();
        }
    }

    private void DetectarEnemigo()
    {
        Debug.DrawRay(
            transform.position,
            Vector2.right * largoLineaDeteccion,
            Color.blue
        );

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.right,
            largoLineaDeteccion,
            capaEnemiga
        );

        if (hit.collider != null)
        {
            estaAtacando = true;
            cronometroAtaque += Time.deltaTime;

            if (cronometroAtaque >= tiempoEntreAtaques)
            {
                EspadachinItaliano espadachin =
                    hit.collider.GetComponent<EspadachinItaliano>();

                if (espadachin != null)
                {
                    espadachin.RecibirDanio(danio);
                }

                BanderaItalia bandera =
                    hit.collider.GetComponent<BanderaItalia>();

                if (bandera != null)
                {
                    bandera.RecibirDanio(danio);
                }

                cronometroAtaque = 0f;
            }

            if (animator != null)
            {
                animator.SetBool("Caminando", false);
                animator.SetBool("Atacando", true);
            }
        }
        else
        {
            estaAtacando = false;
            cronometroAtaque = 0f;

            if (animator != null)
            {
                animator.SetBool("Atacando", false);
                animator.SetBool("Caminando", true);
            }
        }
    }

    private void Caminar()
    {
        transform.Translate(
            Vector2.right * velocidad * Time.deltaTime
        );

        if (animator != null)
        {
            animator.SetBool("Caminando", true);
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

    public void AplicarLentitud(float nuevaVelocidad, float duracion)
    {
        if (!estaLento)
        {
            StartCoroutine(
                RutinaLentitud(
                    nuevaVelocidad,
                    duracion
                )
            );
        }
    }

    private IEnumerator RutinaLentitud(
        float nuevaVelocidad,
        float duracion
    )
    {
        estaLento = true;

        velocidad = nuevaVelocidad;

        yield return new WaitForSeconds(duracion);

        velocidad = velocidadOriginal;

        estaLento = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawRay(
            transform.position,
            Vector2.right * largoLineaDeteccion
        );
    }
}
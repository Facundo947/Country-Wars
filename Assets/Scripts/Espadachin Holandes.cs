using UnityEngine;
using System.Collections;

public class EspadachinHolandes : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    private float velocidadOriginal;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 80;
    private int vidaActual;

    [Header("Combate")]
    [SerializeField] private int danio = 15;
    [SerializeField] private float rangoAtaque = 1.2f;
    [SerializeField] private float tiempoEntreAtaques = 1.2f;
    [SerializeField] private LayerMask capaEnemiga;

    private float cronometroAtaque;
    private Animator animator;
    private bool estaAtacando = false;

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
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.left,
            rangoAtaque,
            capaEnemiga
        );

        if (hit.collider != null)
        {
            estaAtacando = true;
            cronometroAtaque += Time.deltaTime;

            if (cronometroAtaque >= tiempoEntreAtaques)
            {
                Gaucho gaucho =
                    hit.collider.GetComponent<Gaucho>();

                if (gaucho != null)
                {
                    gaucho.RecibirDanio(danio);
                }

                Geisha geisha =
                    hit.collider.GetComponent<Geisha>();

                if (geisha != null)
                {
                    geisha.RecibirDanio(danio);
                }

                BanderaArgentina banderaArgentina =
                    hit.collider.GetComponent<BanderaArgentina>();

                if (banderaArgentina != null)
                {
                    banderaArgentina.RecibirDanio(danio);
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
            Vector2.left * velocidad * Time.deltaTime
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

    public void AplicarLentitud(
        float nuevaVelocidad,
        float duracion
    )
    {
        StopAllCoroutines();

        StartCoroutine(
            LentitudCoroutine(
                nuevaVelocidad,
                duracion
            )
        );
    }

    private IEnumerator LentitudCoroutine(
        float nuevaVelocidad,
        float duracion
    )
    {
        velocidad = nuevaVelocidad;

        yield return new WaitForSeconds(duracion);

        velocidad = velocidadOriginal;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.orange;

        Gizmos.DrawRay(
            transform.position,
            Vector2.left * rangoAtaque
        );
    }
}
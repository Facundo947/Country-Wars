using UnityEngine;
using System.Collections;

public class EspadachinItaliano : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    private float velocidadOriginal;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    private int vidaActual;

    [Header("Combate")]
    [SerializeField] private int danio = 20;
    [SerializeField] private float rangoAtaque = 1.2f;
    [SerializeField] private float largoLineaDeteccion = 1.2f;
    [SerializeField] private float tiempoEntreAtaques = 1.5f;
    [SerializeField] private LayerMask capaAliada;

    private float cronometroAtaque;
    private Animator animator;
    private bool estaAtacando = false;
    private bool estaLento = false;

    private void Start()
    {
        vidaActual = vidaMaxima;
        animator = GetComponent<Animator>();
        velocidadOriginal = velocidad;
    }

    private void Update()
    {
        // ==========================================================
        // FASE DE PLANEACIÓN
        // ==========================================================
        if (GameManager.Instance != null && GameManager.Instance.EnPlaneacion())
        {
            if (animator != null)
            {
                animator.SetBool("Caminando", false);
                animator.SetBool("Atacando", false);
            }

            return;
        }
        // ==========================================================

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
            Vector2.left * largoLineaDeteccion,
            Color.red
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

            if (cronometroAtaque >= tiempoEntreAtaques)
            {
                // MESSI
                Messi messi =
                    hit.collider.GetComponent<Messi>();

                if (messi != null)
                {
                    messi.RecibirDanio(danio);
                }

                // GAUCHO
                Gaucho gaucho =
                    hit.collider.GetComponent<Gaucho>();

                if (gaucho != null)
                {
                    gaucho.RecibirDanio(danio);
                }

                // GUARANÍ
                ArqueroGuarani guarani =
                    hit.collider.GetComponent<ArqueroGuarani>();

                if (guarani != null)
                {
                    guarani.RecibirDanio(danio);
                }

                // ESPADACHÍN ARGENTINO
                EspadachinArgentino espadachinArgentino =
                    hit.collider.GetComponent<EspadachinArgentino>();

                if (espadachinArgentino != null)
                {
                    espadachinArgentino.RecibirDanio(danio);
                }

                // BANDERA ARGENTINA
                BanderaArgentina bandera =
                    hit.collider.GetComponent<BanderaArgentina>();

                if (bandera != null)
                {
                    bandera.RecibirDanio(danio);
                }
                else
                {
                    hit.collider.gameObject.SendMessage(
                        "RecibirDanio",
                        danio,
                        SendMessageOptions.DontRequireReceiver
                    );
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
        float factorVelocidadLenta,
        float duracion
    )
    {
        if (!estaLento)
        {
            StartCoroutine(
                RutinaLentitud(
                    factorVelocidadLenta,
                    duracion
                )
            );
        }
    }

    private IEnumerator RutinaLentitud(
        float factorVelocidadLenta,
        float duracion
    )
    {
        estaLento = true;
        velocidad = factorVelocidadLenta;

        yield return new WaitForSeconds(duracion);

        velocidad = velocidadOriginal;
        estaLento = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(
            transform.position,
            Vector2.left * largoLineaDeteccion
        );
    }
}
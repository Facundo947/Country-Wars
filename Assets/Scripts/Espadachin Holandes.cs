using UnityEngine;
using System.Collections;

public class EspadachinHolandes : MonoBehaviour
{
    [Header("Mejora por nivel")]
    [SerializeField] private float aumentoVelocidadPorNivel = 0.1f;
    [SerializeField] private int aumentoVidaPorNivel = 1;
    [SerializeField] private int aumentoDanioPorNivel = 1;
    [SerializeField] private float reduccionTiempoAtaquePorNivel = 0.1f;

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
        int nivelActual = 1;

        if (GameManager.Instance != null)
        {
            nivelActual = GameManager.Instance.ObtenerNivel();
        }

        int nivelesExtra = nivelActual - 1;

        if (nivelesExtra < 0)
        {
            nivelesExtra = 0;
        }

        // Mejoras por nivel
        velocidad += aumentoVelocidadPorNivel * nivelesExtra;

        vidaMaxima += aumentoVidaPorNivel * nivelesExtra;

        danio += aumentoDanioPorNivel * nivelesExtra;

        tiempoEntreAtaques -= reduccionTiempoAtaquePorNivel * nivelesExtra;

        if (tiempoEntreAtaques < 0.1f)
        {
            tiempoEntreAtaques = 0.1f;
        }

        vidaActual = vidaMaxima;

        velocidadOriginal = velocidad;

        animator = GetComponent<Animator>();

        // DEBUG
        Debug.Log("==============================");
        Debug.Log("ESPADACHÍN HOLANDÉS - ESTADISTICAS");
        Debug.Log("Nivel detectado: " + nivelActual);
        Debug.Log("Niveles extra: " + nivelesExtra);
        Debug.Log("Vida: " + vidaMaxima);
        Debug.Log("Daño: " + danio);
        Debug.Log("Velocidad: " + velocidad);
        Debug.Log("Tiempo entre ataques: " + tiempoEntreAtaques);
        Debug.Log("==============================");
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
                // DAÑO AL GAUCHO
                Gaucho gaucho =
                    hit.collider.GetComponent<Gaucho>();

                if (gaucho != null)
                {
                    gaucho.RecibirDanio(danio);
                }

                // DAÑO A LA GEISHA
                Geisha geisha =
                    hit.collider.GetComponent<Geisha>();

                if (geisha != null)
                {
                    geisha.RecibirDanio(danio);
                }

                // DAÑO A LA BANDERA ARGENTINA
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
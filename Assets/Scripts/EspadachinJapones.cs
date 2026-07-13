using UnityEngine;
using System.Collections;

public class TropaJaponesa : MonoBehaviour
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
    [SerializeField] private LayerMask capaAliada;

    private float cronometroAtaque;
    private Animator animator;
    private bool estaAtacando = false;


    private void Start()
    {
        vidaActual = vidaMaxima;
        animator = GetComponent<Animator>();
        velocidadOriginal = velocidad;
    }


    private void Update()
    {
        // ====================================================================
        // FASE DE PLANEACIÓN (Copiado de la lógica del Gaucho)
        // ====================================================================
        if (GameManager.Instance != null && GameManager.Instance.EnPlaneacion())
        {
            if (animator != null)
            {
                // Como tu japonés usa Bools, los ponemos en false para que se quede quieto en planeación
                animator.SetBool("Caminando", false);
                animator.SetBool("Atacando", false);
            }

            return; // Frena el script acá, impidiendo que camine o ataque antes de tiempo
        }
        // ====================================================================

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
            capaAliada
        );


        if (hit.collider != null)
        {
            estaAtacando = true;
            cronometroAtaque += Time.deltaTime;

            if (cronometroAtaque >= tiempoEntreAtaques)
            {
                // DAÑO AL GAUCHO
                Gaucho gaucho = hit.collider.GetComponent<Gaucho>();
                if (gaucho != null)
                {
                    gaucho.RecibirDanio(danio);
                }

                // DAÑO A LA BANDERA ARGENTINA
                BanderaArgentina bandera = hit.collider.GetComponent<BanderaArgentina>();
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
            Vector2.left * velocidad * Time.deltaTime
        );

        if (animator != null)
        {
            animator.SetBool("Caminando", true);
        }
    }

    // El resto de tus funciones de daño y curación se quedan exactamente igual abajo...
    public void RecibirDanio(int cantidadDanio)
    {
        vidaActual -= cantidadDanio;
        if (vidaActual <= 0) Destroy(gameObject);
    }

    public void Curar(int cantidad)
    {
        vidaActual += cantidad;
        if (vidaActual > vidaMaxima) vidaActual = vidaMaxima;
    }

    public bool EstaHerido() => vidaActual < vidaMaxima;

    public void AplicarLentitud(float nuevaVelocidad, float duracion)
    {
        StopAllCoroutines();
        StartCoroutine(LentitudCoroutine(nuevaVelocidad, duracion));
    }

    private IEnumerator LentitudCoroutine(float nuevaVelocidad, float duracion)
    {
        velocidad = nuevaVelocidad;
        yield return new WaitForSeconds(duracion);
        velocidad = velocidadOriginal;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, Vector2.left * rangoAtaque);
    }
}
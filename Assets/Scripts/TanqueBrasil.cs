using UnityEngine;
using System.Collections;
public class TanqueBrasil : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 1.2f;
    private float velocidadOriginal;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 300;
    private int vidaActual;

    [Header("Ataque con puños")]
    [SerializeField] private int danioPunio = 40;
    [SerializeField] private float rangoDeteccion = 1.5f;
    [SerializeField] private float tiempoEntreGolpes = 1.8f;

    [Tooltip("Capa de las tropas argentinas y de la Bandera Argentina")]
    [SerializeField] private LayerMask capaAliada;

    private float cronometroAtaque;
    private bool estaAtacando;
    private bool estaLento;

    private Animator animator;

    private void Start()
    {
        vidaActual = vidaMaxima;
        velocidadOriginal = velocidad;

        animator = GetComponent<Animator>();

        // Permite atacar inmediatamente al encontrar un objetivo.
        cronometroAtaque = tiempoEntreGolpes;
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
            Vector2.left * rangoDeteccion,
            Color.blue
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
                animator.SetBool("Caminando", false);
                animator.SetBool("Atacando", true);
            }

            if (cronometroAtaque >= tiempoEntreGolpes)
            {
                GolpearConPunio(hit.collider.gameObject);
                cronometroAtaque = 0f;
            }
        }
        else
        {
            estaAtacando = false;
            cronometroAtaque = tiempoEntreGolpes;

            if (animator != null)
            {
                animator.SetBool("Atacando", false);
                animator.SetBool("Caminando", true);
            }
        }
    }

    private void GolpearConPunio(GameObject objetivo)
    {
        objetivo.SendMessageUpwards(
            "RecibirDanio",
            danioPunio,
            SendMessageOptions.DontRequireReceiver
        );

        if (animator != null)
        {
            animator.SetTrigger("GolpePunio");
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
            Morir();
        }
    }

    private void Morir()
    {
        Destroy(gameObject);
    }

    public void AplicarLentitud(float velocidadLenta, float duracion)
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
        Gizmos.color = Color.blue;

        Gizmos.DrawRay(
            transform.position,
            Vector2.left * rangoDeteccion
        );
    }
}

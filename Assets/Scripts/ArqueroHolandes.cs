using UnityEngine;
using System.Collections;

public class ArqueroHolandes : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    private float velocidadOriginal;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    private int vidaActual;

    [Header("Ataque con arco")]
    [SerializeField] private GameObject prefabFlecha;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private int danioFlecha = 20;
    [SerializeField] private float rangoAtaque = 6f;
    [SerializeField] private float tiempoEntreAtaques = 1.5f;

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

        // Permite disparar inmediatamente al encontrar un objetivo.
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
            Vector2.left * rangoAtaque,
            Color.cyan
        );

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

            if (animator != null)
            {
                animator.SetBool("Caminando", false);
                animator.SetBool("Atacando", true);
            }

            if (cronometroAtaque >= tiempoEntreAtaques)
            {
                DispararFlecha();
                cronometroAtaque = 0f;
            }
        }
        else
        {
            estaAtacando = false;

            // Al encontrar un nuevo enemigo podrá disparar inmediatamente.
            cronometroAtaque = tiempoEntreAtaques;

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

    private void DispararFlecha()
    {
        if (prefabFlecha == null)
        {
            Debug.LogWarning(
                "Falta asignar el prefab de la flecha en ArqueroHolandes."
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

        GameObject nuevaFlecha = Instantiate(
            prefabFlecha,
            posicionInicial,
            Quaternion.identity
        );

        FlechaHolandesa flecha =
            nuevaFlecha.GetComponent<FlechaHolandesa>();

        if (flecha != null)
        {
            flecha.Inicializar(
                Vector2.left,
                danioFlecha,
                capaAliada
            );
        }
        else
        {
            Debug.LogWarning(
                "El prefab no tiene el script FlechaHolandesa."
            );
        }

        if (animator != null)
        {
            animator.SetTrigger("DispararFlecha");
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
        Gizmos.color = Color.cyan;

        Gizmos.DrawRay(
            transform.position,
            Vector2.left * rangoAtaque
        );
    }
}
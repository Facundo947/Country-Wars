using UnityEngine;
using System.Collections;

public class SeñoraEmpanadas : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    private float velocidadOriginal;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    private int vidaActual;

    [Header("Ataque con empanadas")]
    [SerializeField] private GameObject prefabEmpanada;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private int danioEmpanada = 25;
    [SerializeField] private float rangoAtaque = 5f;
    [SerializeField] private float tiempoEntreAtaques = 1.5f;

    [Tooltip("Capa de las tropas y banderas enemigas")]
    [SerializeField] private LayerMask capaEnemiga;

    private float cronometroAtaque;
    private bool estaAtacando;
    private bool estaLenta;

    private Animator animator;

    private void Start()
    {
        vidaActual = vidaMaxima;
        velocidadOriginal = velocidad;

        animator = GetComponent<Animator>();

        // Puede lanzar una empanada inmediatamente al encontrar un enemigo.
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
            Vector2.right * rangoAtaque,
            Color.yellow
        );

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.right,
            rangoAtaque,
            capaEnemiga
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
                LanzarEmpanada();
                cronometroAtaque = 0f;
            }
        }
        else
        {
            estaAtacando = false;
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
            Vector2.right * velocidad * Time.deltaTime
        );

        if (animator != null)
        {
            animator.SetBool("Caminando", true);
            animator.SetBool("Atacando", false);
        }
    }

    private void LanzarEmpanada()
    {
        if (prefabEmpanada == null)
        {
            Debug.LogWarning(
                "Falta asignar el prefab de la empanada en SenoraEmpanadas."
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

        GameObject nuevaEmpanada = Instantiate(
            prefabEmpanada,
            posicionInicial,
            Quaternion.identity
        );

        EmpanadaProyectil empanada =
            nuevaEmpanada.GetComponent<EmpanadaProyectil>();

        if (empanada != null)
        {
            empanada.Inicializar(
                Vector2.right,
                danioEmpanada,
                capaEnemiga
            );
        }
        else
        {
            Debug.LogWarning(
                "El prefab no tiene el script EmpanadaProyectil."
            );
        }

        if (animator != null)
        {
            animator.SetTrigger("LanzarEmpanada");
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
        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(
            transform.position,
            Vector2.right * rangoAtaque
        );
    }
}
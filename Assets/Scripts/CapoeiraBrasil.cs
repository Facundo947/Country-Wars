using UnityEngine;
using System.Collections;

public class CapoeiraBrasil : MonoBehaviour
{
    [Header("Mejora por nivel")]
    [SerializeField] private float aumentoVelocidadPorNivel = 0.1f;
    [SerializeField] private int aumentoVidaPorNivel = 1;
    [SerializeField] private int aumentoDanioPorNivel = 1;
    [SerializeField] private float reduccionTiempoAtaquePorNivel = 0.1f;

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
        velocidad +=
            aumentoVelocidadPorNivel * nivelesExtra;

        vidaMaxima +=
            aumentoVidaPorNivel * nivelesExtra;

        danioManos +=
            aumentoDanioPorNivel * nivelesExtra;

        danioPiernas +=
            aumentoDanioPorNivel * nivelesExtra;

        tiempoEntreAtaques -=
            reduccionTiempoAtaquePorNivel * nivelesExtra;

        if (tiempoEntreAtaques < 0.1f)
        {
            tiempoEntreAtaques = 0.1f;
        }

        vidaActual = vidaMaxima;

        velocidadOriginal = velocidad;

        animator = GetComponent<Animator>();

        // Puede atacar inmediatamente al encontrar un enemigo.
        cronometroAtaque = tiempoEntreAtaques;

        // DEBUG
        Debug.Log("==============================");
        Debug.Log("CAPOEIRA BRASIL - ESTADISTICAS");
        Debug.Log("Nivel detectado: " + nivelActual);
        Debug.Log("Niveles extra: " + nivelesExtra);
        Debug.Log("Vida: " + vidaMaxima);
        Debug.Log("Daño manos: " + danioManos);
        Debug.Log("Daño piernas: " + danioPiernas);
        Debug.Log("Velocidad: " + velocidad);
        Debug.Log("Tiempo entre ataques: " + tiempoEntreAtaques);
        Debug.Log("==============================");
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


using UnityEngine;
using System.Collections;

public class AlemanaCerveza : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    private float velocidadOriginal;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    private int vidaActual;

    [Header("Ataque de cerveza")]
    [SerializeField] private GameObject prefabCerveza;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private int danioCerveza = 20;
    [SerializeField] private float rangoAtaque = 5f;
    [SerializeField] private float tiempoEntreAtaques = 1.5f;

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
            Color.yellow
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
                TirarCerveza();
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
            Vector2.left * velocidad * Time.deltaTime
        );

        if (animator != null)
        {
            animator.SetBool("Caminando", true);
            animator.SetBool("Atacando", false);
        }
    }

    private void TirarCerveza()
    {
        if (prefabCerveza == null)
        {
            Debug.LogWarning(
                "Falta asignar el prefab de la cerveza en AlemanaCerveza."
            );

            return;
        }

        Vector3 posicionDisparo;

        if (puntoDisparo != null)
        {
            posicionDisparo = puntoDisparo.position;
        }
        else
        {
            posicionDisparo = transform.position;
        }

        GameObject nuevaCerveza = Instantiate(
            prefabCerveza,
            posicionDisparo,
            Quaternion.identity
        );

        ProyectilCerveza proyectil =
            nuevaCerveza.GetComponent<ProyectilCerveza>();

        if (proyectil != null)
        {
            proyectil.Inicializar(
                Vector2.left,
                danioCerveza,
                capaAliada
            );
        }

        if (animator != null)
        {
            animator.SetTrigger("TirarCerveza");
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
            Vector2.left * rangoAtaque
        );
    }
}

using UnityEngine;
using System.Collections;

public class PizzeroItaliano : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    private float velocidadOriginal;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    private int vidaActual;

    [Header("Ataque con pizza")]
    [SerializeField] private GameObject prefabPizza;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private int danioPizza = 25;
    [SerializeField] private float rangoAtaque = 5f;
    [SerializeField] private float tiempoEntreAtaques = 1.5f;

    [Tooltip("Capa de las tropas argentinas y la Bandera Argentina")]
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

        // Permite atacar inmediatamente al encontrar un enemigo.
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
                LanzarPizza();
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

    private void LanzarPizza()
    {
        if (prefabPizza == null)
        {
            Debug.LogWarning(
                "Falta asignar el prefab de la pizza en PizzeroItaliano."
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

        GameObject nuevaPizza = Instantiate(
            prefabPizza,
            posicionInicial,
            Quaternion.identity
        );

        PizzaProyectil pizza =
            nuevaPizza.GetComponent<PizzaProyectil>();

        if (pizza != null)
        {
            pizza.Inicializar(
                Vector2.left,
                danioPizza,
                capaAliada
            );
        }
        else
        {
            Debug.LogWarning(
                "El prefab de la pizza no tiene el script PizzaProyectil."
            );
        }

        if (animator != null)
        {
            animator.SetTrigger("LanzarPizza");
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
        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(
            transform.position,
            Vector2.left * rangoAtaque
        );
    }
}
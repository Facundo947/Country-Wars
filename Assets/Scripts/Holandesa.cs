using UnityEngine;

public class Holandesa : MonoBehaviour
{
    [Header("Movimiento y Rango")]
    [SerializeField] private float velocidadCamino = 2f;
    [SerializeField] private float largoLineaDeteccion = 6f;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 80;
    private int vidaActual;

    [Header("Proyectil (Flor)")]
    [SerializeField] private GameObject prefabFlor;

    [Header("Detección")]
    [SerializeField] private LayerMask capaObjetivos;

    [Header("Ataque")]
    [SerializeField] private float tiempoEntreDisparos = 1.2f;
    private float cooldown;

    [Header("Componentes")]
    private Animator miAnimator;
    private Rigidbody2D miRigidbody;
    private Transform objetivoActual;

    private void Start()
    {
        vidaActual = vidaMaxima;
        miAnimator = GetComponent<Animator>();
        miRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // ==========================================
        // FASE DE PLANEACIÓN
        // ==========================================
        if (GameManager.Instance != null &&
            GameManager.Instance.EnPlaneacion())
        {
            miRigidbody.linearVelocity = Vector2.zero;

            objetivoActual = null;

            if (miAnimator != null)
            {
                miAnimator.SetFloat("caminando", 0f);
                miAnimator.SetFloat("atacando", 0f);
            }

            return;
        }

        // ==========================================
        // FASE DE EJECUCIÓN
        // ==========================================
        DetectarObjetivo();
        ControlMovimiento();
        ControlDisparo();
    }

    // =========================================================
    // DETECCIÓN CON RAYCAST
    // =========================================================
    private void DetectarObjetivo()
    {
        Debug.DrawRay(transform.position, Vector2.left * largoLineaDeteccion, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.left,
            largoLineaDeteccion,
            capaObjetivos
        );

        if (hit.collider != null)
        {
            if (hit.collider.GetComponent<Messi>() != null ||
                hit.collider.GetComponent<Gaucho>() != null ||
                hit.collider.GetComponent<BanderaArgentina>() != null)
            {
                objetivoActual = hit.collider.transform;
                return;
            }
        }

        objetivoActual = null;
    }

    // =========================================================
    // MOVIMIENTO + ANIMACIÓN
    // =========================================================
    private void ControlMovimiento()
    {
        if (objetivoActual != null)
        {
            miRigidbody.linearVelocity = Vector2.zero;

            miAnimator.SetFloat("caminando", 0f);
            miAnimator.SetFloat("atacando", 1f);
        }
        else
        {
            objetivoActual = null;

            miRigidbody.linearVelocity =
                new Vector2(-velocidadCamino, miRigidbody.linearVelocity.y);

            miAnimator.SetFloat("caminando", 1f);
            miAnimator.SetFloat("atacando", 0f);
        }
    }

    // =========================================================
    // DISPARO
    // =========================================================
    private void ControlDisparo()
    {
        if (objetivoActual == null)
            return;

        cooldown -= Time.deltaTime;

        if (cooldown <= 0f)
        {
            DispararFlor();
            cooldown = tiempoEntreDisparos;
        }
    }

    // =========================================================
    // DISPARO REAL
    // =========================================================
    private void DispararFlor()
    {
        if (prefabFlor == null || objetivoActual == null)
            return;

        GameObject flor = Instantiate(
            prefabFlor,
            transform.position,
            Quaternion.identity
        );

        FlorHolandesa script =
            flor.GetComponent<FlorHolandesa>();

        if (script != null)
        {
            script.ConfigurarObjetivo(objetivoActual);
        }
    }

    // =========================================================
    // VIDA
    // =========================================================
    public void RecibirDanio(int daño)
    {
        vidaActual -= daño;

        if (vidaActual <= 0)
        {
            Destroy(gameObject);
        }
    }

    // =========================================================
    // DEBUG
    // =========================================================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(
            transform.position,
            Vector2.left * largoLineaDeteccion
        );
    }
}
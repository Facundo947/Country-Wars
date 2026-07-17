using UnityEngine;

public class Boleadora : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 10f;

    [Header("Daño")]
    [SerializeField] private int danio = 25;

    [Header("Lentitud")]
    [SerializeField] private float velocidadLenta = 0.5f;
    [SerializeField] private float duracionLentitud = 2f;

    [Header("Tiempo de vida")]
    [SerializeField] private float tiempoDeVida = 5f;

    private void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    private void Update()
    {
        transform.Translate(
            Vector2.right * velocidad * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ==========================================
        // JAPÓN
        // ==========================================

        TropaJaponesa enemigo =
            other.GetComponent<TropaJaponesa>();

        if (enemigo != null)
        {
            enemigo.RecibirDanio(danio);
            enemigo.AplicarLentitud(
                velocidadLenta,
                duracionLentitud
            );

            Destroy(gameObject);
            return;
        }

        Geisha geisha =
            other.GetComponent<Geisha>();

        if (geisha != null)
        {
            geisha.RecibirDanio(danio);

            Destroy(gameObject);
            return;
        }

        BanderaJaponesa banderaJapon =
            other.GetComponent<BanderaJaponesa>();

        if (banderaJapon != null)
        {
            banderaJapon.RecibirDanio(danio);

            Destroy(gameObject);
            return;
        }

        // ==========================================
        // HOLANDA
        // ==========================================

        EspadachinHolandes holandes =
            other.GetComponent<EspadachinHolandes>();

        if (holandes != null)
        {
            holandes.RecibirDanio(danio);
            holandes.AplicarLentitud(
                velocidadLenta,
                duracionLentitud
            );

            Destroy(gameObject);
            return;
        }

        ArqueroHolandes arquero =
            other.GetComponent<ArqueroHolandes>();

        if (arquero != null)
        {
            arquero.RecibirDanio(danio);
            arquero.AplicarLentitud(
                velocidadLenta,
                duracionLentitud
            );

            Destroy(gameObject);
            return;
        }

        BanderaHolandesa holandesa =
            other.GetComponent<BanderaHolandesa>();

        if (holandesa != null)
        {
            holandesa.RecibirDanio(danio);

            Destroy(gameObject);
            return;
        }

        // ==========================================
        // ITALIA
        // ==========================================

        EspadachinItaliano enemigoItaliano =
            other.GetComponent<EspadachinItaliano>();

        if (enemigoItaliano != null)
        {
            enemigoItaliano.RecibirDanio(danio);
            enemigoItaliano.AplicarLentitud(
                velocidadLenta,
                duracionLentitud
            );

            Destroy(gameObject);
            return;
        }

        PizzeroItaliano pizzero =
            other.GetComponent<PizzeroItaliano>();

        if (pizzero != null)
        {
            pizzero.RecibirDanio(danio);
            pizzero.AplicarLentitud(
                velocidadLenta,
                duracionLentitud
            );

            Destroy(gameObject);
            return;
        }

        // TANQUE ITALIANO
        TanqueItaliano tanque =
            other.GetComponent<TanqueItaliano>();

        if (tanque != null)
        {
            tanque.RecibirDanio(danio);
            tanque.AplicarLentitud(
                velocidadLenta,
                duracionLentitud
            );

            Destroy(gameObject);
            return;
        }

        BanderaItalia banderaItaliana =
            other.GetComponent<BanderaItalia>();

        if (banderaItaliana != null)
        {
            banderaItaliana.RecibirDanio(danio);

            Destroy(gameObject);
            return;
        }
    }
}
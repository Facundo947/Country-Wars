using System;
using TMPro;
using UnityEngine;

public class SistemaMonedas : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int monedasIniciales = 100;
    [SerializeField] private int monedasMaximas = 200;

    [Header("Interfaz")]
    [SerializeField] private TMP_Text textoMonedas;

    public int MonedasActuales { get; private set; }

    public event Action<int> MonedasCambiadas;

    private void Start()
    {
        MonedasActuales = Mathf.Clamp(
            monedasIniciales,
            0,
            monedasMaximas
        );

        ActualizarInterfaz();
    }

    public bool PuedePagar(int costo)
    {
        costo = Mathf.Max(0, costo);

        return MonedasActuales >= costo;
    }

    public bool IntentarGastar(int costo)
    {
        costo = Mathf.Max(0, costo);

        if (!PuedePagar(costo))
        {
            Debug.Log("No hay monedas suficientes.");
            return false;
        }

        MonedasActuales -= costo;

        ActualizarInterfaz();

        return true;
    }

    public void AgregarMonedas(int cantidad)
    {
        if (cantidad <= 0)
        {
            return;
        }

        MonedasActuales = Mathf.Clamp(
            MonedasActuales + cantidad,
            0,
            monedasMaximas
        );

        ActualizarInterfaz();
    }

    private void ActualizarInterfaz()
    {
        if (textoMonedas != null)
        {
            textoMonedas.text =
                MonedasActuales.ToString();
        }

        MonedasCambiadas?.Invoke(
            MonedasActuales
        );
    }
}
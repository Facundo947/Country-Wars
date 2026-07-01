using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PausarJuego : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;
    [SerializeField] private GameObject menuOpciones;

    public void Pausa()
    {
        Time.timeScale = 0f;
        botonPausa.SetActive(false);
        menuPausa.SetActive(true);
    }

    public void Reanudar()
    {
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
    }

    public void Reintentan()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrMenuPrincipal()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Menu Principal");
    }

    public void Cerrar()
    {
        Debug.Log("Cerrando Juego");
        Application.Quit();
    }
    public void AbrirOpciones()
    {
        menuPausa.SetActive(false);
        menuOpciones.SetActive(true);
    }

    public void VolverAPausa()
    {
        menuOpciones.SetActive(false);
        menuPausa.SetActive(true);
    }
}
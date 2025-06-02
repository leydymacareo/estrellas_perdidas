using UnityEngine;
using System.Collections;

public class PantallaInicioUI : MonoBehaviour
{
    public GameObject panelInicio;
    private bool juegoIniciado = false;
    private static bool yaMostroInicio = false;

    void Start()
    {
        if (!yaMostroInicio)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (panelInicio != null)
                panelInicio.SetActive(true);
        }
        else
        {
            if (panelInicio != null)
                panelInicio.SetActive(false);
        }
    }

    void Update()
    {
        if (panelInicio != null && panelInicio.activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Jugar()
    {
        if (juegoIniciado) return;
        juegoIniciado = true;
        StartCoroutine(IniciarJuegoConRetraso());
    }

    IEnumerator IniciarJuegoConRetraso()
    {
        yield return null;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        if (panelInicio != null)
            panelInicio.SetActive(false);

        yaMostroInicio = true;
    }
}

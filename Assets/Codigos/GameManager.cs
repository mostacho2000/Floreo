using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    #region Estados del Juego
    public enum GameState
    {
        MainMenu,
        Credits,
        FlowerSelection,
        Tutorial,
        LevelSetup,
        Playing,
        Paused,
        SoundSettings
    }
    public GameState currentState;
    #endregion

    #region Variables Públicas (Para arrastrar en el Inspector)

    [Header("Paneles Principales")]
    public GameObject panelMenu;
    public GameObject panelCreditos;
    public GameObject panelSelectorDeFlores;
    public GameObject panelPausa;
    public GameObject panelSonido;

    [Header("Elementos del Tutorial")]
    public GameObject panelContenedorTutorial;
    public GameObject[] panelesDialogo;
    public Image imagenNpc;

    [Header("Elementos del Nivel")]
    public GameObject panelContenedorSetup;
    public GameObject panelContenedorJuego;
    public GameObject botonVolverMenu; // <--- REEMPLAZA a panelGameOver
    public Image imagenFondo;
    public Image imagenMaceta;
    public Image imagenPlanta;

    [Header("Sprites para Setup")]
    public Sprite[] spritesFondo;
    public Sprite[] spritesMaceta;

    [Header("Sprites de la Planta")]
    public Sprite spriteRetono;
    public Sprite spriteCrecida;
    public Sprite spriteMarchita;

    [Header("Lógica del Nivel (Geranio)")]
    public int riegosNecesarios = 1;
    public int abonosNecesarios = 1;

    #endregion

    #region Variables Privadas
    private int dialogoActual = 0;
    private int fondoActual = 0;
    private int macetaActual = 0;
    private GameState previousState;

    private int riegosActuales = 0;
    private int abonosActuales = 0;
    #endregion

    #region Ciclo de Vida de Unity
    void Start()
    {
        CambiarEstado(GameState.MainMenu);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) &&
            (currentState == GameState.LevelSetup || currentState == GameState.Playing))
        {
            previousState = currentState;
            CambiarEstado(GameState.Paused);
        }
    }
    #endregion

    #region Máquina de Estados Principal
    
    void CambiarEstado(GameState nuevoEstado)
    {
        currentState = nuevoEstado;

        if (nuevoEstado != GameState.Paused && nuevoEstado != GameState.SoundSettings)
        {
            ApagarTodo();
        }

        switch (currentState)
        {
            case GameState.MainMenu:
                panelMenu.SetActive(true);
                break;
            case GameState.Credits:
                panelCreditos.SetActive(true);
                break;
            case GameState.FlowerSelection:
                panelSelectorDeFlores.SetActive(true);
                break;
            case GameState.Tutorial:
                if (imagenFondo != null) imagenFondo.gameObject.SetActive(true);
                panelContenedorTutorial.SetActive(true);
                if (imagenNpc != null) imagenNpc.gameObject.SetActive(true);
                foreach (var panel in panelesDialogo) { if (panel != null) panel.SetActive(false); }
                if (panelesDialogo.Length > 0) panelesDialogo[dialogoActual].SetActive(true);
                break;
            case GameState.LevelSetup:
                if (imagenFondo != null) imagenFondo.gameObject.SetActive(true);
                if (imagenMaceta != null) imagenMaceta.gameObject.SetActive(true);
                panelContenedorSetup.SetActive(true);
                break;
            
            // --- SECCIÓN MODIFICADA ---
            case GameState.Playing:
                if (imagenFondo != null) imagenFondo.gameObject.SetActive(true);
                if (imagenMaceta != null) imagenMaceta.gameObject.SetActive(true);
                if (imagenPlanta != null) imagenPlanta.gameObject.SetActive(true);
                panelContenedorJuego.SetActive(true);
                if (botonVolverMenu != null) botonVolverMenu.SetActive(false); // Nos aseguramos de ocultar el botón de Game Over
                break;

            case GameState.Paused:
                if (panelContenedorSetup != null) panelContenedorSetup.SetActive(false);
                if (panelContenedorJuego != null) panelContenedorJuego.SetActive(false);
                // El botón de volver al menú también debe ocultarse si está activo
                if (botonVolverMenu != null) botonVolverMenu.SetActive(false);
                
                panelSonido.SetActive(false);
                panelPausa.SetActive(true);
                break;
            case GameState.SoundSettings:
                panelPausa.SetActive(true);
                panelSonido.SetActive(true);
                break;
        }
    }

    void ApagarTodo()
    {
        panelMenu.SetActive(false);
        panelCreditos.SetActive(false);
        panelSelectorDeFlores.SetActive(false);
        panelPausa.SetActive(false);
        panelSonido.SetActive(false);
        panelContenedorTutorial.SetActive(false);
        panelContenedorSetup.SetActive(false);
        panelContenedorJuego.SetActive(false);
        if (botonVolverMenu != null) botonVolverMenu.SetActive(false); // <--- CAMBIO
        
        if (imagenFondo != null) imagenFondo.gameObject.SetActive(false);
        if (imagenMaceta != null) imagenMaceta.gameObject.SetActive(false);
        if (imagenPlanta != null) imagenPlanta.gameObject.SetActive(false);
        if (imagenNpc != null) imagenNpc.gameObject.SetActive(false);
    }
    
    #endregion

    #region Métodos para Botones
    
    public void BotonFlores() => CambiarEstado(GameState.FlowerSelection);
    public void BotonCreditos() => CambiarEstado(GameState.Credits);
    public void BotonSalir() => Application.Quit();
    public void BotonVolverAlMenu() => CambiarEstado(GameState.MainMenu);

    public void IniciarNivelGeranio()
    {
        dialogoActual = 0;
        fondoActual = 0;
        macetaActual = 0;
        imagenFondo.sprite = spritesFondo[fondoActual];
        imagenMaceta.sprite = spritesMaceta[macetaActual];
        imagenPlanta.sprite = spriteRetono;
        CambiarEstado(GameState.Tutorial);
    }
    public void AvanzarDialogo()
    {
        panelesDialogo[dialogoActual].SetActive(false);
        dialogoActual++;
        if (dialogoActual < panelesDialogo.Length)
        {
            panelesDialogo[dialogoActual].SetActive(true);
        }
        else
        {
            CambiarEstado(GameState.LevelSetup);
        }
    }

    public void CiclarFondo()
    {
        fondoActual = (fondoActual + 1) % spritesFondo.Length;
        imagenFondo.sprite = spritesFondo[fondoActual];
    }
    public void CiclarMaceta()
    {
        macetaActual = (macetaActual + 1) % spritesMaceta.Length;
        imagenMaceta.sprite = spritesMaceta[macetaActual];
    }
    public void EmpezarJuego()
    {
        riegosActuales = 0;
        abonosActuales = 0;
        CambiarEstado(GameState.Playing);
    }

    public void BotonReanudar() => CambiarEstado(previousState);
    public void BotonSonido() => CambiarEstado(GameState.SoundSettings);
    public void BotonVolverAPausa() => CambiarEstado(GameState.Paused);

    // --- LÓGICA DE JUEGO ---
    public void BotonRegar()
    {
        if (currentState != GameState.Playing) return;
        riegosActuales++;
        Debug.Log("Planta regada. Total: " + riegosActuales);
    }

    public void BotonAbono()
    {
        if (currentState != GameState.Playing) return;
        abonosActuales++;
        Debug.Log("Planta abonada. Total: " + abonosActuales);
    }

    public void BotonPasarTiempo()
    {
        if (currentState != GameState.Playing) return;

        if (riegosActuales >= riegosNecesarios && abonosActuales >= abonosNecesarios)
        {
            Debug.Log("¡Condiciones cumplidas! La planta crece.");
            imagenPlanta.sprite = spriteCrecida;
            riegosActuales = 0;
            abonosActuales = 0;
        }
        else
        {
            Debug.Log("¡Condiciones NO cumplidas! La planta se marchita.");
            Morir();
        }
    }

    // --- FUNCIÓN 'MORIR' MODIFICADA ---
    private void Morir()
    {
        imagenPlanta.sprite = spriteMarchita;
        // Ocultamos los botones de acción
        panelContenedorJuego.SetActive(false);
        // Mostramos el botón de Game Over
        botonVolverMenu.SetActive(true);
    }

    #endregion
}
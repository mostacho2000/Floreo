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
    public GameObject botonVolverMenu;
    public Image imagenFondo;
    public Image imagenMaceta;
    public Image imagenPlanta;
    public Image imagenEscarabajo;

    [Header("Sprites para Setup")]
    public Sprite[] spritesFondo;
    public Sprite[] spritesMaceta;

    [Header("Sprites de la Planta")]
    public Sprite spriteRetono;
    public Sprite spriteCrecida;
    public Sprite spriteFinalizado;
    public Sprite spriteMarchita;

    #endregion

    #region Variables Privadas
    private enum PlantStage { Retono, Crecida, Finalizado }
    private PlantStage currentPlantStage;

    private int dialogoActual = 0;
    private int fondoActual = 0;
    private int macetaActual = 0;
    private GameState previousState;

    private int riegosActuales = 0;
    private bool haPuestoPesticida = false;
    private bool tieneEscarabajo = false;
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
            // ... (casos MainMenu, Credits, FlowerSelection, Tutorial, LevelSetup no cambian) ...
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
            case GameState.Playing:
                if (imagenFondo != null) imagenFondo.gameObject.SetActive(true);
                if (imagenMaceta != null) imagenMaceta.gameObject.SetActive(true);
                if (imagenPlanta != null) imagenPlanta.gameObject.SetActive(true);
                panelContenedorJuego.SetActive(true);
                if (botonVolverMenu != null) botonVolverMenu.SetActive(false);
                break;
            case GameState.Paused:
                if (panelContenedorSetup != null) panelContenedorSetup.SetActive(false);
                if (panelContenedorJuego != null) panelContenedorJuego.SetActive(false);
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
        // ... (sin cambios)
        panelMenu.SetActive(false);
        panelCreditos.SetActive(false);
        panelSelectorDeFlores.SetActive(false);
        panelPausa.SetActive(false);
        panelSonido.SetActive(false);
        panelContenedorTutorial.SetActive(false);
        panelContenedorSetup.SetActive(false);
        panelContenedorJuego.SetActive(false);
        if (botonVolverMenu != null) botonVolverMenu.SetActive(false);

        if (imagenFondo != null) imagenFondo.gameObject.SetActive(false);
        if (imagenMaceta != null) imagenMaceta.gameObject.SetActive(false);
        if (imagenPlanta != null) imagenPlanta.gameObject.SetActive(false);
        if (imagenNpc != null) imagenNpc.gameObject.SetActive(false);
        if (imagenEscarabajo != null) imagenEscarabajo.gameObject.SetActive(false);
    }

    #endregion

    #region Métodos para Botones

    public void BotonFlores() => CambiarEstado(GameState.FlowerSelection);
    public void BotonCreditos() => CambiarEstado(GameState.Credits);
    public void BotonSalir() => Application.Quit();
    public void BotonVolverAlMenu() => CambiarEstado(GameState.MainMenu);

    public void IniciarNivelGeranio()
    {
        // ... (sin cambios)
        dialogoActual = 0;
        fondoActual = 0;
        macetaActual = 0;
        imagenFondo.sprite = spritesFondo[fondoActual];
        imagenMaceta.sprite = spritesMaceta[macetaActual];
        imagenPlanta.sprite = spriteRetono;
        currentPlantStage = PlantStage.Retono;
        tieneEscarabajo = false;
        if (imagenEscarabajo != null) imagenEscarabajo.gameObject.SetActive(false);
        CambiarEstado(GameState.Tutorial);
    }
    public void AvanzarDialogo()
    {
        // ... (sin cambios)
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
        // ... (sin cambios)
        fondoActual = (fondoActual + 1) % spritesFondo.Length;
        imagenFondo.sprite = spritesFondo[fondoActual];
    }

    public void CiclarMaceta()
    {
        // ... (sin cambios)
        macetaActual = (macetaActual + 1) % spritesMaceta.Length;
        imagenMaceta.sprite = spritesMaceta[macetaActual];
    }

    public void EmpezarJuego()
    {
        // ... (sin cambios)
        riegosActuales = 0;
        haPuestoPesticida = false;
        CambiarEstado(GameState.Playing);
    }

    public void BotonReanudar() => CambiarEstado(previousState);
    public void BotonSonido() => CambiarEstado(GameState.SoundSettings);
    public void BotonVolverAPausa() => CambiarEstado(GameState.Paused);

    // --- LÓGICA DE JUEGO ---
    public void BotonRegar()
    {
        // ... (sin cambios)
        if (currentState != GameState.Playing) return;
        riegosActuales++;
        Debug.Log("Planta regada. Total: " + riegosActuales);
    }

    public void BotonPesticida()
    {
        // ... (sin cambios)
        if (currentState != GameState.Playing) return;
        haPuestoPesticida = true;
        Debug.Log("Pesticida aplicado.");
    }

    // --- LÓGICA DE 'PASAR TIEMPO' MODIFICADA ---
    public void BotonPasarTiempo()
    {
        if (currentState != GameState.Playing) return;

        // --- 1. VERIFICACIÓN DE MUERTE (PRIORIDAD ALTA) ---
        // Muerte por riego incorrecto
        if (riegosActuales <= 0 || riegosActuales >= 4)
        {
            Debug.Log("¡GAME OVER! Riego incorrecto (Total: " + riegosActuales + ")");
            Morir();
            return;
        }
        // Muerte por escarabajo no tratado
        if (tieneEscarabajo && !haPuestoPesticida)
        {
            Debug.Log("¡GAME OVER! El escarabajo no fue tratado.");
            Morir();
            return;
        }

        // --- 2. LÓGICA DE PESTICIDA Y ESCARABAJO ---
        // Si llegamos aquí, la planta sobrevive. Ahora vemos qué pasa con el bicho.
        bool bichoCuradoOPrevenido = false;

        if (tieneEscarabajo && haPuestoPesticida)
        {
            // Caso A: Tenía bicho y lo curó.
            tieneEscarabajo = false;
            imagenEscarabajo.gameObject.SetActive(false);
            bichoCuradoOPrevenido = true;
            Debug.Log("Escarabajo eliminado.");
        }
        else if (!tieneEscarabajo && haPuestoPesticida)
        {
            // Caso B: No tenía bicho y puso pesticida (prevención).
            bichoCuradoOPrevenido = true;
            Debug.Log("Planta protegida, no aparecerá escarabajo.");
        }
        else if (!tieneEscarabajo && !haPuestoPesticida)
        {
            // Caso C: No tenía bicho y NO puso pesticida. Aparecerá uno.
            tieneEscarabajo = true;
            imagenEscarabajo.gameObject.SetActive(true);
            bichoCuradoOPrevenido = false; // No está curado ni prevenido
            Debug.Log("¡Aparece un escarabajo!");
        }

        // --- 3. LÓGICA DE CRECIMIENTO ---
        switch (currentPlantStage)
        {
            case PlantStage.Retono:
                // Desde Retoño siempre se avanza a Crecida (con o sin bicho)
                Debug.Log("La planta crece a 'Crecida'.");
                imagenPlanta.sprite = spriteCrecida;
                currentPlantStage = PlantStage.Crecida;
                break;

            case PlantStage.Crecida:
                // --- ¡AQUÍ ESTÁ TU NUEVA LÓGICA! ---
                // Solo avanza a Finalizado si el bicho fue curado o prevenido
                if (bichoCuradoOPrevenido)
                {
                    Debug.Log("La planta crece a 'Finalizado'.");
                    imagenPlanta.sprite = spriteFinalizado;
                    currentPlantStage = PlantStage.Finalizado;
                    GanarNivel();
                }
                else
                {
                    // Si no puso pesticida (Caso C), el bicho aparece
                    // pero la planta NO avanza.
                    Debug.Log("La planta se queda en 'Crecida' por el nuevo escarabajo.");
                }
                break;

            case PlantStage.Finalizado:
                Debug.Log("La planta ya está en su etapa final.");
                break;
        }

        // Reseteamos contadores de acciones para el próximo turno
        riegosActuales = 0;
        haPuestoPesticida = false;
    }

    private void Morir()
    {
        // ... (sin cambios)
        imagenPlanta.sprite = spriteMarchita;
        panelContenedorJuego.SetActive(false);
        botonVolverMenu.SetActive(true);
        if (imagenEscarabajo != null) imagenEscarabajo.gameObject.SetActive(false);
    }

    private void GanarNivel()
    {
        // ... (sin cambios)
        Debug.Log("¡Nivel Ganado! Mostrando botón de menú.");
        panelContenedorJuego.SetActive(false);
        botonVolverMenu.SetActive(true);
        if (imagenEscarabajo != null) imagenEscarabajo.gameObject.SetActive(false);
    }

    #endregion
}
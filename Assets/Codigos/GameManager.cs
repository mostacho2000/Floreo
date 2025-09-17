// Importamos las librerías necesarias de Unity.
using UnityEngine;
using UnityEngine.UI; // Necesaria para trabajar con elementos de la UI como Botones e Imágenes.
using System.IO; // Necesaria para futuras funciones de guardado de imagen.

public class GameManager : MonoBehaviour
{
    #region Estados del Juego
    // Usaremos un "enum" para definir los posibles estados del juego.
    // Es una forma muy ordenada de saber en qué parte del juego estamos.
    public enum GameState
    {
        MainMenu,
        Credits,
        FlowerSelection,
        LevelSetup, // Pantalla para elegir fondo y maceta
        Playing,
        Paused,
        SoundSettings,
        LevelWon
    }
    public GameState currentState; // Variable para guardar el estado actual.
    #endregion

    #region Variables Públicas (Para arrastrar en el Inspector de Unity)

    [Header("Paneles Principales")]
    public GameObject panelMenu;
    public GameObject panelCreditos;
    public GameObject panelSelectorDeFlores;
    public GameObject panelPausa;
    public GameObject panelSonido;
    public GameObject panelSetupNivel; // Panel para elegir fondo y maceta
    public GameObject panelHUDJuego; // Panel con los botones de regar, pesticida, etc.
    public GameObject panelVictoria; // Panel que aparece al ganar

    [Header("Botones de Selección de Flor")]
    public Button botonFlorGeranio;
    public Button botonFlor2; // A futuro
    public Button botonFlor3; // A futuro
    public Button botonFlor4; // A futuro

    [Header("Elementos del Nivel (Juego)")]
    public Image imagenFondo;
    public Image imagenMaceta;
    public Image imagenFlor;
    public GameObject imagenInsecto; // Usamos GameObject para poder apagarlo/prenderlo fácilmente

    [Header("Sprites (Imágenes para cambiar)")]
    public Sprite fondoJardinSoleado;
    public Sprite fondoJardinSombra;
    public Sprite fondoDentroCasa;
    public Sprite macetaNutrientes;
    public Sprite macetaNormal;

    [Header("Sprites del Geranio")]
    public Sprite geranioRetono;
    public Sprite geranioCrecida;
    public Sprite geranioFlorecida;
    public Sprite geranioMuerta;

    [Header("Tutorial")]
    public GameObject[] panelesTutorial; // Un array para los 3 paneles de diálogo

    #endregion

    #region Variables Privadas (Estado interno del nivel)

    // Un enum para las etapas de la planta
    private enum FlowerStage { Retono, Crecida, Florecida }
    private FlowerStage currentFlowerStage;

    // Banderas (flags) para controlar las acciones del jugador en cada turno
    private bool haRegado = false;
    private bool haPuestoPesticida = false;
    private bool tienePlaga = false;

    // Variables para guardar la elección del jugador
    private int fondoElegidoID; // 1: Sol, 2: Sombra, 3: Casa

    // Variable para el tutorial
    private int pasoTutorialActual = 0;

    // Variable para desbloquear niveles
    private bool nivelGeranioCompletado = false;

    #endregion

    #region Ciclo de Vida de Unity (Start y Update)

    void Start()
    {
        // El juego siempre empieza en el menú principal.
        CambiarEstado(GameState.MainMenu);
        // Asegurarnos que los botones de otros niveles estén desactivados.
        botonFlor2.interactable = false;
        botonFlor3.interactable = false;
        botonFlor4.interactable = false;
    }

    void Update()
    {
        // Si el jugador presiona la tecla "Escape" y está jugando, se abre el menú de pausa.
        if (Input.GetKeyDown(KeyCode.Escape) && currentState == GameState.Playing)
        {
            CambiarEstado(GameState.Paused);
        }
    }

    #endregion

    #region Máquina de Estados Principal

    // Este es el método central que apaga y prende los paneles correctos.
    void CambiarEstado(GameState nuevoEstado)
    {
        currentState = nuevoEstado;

        // Primero, por seguridad, apagamos todos los paneles.
        panelMenu.SetActive(false);
        panelCreditos.SetActive(false);
        panelSelectorDeFlores.SetActive(false);
        panelPausa.SetActive(false);
        panelSonido.SetActive(false);
        panelSetupNivel.SetActive(false);
        panelHUDJuego.SetActive(false);
        panelVictoria.SetActive(false);
        imagenInsecto.SetActive(false);

        // Apagamos los diálogos del tutorial
        foreach (var panel in panelesTutorial)
        {
            panel.SetActive(false);
        }

        // Ahora, activamos solo el panel del estado correspondiente.
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
                // Actualizamos si los botones de nivel están disponibles
                botonFlor2.interactable = nivelGeranioCompletado;
                break;

            case GameState.LevelSetup:
                panelSetupNivel.SetActive(true);
                break;

            case GameState.Playing:
                panelHUDJuego.SetActive(true);
                // Si es la primera vez, mostramos el tutorial.
                if (pasoTutorialActual < panelesTutorial.Length)
                {
                    panelesTutorial[pasoTutorialActual].SetActive(true);
                }
                break;

            case GameState.Paused:
                panelPausa.SetActive(true);
                // El panel de pausa debe verse por encima del juego.
                panelHUDJuego.SetActive(true);
                break;

            case GameState.SoundSettings:
                panelSonido.SetActive(true);
                // El de sonido se ve sobre el de pausa.
                panelPausa.SetActive(true);
                panelHUDJuego.SetActive(true);
                break;

            case GameState.LevelWon:
                panelVictoria.SetActive(true);
                panelHUDJuego.SetActive(true); // Dejamos el fondo del juego visible
                break;
        }
    }
    #endregion

    #region Métodos para los Botones del Menú y Pausa

    public void BotonFlores() => CambiarEstado(GameState.FlowerSelection);
    public void BotonCreditos() => CambiarEstado(GameState.Credits);
    public void BotonSalir()
    {
        Debug.Log("Saliendo del juego..."); // Mensaje para probar en el editor
        Application.Quit();
    }
    public void BotonVolverAlMenu() => CambiarEstado(GameState.MainMenu);
    public void BotonPausa() => CambiarEstado(GameState.Paused);
    public void BotonSonido() => CambiarEstado(GameState.SoundSettings);
    public void BotonVolverAPausa() => CambiarEstado(GameState.Paused);

    #endregion

    #region Métodos para el Setup del Nivel

    public void ElegirFondo(int id)
    {
        fondoElegidoID = id;
        if (id == 1) imagenFondo.sprite = fondoJardinSoleado;
        if (id == 2) imagenFondo.sprite = fondoJardinSombra;
        if (id == 3) imagenFondo.sprite = fondoDentroCasa;
    }

    public void ElegirMaceta(int id)
    {
        if (id == 1) imagenMaceta.sprite = macetaNutrientes;
        if (id == 2) imagenMaceta.sprite = macetaNormal;
    }

    public void IniciarNivelGeranio()
    {
        // Reiniciamos todas las variables del nivel del geranio
        currentFlowerStage = FlowerStage.Retono;
        imagenFlor.sprite = geranioRetono;
        haRegado = false;
        haPuestoPesticida = false;
        tienePlaga = false;
        pasoTutorialActual = 0;

        CambiarEstado(GameState.Playing);
    }
    #endregion

    #region Tutorial
    public void SiguientePasoTutorial()
    {
        // Apagamos el panel actual
        panelesTutorial[pasoTutorialActual].SetActive(false);
        pasoTutorialActual++;
        // Si hay un siguiente paso, lo mostramos.
        if (pasoTutorialActual < panelesTutorial.Length)
        {
            panelesTutorial[pasoTutorialActual].SetActive(true);
        }
    }
    #endregion

    #region Lógica del Juego (Acciones del Jugador)

    public void BotonRegar()
    {
        if (currentState != GameState.Playing) return;
        haRegado = true;
        Debug.Log("Planta regada.");
        // Aquí podrías añadir un feedback visual, como cambiar el color del botón.
    }

    public void BotonPesticida()
    {
        if (currentState != GameState.Playing) return;
        haPuestoPesticida = true;
        Debug.Log("Pesticida aplicado.");
    }

    public void BotonPasarTiempo()
    {
        if (currentState != GameState.Playing) return;
        Debug.Log("Evaluando crecimiento...");

        // --- LÓGICA ESPECÍFICA DEL GERANIO ---

        // 1. Condición de MUERTE: Si no se riega, muere.
        if (!haRegado)
        {
            Morir();
            return;
        }

        // 2. Condición de PLAGA: Si no se pone pesticida, aparece un insecto.
        if (!haPuestoPesticida && !tienePlaga)
        {
            tienePlaga = true;
            imagenInsecto.SetActive(true);
            Debug.Log("¡Ha aparecido una plaga!");
            // El turno no avanza, se queda en la misma etapa pero con el bicho.
            // Reseteamos las acciones para el siguiente intento.
            haRegado = false;
            haPuestoPesticida = false;
            return;
        }

        // 3. Condición de MUERTE: Si tiene plaga, se riega, pero no se le pone pesticida, muere.
        if (tienePlaga && !haPuestoPesticida)
        {
            Morir();
            return;
        }
        
        // 4. Condición de CURACIÓN: Si tiene plaga, pero se riega y se pone pesticida, se cura.
        if (tienePlaga && haPuestoPesticida)
        {
            tienePlaga = false;
            imagenInsecto.SetActive(false);
            Debug.Log("¡Plaga eliminada!");
        }

        // 5. Condición de CRECIMIENTO: Si se regó, no hay plaga (o ya se curó), y se puso pesticida.
        if (haRegado && !tienePlaga)
        {
            switch (currentFlowerStage)
            {
                case FlowerStage.Retono:
                    currentFlowerStage = FlowerStage.Crecida;
                    imagenFlor.sprite = geranioCrecida;
                    Debug.Log("La planta ha crecido.");
                    break;
                case FlowerStage.Crecida:
                    // Condición especial del geranio: no puede estar dentro de casa.
                    if (fondoElegidoID == 3) // 3 es "Dentro de Casa"
                    {
                        Debug.Log("El geranio no puede florecer dentro de casa.");
                        Morir();
                    }
                    else
                    {
                        currentFlowerStage = FlowerStage.Florecida;
                        imagenFlor.sprite = geranioFlorecida;
                        Debug.Log("¡El geranio ha florecido!");
                        GanarNivel();
                    }
                    break;
            }
        }
        
        // Al final de cada turno exitoso, reseteamos las acciones del jugador.
        haRegado = false;
        haPuestoPesticida = false;
    }

    private void Morir()
    {
        imagenFlor.sprite = geranioMuerta;
        Debug.Log("La planta ha muerto. Reinicia el nivel desde el menú.");
        // Desactivamos los botones de acción para que el jugador deba reiniciar.
        panelHUDJuego.SetActive(false);
        // Podrías mostrar un panel de "Has perdido" y un botón para reiniciar.
    }

    private void GanarNivel()
    {
        nivelGeranioCompletado = true;
        CambiarEstado(GameState.LevelWon);
    }
    #endregion

    #region Funcionalidad Extra (Descargar PNG)

    public void DescargarPNG()
    {
        // Esta función es un ejemplo. Guardar archivos depende mucho de la plataforma
        // (PC, Web, Móvil). Esto funcionaría para PC.
        Debug.Log("Función de descarga llamada. Aún por implementar la lógica de guardado real.");
        
        // Aquí iría el código para capturar una porción de la pantalla o combinar
        // los sprites y guardarlos como un archivo PNG. Es una lógica más avanzada.
        // Por ahora, solo muestra un mensaje.
    }

    #endregion
}
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
    public Image imagenFondo;
    public Image imagenMaceta;
    public Image imagenPlanta;
    public Image imagenEscarabajo;

    [Header("Elementos de Fin de Nivel")] // <-- NUEVA SECCIÓN PARA ORDENAR
    public GameObject botonVolverMenu_Win;
    public GameObject botonVolverMenu_Lose;
    public GameObject botonDescarga;
    public GameObject imagenVictoria; // <--- NUEVA IMAGEN
    public GameObject imagenDerrota;  // <--- NUEVA IMAGEN

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
            case GameState.Playing:
                if (imagenFondo != null) imagenFondo.gameObject.SetActive(true);
                if (imagenMaceta != null) imagenMaceta.gameObject.SetActive(true);
                if (imagenPlanta != null) imagenPlanta.gameObject.SetActive(true);
                panelContenedorJuego.SetActive(true);
                // Nos aseguramos de apagar los botones y pantallas de fin de juego
                if (botonVolverMenu_Win != null) botonVolverMenu_Win.SetActive(false);
                if (botonVolverMenu_Lose != null) botonVolverMenu_Lose.SetActive(false);
                if (botonDescarga != null) botonDescarga.SetActive(false);
                if (imagenVictoria != null) imagenVictoria.SetActive(false);
                if (imagenDerrota != null) imagenDerrota.SetActive(false);
                break;
            case GameState.Paused:
                if (panelContenedorSetup != null) panelContenedorSetup.SetActive(false);
                if (panelContenedorJuego != null) panelContenedorJuego.SetActive(false);
                // Apagamos también los botones y pantallas de fin de juego
                if (botonVolverMenu_Win != null) botonVolverMenu_Win.SetActive(false);
                if (botonVolverMenu_Lose != null) botonVolverMenu_Lose.SetActive(false);
                if (botonDescarga != null) botonDescarga.SetActive(false);
                if (imagenVictoria != null) imagenVictoria.SetActive(false);
                if (imagenDerrota != null) imagenDerrota.SetActive(false);

                panelSonido.SetActive(false);
                panelPausa.SetActive(true);
                break;
            default:
                ActivarPanelesSegunEstado(nuevoEstado);
                break;
        }
    }

    void ActivarPanelesSegunEstado(GameState estado)
    {
        // ... (sin cambios)
        panelMenu.SetActive(estado == GameState.MainMenu);
        panelCreditos.SetActive(estado == GameState.Credits);
        panelSelectorDeFlores.SetActive(estado == GameState.FlowerSelection);
        panelPausa.SetActive(estado == GameState.Paused);
        panelSonido.SetActive(estado == GameState.SoundSettings);

        if (estado == GameState.Tutorial)
        {
            if (imagenFondo != null) imagenFondo.gameObject.SetActive(true);
            panelContenedorTutorial.SetActive(true);
            if (imagenNpc != null) imagenNpc.gameObject.SetActive(true);
            foreach (var panel in panelesDialogo) { if (panel != null) panel.SetActive(false); }
            if (panelesDialogo.Length > 0) panelesDialogo[dialogoActual].SetActive(true);
        }

        if (estado == GameState.LevelSetup)
        {
            if (imagenFondo != null) imagenFondo.gameObject.SetActive(true);
            if (imagenMaceta != null) imagenMaceta.gameObject.SetActive(true);
            panelContenedorSetup.SetActive(true);
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
        
        if (botonVolverMenu_Win != null) botonVolverMenu_Win.SetActive(false);
        if (botonVolverMenu_Lose != null) botonVolverMenu_Lose.SetActive(false);
        if (botonDescarga != null) botonDescarga.SetActive(false);
        if (imagenVictoria != null) imagenVictoria.SetActive(false); // <--- NUEVA LÍNEA
        if (imagenDerrota != null) imagenDerrota.SetActive(false);  // <--- NUEVA LÍNEA

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

    // ... (IniciarNivelGeranio, AvanzarDialogo, CiclarFondo, CiclarMaceta, EmpezarJuego, Pausa, etc. no cambian) ...
    public void IniciarNivelGeranio()
    {
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
        haPuestoPesticida = false;
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
    public void BotonPesticida()
    {
        if (currentState != GameState.Playing) return;
        haPuestoPesticida = true;
        Debug.Log("Pesticida aplicado.");
    }
    public void BotonPasarTiempo()
    {
        // ... (La lógica de BotonPasarTiempo no cambia en absoluto)
        if (currentState != GameState.Playing) return;
        if (riegosActuales <= 0 || riegosActuales >= 4)
        {
            Debug.Log("¡GAME OVER! Riego incorrecto (Total: " + riegosActuales + ")");
            Morir();
            return;
        }
        if (tieneEscarabajo && !haPuestoPesticida)
        {
            Debug.Log("¡GAME OVER! El escarabajo no fue tratado.");
            Morir();
            return;
        }
        bool bichoCuradoOPrevenido = false;
        if (tieneEscarabajo && haPuestoPesticida)
        {
            tieneEscarabajo = false;
            imagenEscarabajo.gameObject.SetActive(false);
            bichoCuradoOPrevenido = true;
            Debug.Log("Escarabajo eliminado.");
        }
        else if (!tieneEscarabajo && haPuestoPesticida)
        {
            bichoCuradoOPrevenido = true;
            Debug.Log("Planta protegida, no aparecerá escarabajo.");
        }
        else if (!tieneEscarabajo && !haPuestoPesticida)
        {
            tieneEscarabajo = true;
            imagenEscarabajo.gameObject.SetActive(true);
            bichoCuradoOPrevenido = false;
            Debug.Log("¡Aparece un escarabajo!");
        }
        switch (currentPlantStage)
        {
            case PlantStage.Retono:
                Debug.Log("La planta crece a 'Crecida'.");
                imagenPlanta.sprite = spriteCrecida;
                currentPlantStage = PlantStage.Crecida;
                break;
            case PlantStage.Crecida:
                if (bichoCuradoOPrevenido)
                {
                    Debug.Log("La planta crece a 'Finalizado'.");
                    imagenPlanta.sprite = spriteFinalizado;
                    currentPlantStage = PlantStage.Finalizado;
                    GanarNivel();
                }
                else
                {
                    Debug.Log("La planta se queda en 'Crecida' por el nuevo escarabajo.");
                }
                break;
            case PlantStage.Finalizado:
                Debug.Log("La planta ya está en su etapa final.");
                break;
        }
        riegosActuales = 0;
        haPuestoPesticida = false;
    }

    // --- FUNCIONES 'MORIR' Y 'GANAR' MODIFICADAS ---
    private void Morir()
    {
        imagenPlanta.sprite = spriteMarchita;
        panelContenedorJuego.SetActive(false);
        
        // Mostramos SOLO los elementos de Game Over
        if (imagenDerrota != null) imagenDerrota.SetActive(true); // <--- NUEVA LÍNEA
        botonVolverMenu_Lose.SetActive(true);

        // Ocultamos los de victoria
        if (imagenVictoria != null) imagenVictoria.SetActive(false);
        botonVolverMenu_Win.SetActive(false);
        botonDescarga.SetActive(false); 
        
        if (imagenEscarabajo != null) imagenEscarabajo.gameObject.SetActive(false);
    }

    private void GanarNivel()
    {
        Debug.Log("¡Nivel Ganado! Mostrando botones.");
        panelContenedorJuego.SetActive(false);
        
        // Mostramos los elementos de victoria
        if (imagenVictoria != null) imagenVictoria.SetActive(true); // <--- NUEVA LÍNEA
        botonVolverMenu_Win.SetActive(true);
        botonDescarga.SetActive(true);

        // Ocultamos los de derrota
        if (imagenDerrota != null) imagenDerrota.SetActive(false);
        botonVolverMenu_Lose.SetActive(false);
        
        if (imagenEscarabajo != null) imagenEscarabajo.gameObject.SetActive(false);
    }

    public void BotonDescarga()
    {
        Debug.Log("¡Botón de descarga presionado!");
    }

    #endregion
}
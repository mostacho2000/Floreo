using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// --- NUEVA CLASE DE DATOS ---
// Esto define un "paquete" de datos. No es un script,
// solo un molde para organizar la información.
[System.Serializable]
public class FlowerData
{
    [Header("Identificación")]
    public string flowerName; // Para depuración

    [Header("Diálogos del Tutorial")]
    public GameObject[] panelesDialogo = new GameObject[3];

    [Header("GameObjects de la Planta")]
    public GameObject plantaRetono;
    public GameObject plantaCrecida;
    public GameObject plantaFinalizado;
    public GameObject plantaMarchita;

    [Header("Reglas de Crecimiento")]
    public int riegosEtapa1;     // Retoño -> Crecida
    public int pesticidasEtapa1; // Retoño -> Crecida
    public int riegosEtapa2;     // Crecida -> Finalizado
    public int pesticidasEtapa2; // Crecida -> Finalizado
}
// --- FIN DE LA NUEVA CLASE ---


public class GameManager : MonoBehaviour
{
    #region Estados del Juego
    public enum GameState { MainMenu, Credits, FlowerSelection, Tutorial, LevelSetup, Playing, Paused, SoundSettings }
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
    // public GameObject[] panelesDialogo; // <-- ELIMINADO (Ahora en FlowerData)
    public Image imagenNpc;

    [Header("Elementos del Nivel")]
    public GameObject panelContenedorSetup;
    public GameObject panelContenedorJuego;
    public Image imagenFondo;
    public Image imagenMaceta;
    public Image imagenEscarabajo;
    public Slider barraAgua;
    public Slider barraPesticida;

    [Header("Elementos de Fin de Nivel")]
    public GameObject botonVolverMenu_Win;
    public GameObject botonVolverMenu_Lose;
    public GameObject botonDescarga;
    public GameObject imagenVictoria;
    public GameObject imagenDerrota;

    [Header("Sprites para Setup")]
    public Sprite[] spritesFondo;
    public Sprite[] spritesMaceta;

    // --- SECCIÓN DE DATOS DE FLORES (REEMPLAZA A LAS IMÁGENES DE PLANTA) ---
    [Header("Datos de las Flores")]
    public FlowerData[] todasLasFlores; // Aquí irán tus 4 flores

    #endregion

    #region Variables Privadas
    private enum PlantStage { Retono, Crecida, Finalizado }
    private PlantStage currentPlantStage;
    
    private FlowerData currentFlowerData; // La flor que se está jugando
    private int riegosRequeridos;
    private int pesticidasRequeridos;
    
    private int dialogoActual = 0;
    private int fondoActual = 0;
    private int macetaActual = 0;
    private GameState previousState;

    private int riegosActuales = 0;
    private int pesticidasActuales = 0; // <--- CAMBIO A INT
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
                
                // Activamos solo la planta correcta de la flor correcta
                switch (currentPlantStage)
                {
                    case PlantStage.Retono:
                        if (currentFlowerData.plantaRetono != null) currentFlowerData.plantaRetono.SetActive(true);
                        break;
                    case PlantStage.Crecida:
                        if (currentFlowerData.plantaCrecida != null) currentFlowerData.plantaCrecida.SetActive(true);
                        break;
                    case PlantStage.Finalizado:
                        if (currentFlowerData.plantaFinalizado != null) currentFlowerData.plantaFinalizado.SetActive(true);
                        break;
                }

                panelContenedorJuego.SetActive(true);
                if (barraAgua != null) barraAgua.gameObject.SetActive(true);
                if (barraPesticida != null) barraPesticida.gameObject.SetActive(true);
                if (botonVolverMenu_Win != null) botonVolverMenu_Win.SetActive(false);
                if (botonVolverMenu_Lose != null) botonVolverMenu_Lose.SetActive(false);
                if (botonDescarga != null) botonDescarga.SetActive(false);
                if (imagenVictoria != null) imagenVictoria.SetActive(false);
                if (imagenDerrota != null) imagenDerrota.SetActive(false);
                break;
            case GameState.Paused:
                if (panelContenedorSetup != null) panelContenedorSetup.SetActive(false);
                if (panelContenedorJuego != null) panelContenedorJuego.SetActive(false);
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
            // Apagamos todos los diálogos de la flor actual
            foreach (var panel in currentFlowerData.panelesDialogo) { if (panel != null) panel.SetActive(false); }
            // Encendemos solo el primero
            if (currentFlowerData.panelesDialogo.Length > 0) currentFlowerData.panelesDialogo[dialogoActual].SetActive(true);
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
        if (barraAgua != null) barraAgua.gameObject.SetActive(false);
        if (barraPesticida != null) barraPesticida.gameObject.SetActive(false);
        if (botonVolverMenu_Win != null) botonVolverMenu_Win.SetActive(false);
        if (botonVolverMenu_Lose != null) botonVolverMenu_Lose.SetActive(false);
        if (botonDescarga != null) botonDescarga.SetActive(false);
        if (imagenVictoria != null) imagenVictoria.SetActive(false);
        if (imagenDerrota != null) imagenDerrota.SetActive(false);
        if (imagenFondo != null) imagenFondo.gameObject.SetActive(false);
        if (imagenMaceta != null) imagenMaceta.gameObject.SetActive(false);
        if (imagenNpc != null) imagenNpc.gameObject.SetActive(false);
        if (imagenEscarabajo != null) imagenEscarabajo.gameObject.SetActive(false);

        // Apagamos TODAS las imágenes de TODAS las flores
        if (todasLasFlores != null)
        {
            foreach (FlowerData flower in todasLasFlores)
            {
                if (flower.plantaRetono != null) flower.plantaRetono.SetActive(false);
                if (flower.plantaCrecida != null) flower.plantaCrecida.SetActive(false);
                if (flower.plantaFinalizado != null) flower.plantaFinalizado.SetActive(false);
                if (flower.plantaMarchita != null) flower.plantaMarchita.SetActive(false);
            }
        }
    }
    
    #endregion

    #region Métodos para Botones
    
    public void BotonFlores() => CambiarEstado(GameState.FlowerSelection);
    public void BotonCreditos() => CambiarEstado(GameState.Credits);
    public void BotonSalir() => Application.Quit();
    public void BotonVolverAlMenu() => CambiarEstado(GameState.MainMenu);

    // --- ¡NUEVO! ---
    // Esta función será llamada por los 4 botones de flores
    // Geranio: 0, Rosa: 1, Girasol: 2, Noche Buena: 3
    public void IniciarNivel(int flowerIndex)
    {
        if (flowerIndex >= todasLasFlores.Length)
        {
            Debug.LogError("Índice de flor inválido: " + flowerIndex);
            return;
        }

        // Cargamos los datos de la flor seleccionada
        currentFlowerData = todasLasFlores[flowerIndex];
        
        dialogoActual = 0;
        fondoActual = 0;
        macetaActual = 0;
        imagenFondo.sprite = spritesFondo[fondoActual];
        imagenMaceta.sprite = spritesMaceta[macetaActual];
        
        tieneEscarabajo = false;
        if (imagenEscarabajo != null) imagenEscarabajo.gameObject.SetActive(false);
        
        CambiarEstado(GameState.Tutorial);
    }

    public void AvanzarDialogo()
    {
        currentFlowerData.panelesDialogo[dialogoActual].SetActive(false);
        dialogoActual++;
        if (dialogoActual < currentFlowerData.panelesDialogo.Length)
        {
            currentFlowerData.panelesDialogo[dialogoActual].SetActive(true);
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
        pesticidasActuales = 0;
        currentPlantStage = PlantStage.Retono;
        
        // --- CONFIGURAR BARRAS CON REGLAS DE ETAPA 1 ---
        riegosRequeridos = currentFlowerData.riegosEtapa1;
        pesticidasRequeridos = currentFlowerData.pesticidasEtapa1;
        
        barraAgua.maxValue = riegosRequeridos;
        barraAgua.value = 0;
        barraPesticida.maxValue = pesticidasRequeridos;
        barraPesticida.value = 0;
        
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
        barraAgua.value = riegosActuales;
    }

    public void BotonPesticida()
    {
        if (currentState != GameState.Playing) return;
        pesticidasActuales++;
        barraPesticida.value = pesticidasActuales;
    }

    public void BotonPasarTiempo()
    {
        if (currentState != GameState.Playing) return;

        // --- 1. VERIFICACIÓN DE MUERTE (PRIORIDAD ALTA) ---
        // Muerte por riego incorrecto (debe ser EXACTAMENTE lo requerido)
        if (riegosActuales != riegosRequeridos)
        {
            Debug.Log("¡GAME OVER! Riego incorrecto (Total: " + riegosActuales + ", se necesitaban " + riegosRequeridos + ")");
            Morir();
            return;
        }
        // Muerte por escarabajo no tratado
        if (tieneEscarabajo && pesticidasActuales < pesticidasRequeridos)
        {
            Debug.Log("¡GAME OVER! El escarabajo no fue tratado.");
            Morir();
            return;
        }

        // --- 2. LÓGICA DE PESTICIDA Y ESCARABAJO ---
        bool pesticidaSuficiente = (pesticidasActuales >= pesticidasRequeridos);
        
        if (tieneEscarabajo && pesticidaSuficiente)
        {
            tieneEscarabajo = false;
            imagenEscarabajo.gameObject.SetActive(false);
            Debug.Log("Escarabajo eliminado.");
        }
        else if (!tieneEscarabajo && !pesticidaSuficiente)
        {
            tieneEscarabajo = true;
            imagenEscarabajo.gameObject.SetActive(true);
            Debug.Log("¡Aparece un escarabajo!");
        }

        // --- 3. LÓGICA DE CRECIMIENTO ---
        switch (currentPlantStage)
        {
            case PlantStage.Retono:
                Debug.Log("La planta crece a 'Crecida'.");
                currentFlowerData.plantaRetono.SetActive(false);
                currentFlowerData.plantaCrecida.SetActive(true);
                currentPlantStage = PlantStage.Crecida;
                
                // Actualizamos las reglas para la ETAPA 2
                riegosRequeridos = currentFlowerData.riegosEtapa2;
                pesticidasRequeridos = currentFlowerData.pesticidasEtapa2;
                break;
            case PlantStage.Crecida:
                if (pesticidaSuficiente)
                {
                    Debug.Log("La planta crece a 'Finalizado'.");
                    currentFlowerData.plantaCrecida.SetActive(false);
                    currentFlowerData.plantaFinalizado.SetActive(true);
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
        
        // --- RESETEAR BARRAS Y CONTADORES ---
        riegosActuales = 0;
        pesticidasActuales = 0;
        barraAgua.maxValue = riegosRequeridos; // Actualiza el máximo
        barraPesticida.maxValue = pesticidasRequeridos; // Actualiza el máximo
        barraAgua.value = 0;
        barraPesticida.value = 0;
    }

    private void ApagarPlantasActivas()
    {
        if (currentFlowerData == null) return;
        if (currentFlowerData.plantaRetono != null) currentFlowerData.plantaRetono.SetActive(false);
        if (currentFlowerData.plantaCrecida != null) currentFlowerData.plantaCrecida.SetActive(false);
        if (currentFlowerData.plantaFinalizado != null) currentFlowerData.plantaFinalizado.SetActive(false);
        if (currentFlowerData.plantaMarchita != null) currentFlowerData.plantaMarchita.SetActive(false);
    }
    
    private void Morir()
    {
        ApagarPlantasActivas(); // Apagamos la planta activa
        currentFlowerData.plantaMarchita.SetActive(true); // Encendemos la marchita
        
        panelContenedorJuego.SetActive(false);
        if (imagenDerrota != null) imagenDerrota.SetActive(true);
        botonVolverMenu_Lose.SetActive(true);
        if (imagenVictoria != null) imagenVictoria.SetActive(false);
        botonVolverMenu_Win.SetActive(false);
        botonDescarga.SetActive(false); 
        if (imagenEscarabajo != null) imagenEscarabajo.gameObject.SetActive(false);
    }

    private void GanarNivel()
    {
        // La planta finalizada ya se activó en BotonPasarTiempo()
        panelContenedorJuego.SetActive(false);
        if (imagenVictoria != null) imagenVictoria.SetActive(true);
        botonVolverMenu_Win.SetActive(true);
        botonDescarga.SetActive(true);
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
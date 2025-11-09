using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.IO;
using System;

// --- CLASE DE DATOS (Sin cambios) ---
[System.Serializable]
public class FlowerData
{
    [Header("Identificación")]
    public string flowerName;
    [Header("Diálogos del Tutorial")]
    public GameObject[] panelesDialogo = new GameObject[3];
    [Header("GameObjects de la Planta")]
    public GameObject plantaRetono;
    public GameObject plantaCrecida;
    public GameObject plantaFinalizado;
    public GameObject plantaMarchita;
    [Header("Imagen de Recompensa")]
    public Sprite rewardImage;
    [Header("Reglas de Crecimiento")]
    public int riegosEtapa1;
    public int pesticidasEtapa1;
    public int riegosEtapa2;
    public int pesticidasEtapa2;
}

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
    
    [Header("Botones de Selección de Flor")]
    public Button botonGeranio;
    public Button botonRosa;
    public Button botonGirasol;
    public Button botonNocheBuena;

    [Header("Elementos del Tutorial")]
    public GameObject panelContenedorTutorial;
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

    [Header("Datos de las Flores")]
    public FlowerData[] todasLasFlores;
    #endregion

    #region Variables Privadas
    // --- SECCIÓN CORREGIDA ---
    private enum PlantStage { Retono, Crecida, Finalizado }
    private PlantStage currentPlantStage;
    
    private FlowerData currentFlowerData;
    private int riegosRequeridos;
    private int pesticidasRequeridos;
    
    private int dialogoActual = 0;
    private int fondoActual = 0;
    private int macetaActual = 0;
    private GameState previousState;

    private int riegosActuales = 0;
    private int pesticidasActuales = 0; // <-- ¡ESTA ES LA LÍNEA CORREGIDA! (Era 'bool haPuestoPesticida')
    private bool tieneEscarabajo = false;
    
    private bool geranioCompletado = false;
    // --- FIN DE LA CORRECCIÓN ---
    #endregion

    #region Ciclo de Vida de Unity
    void Start()
    {
        CambiarEstado(GameState.MainMenu);
        geranioCompletado = PlayerPrefs.GetInt("GeranioCompletado", 0) == 1;
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
        panelPausa.SetActive(estado == GameState.Paused);
        panelSonido.SetActive(estado == GameState.SoundSettings);

        panelSelectorDeFlores.SetActive(estado == GameState.FlowerSelection);
        if (estado == GameState.FlowerSelection)
        {
            ActualizarBotonesDeNivel();
        }

        if (estado == GameState.Tutorial)
        {
            if (imagenFondo != null) imagenFondo.gameObject.SetActive(true);
            panelContenedorTutorial.SetActive(true);
            if (imagenNpc != null) imagenNpc.gameObject.SetActive(true);
            foreach (var panel in currentFlowerData.panelesDialogo) { if (panel != null) panel.SetActive(false); }
            if (currentFlowerData.panelesDialogo.Length > 0) currentFlowerData.panelesDialogo[dialogoActual].SetActive(true);
        }

        if (estado == GameState.LevelSetup)
        {
            if (imagenFondo != null) imagenFondo.gameObject.SetActive(true);
            if (imagenMaceta != null) imagenMaceta.gameObject.SetActive(true);
            panelContenedorSetup.SetActive(true);
        }
    }

    void ActualizarBotonesDeNivel()
    {
        if (botonGeranio != null) botonGeranio.interactable = true;
        if (botonRosa != null) botonRosa.interactable = geranioCompletado;
        if (botonGirasol != null) botonGirasol.interactable = geranioCompletado;
        if (botonNocheBuena != null) botonNocheBuena.interactable = geranioCompletado;
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
        
        if (todasLasFlores != null)
        {
            foreach (FlowerData flower in todasLasFlores)
            {
                if (flower == null) continue;
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

    public void IniciarNivel(int flowerIndex)
    {
        if (flowerIndex >= todasLasFlores.Length)
        {
            Debug.LogError("Índice de flor inválido: " + flowerIndex);
            return;
        }
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

        if (riegosActuales != riegosRequeridos)
        {
            Morir();
            return;
        }

        bool pesticidaSuficiente = (pesticidasActuales >= pesticidasRequeridos);

        if (tieneEscarabajo && !pesticidaSuficiente)
        {
            Morir();
            return;
        }
        
        if (tieneEscarabajo && pesticidaSuficiente)
        {
            tieneEscarabajo = false;
            imagenEscarabajo.gameObject.SetActive(false);
        }
        else if (!tieneEscarabajo && !pesticidaSuficiente)
        {
            tieneEscarabajo = true;
            imagenEscarabajo.gameObject.SetActive(true);
        }

        switch (currentPlantStage)
        {
            case PlantStage.Retono:
                currentFlowerData.plantaRetono.SetActive(false);
                currentFlowerData.plantaCrecida.SetActive(true);
                currentPlantStage = PlantStage.Crecida;
                riegosRequeridos = currentFlowerData.riegosEtapa2;
                pesticidasRequeridos = currentFlowerData.pesticidasEtapa2;
                break;
            case PlantStage.Crecida:
                if (pesticidaSuficiente)
                {
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
        
        riegosActuales = 0;
        pesticidasActuales = 0;
        barraAgua.maxValue = riegosRequeridos;
        barraPesticida.maxValue = pesticidasRequeridos;
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
        ApagarPlantasActivas();
        currentFlowerData.plantaMarchita.SetActive(true);
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
        panelContenedorJuego.SetActive(false);
        if (imagenVictoria != null) imagenVictoria.SetActive(true);
        botonVolverMenu_Win.SetActive(true);
        botonDescarga.SetActive(true);
        if (imagenDerrota != null) imagenDerrota.SetActive(false);
        botonVolverMenu_Lose.SetActive(false);
        if (imagenEscarabajo != null) imagenEscarabajo.gameObject.SetActive(false);

        if (currentFlowerData == todasLasFlores[0] && !geranioCompletado)
        {
            geranioCompletado = true;
            PlayerPrefs.SetInt("GeranioCompletado", 1);
            PlayerPrefs.Save();
            Debug.Log("¡NIVEL DE GERANIO COMPLETADO! Niveles desbloqueados.");
        }
    }

    public void BotonDescarga()
    {
        Debug.Log("¡Botón de descarga presionado!");
        
        if (currentFlowerData.rewardImage == null)
        {
            Debug.LogError("¡No hay 'Reward Image' asignada para esta flor!");
            return;
        }

        try
        {
            Sprite sprite = currentFlowerData.rewardImage;
            Texture2D sourceTex = sprite.texture;
            Rect rect = sprite.textureRect;
            Texture2D newTex = new Texture2D((int)rect.width, (int)rect.height);
            Color[] pixels = sourceTex.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            newTex.SetPixels(pixels);
            newTex.Apply();
            byte[] bytes = newTex.EncodeToPNG();
            Destroy(newTex);
            string picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string fileName = currentFlowerData.flowerName + "_Recompensa.png";
            string fullPath = Path.Combine(picturesPath, fileName);
            File.WriteAllBytes(fullPath, bytes);
            Debug.Log("¡Imagen guardada! Ruta: " + fullPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error al guardar la imagen: " + e.Message);
            Debug.LogError("¡ASEGÚRATE DE QUE LA IMAGEN TENGA 'READ/WRITE ENABLED' ACTIVADO EN EL INSPECTOR!");
        }
    }

    #endregion
}
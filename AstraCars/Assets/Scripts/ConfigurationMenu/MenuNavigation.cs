using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    // Botones del menú de configuración
    [SerializeField] private Button[] modeButtons;
    [SerializeField] private Button[] difficultyButtons;
    [SerializeField] private Button[] playersButton;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button middleButton; // Nuevo botón intermedio

    // Sección actual del menú en la que se encuentra el usuario
    private enum Section { Mode, Difficulty, AcceptCancel, Players }
    private Section currentSection = Section.Players;

    // Índices de selección para cada grupo de botones
    private int modeIndex = 0;
    private int difficultyIndex = 0;
    private int playersIndex = 0;
    private int acceptCancelIndex = 0; // 0 = Cancel, 1 = Intermedio, 2 = Accept

    // Dificultades disponibles
    private readonly string[] difficulties = { "Facil", "Normal", "Dificil", "Personalizada" };

    // Variables para gestionar el tiempo entre pulsaciones
    private float inputCooldown = 0.3f;
    private float lastInputTime = 0f;

    void Start()
    {
        // Selecciona el modo y dificultad iniciales
        modeButtons[modeIndex].onClick.Invoke();
        difficultyButtons[difficultyIndex].onClick.Invoke();
        playersButton[playersIndex].onClick.Invoke();
        HighlightAll();
    }

    void Update()
    {
        // Cooldown para evitar desplazamiento rápido (Dance Pad)
        if (Time.unscaledTime - lastInputTime < inputCooldown)
            return;

        bool inputProcessed = false;

        switch (currentSection)
        {
            case Section.Players:
                if (InputManager.GetMenuRight())
                {
                    playersIndex = Mathf.Min(playersIndex + 1, playersButton.Length - 1);
                    UpdatePlayersPrefs();
                    inputProcessed = true;
                }
                else if (InputManager.GetMenuLeft())
                {
                    playersIndex = Mathf.Max(playersIndex - 1, 0);
                    UpdatePlayersPrefs();
                    inputProcessed = true;
                }
                else if (InputManager.GetMenuDown())
                {
                    UpdatePlayersPrefs();
                    currentSection = Section.Mode;
                    inputProcessed = true;
                }
                HighlightAll();
                break;

            case Section.Mode:
                if (InputManager.GetMenuRight())
                {
                    modeIndex = Mathf.Min(modeIndex + 1, modeButtons.Length - 1);
                    UpdateModePrefs();
                    inputProcessed = true;
                }
                else if (InputManager.GetMenuLeft())
                {
                    modeIndex = Mathf.Max(modeIndex - 1, 0);
                    UpdateModePrefs();
                    inputProcessed = true;
                }
                else if (InputManager.GetMenuDown())
                {
                    currentSection = Section.Difficulty;
                    inputProcessed = true;
                }
                else if (InputManager.GetMenuUp())
                {
                    currentSection = Section.Players;
                    inputProcessed = true;
                }
                HighlightAll();
                break;

            case Section.Difficulty:
                if (InputManager.GetMenuRight())
                {
                    difficultyIndex = Mathf.Min(difficultyIndex + 1, difficultyButtons.Length - 1);
                    UpdateDifficultyPrefs();
                    inputProcessed = true;
                }
                else if (InputManager.GetMenuLeft())
                {
                    difficultyIndex = Mathf.Max(difficultyIndex - 1, 0);
                    UpdateDifficultyPrefs();
                    inputProcessed = true;
                }
                else if (InputManager.GetMenuUp())
                {
                    currentSection = Section.Mode;
                    inputProcessed = true;
                }
                else if (InputManager.GetMenuDown())
                {
                    currentSection = Section.AcceptCancel;
                    acceptCancelIndex = 1; // Selecciona el botón intermedio al entrar
                    inputProcessed = true;
                }
                HighlightAll();
                break;

            case Section.AcceptCancel:
                if (InputManager.GetMenuRight())
                {
                    if (acceptCancelIndex == 1)
                    {
                        acceptCancelIndex = 2; // Aceptar
                        HighlightAll();
                        OnAcceptButtonClick();
                        inputProcessed = true;
                    }
                }
                else if (InputManager.GetMenuLeft())
                {
                    if (acceptCancelIndex == 1)
                    {
                        acceptCancelIndex = 0; // Cancelar
                        HighlightAll();
                        CancelConfig();
                        inputProcessed = true;
                    }
                }
                else if (InputManager.GetMenuUp())
                {
                    currentSection = Section.Difficulty;
                    inputProcessed = true;
                }
                HighlightAll();
                break;
        }

        if (inputProcessed)
            lastInputTime = Time.unscaledTime;
    }

    // Resalta los botones actualmente seleccionados en cada sección
    void HighlightAll()
    {
        // Mode buttons
        for (int i = 0; i < modeButtons.Length; i++)
        {
            if (currentSection == Section.Mode && i == modeIndex)
                SetButtonColor(modeButtons[i], Color.green); // Seleccionado actualmente
            else if (i == modeIndex)
                SetButtonColor(modeButtons[i], Color.yellow); // Activo general
            else
                SetButtonColor(modeButtons[i], Color.white);
        }

        // Difficulty buttons
        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            if (currentSection == Section.Difficulty && i == difficultyIndex)
                SetButtonColor(difficultyButtons[i], Color.green);
            else if (i == difficultyIndex)
                SetButtonColor(difficultyButtons[i], Color.yellow);
            else
                SetButtonColor(difficultyButtons[i], Color.white);
        }

        // Players buttons
        for (int i = 0; i < playersButton.Length; i++)
        {
            if (currentSection == Section.Players && i == playersIndex)
                SetButtonColor(playersButton[i], Color.green);
            else if (i == playersIndex)
                SetButtonColor(playersButton[i], Color.yellow);
            else
                SetButtonColor(playersButton[i], Color.white);
        }
    }

    // Cambia el color del botón según el estado de selección
    void SetButtonColor(Button btn, Color color)
    {
        var colors = btn.colors;
        colors.normalColor = color;
        btn.colors = colors;
    }

    // Métodos para sincronizar PlayerPrefs y los índices
    void UpdateModePrefs()
    {
        PlayerPrefs.SetString("GameMode", modeIndex == 0 ? "ReachEndMode" : "CollectMode");
    }

    void UpdateDifficultyPrefs()
    {
        PlayerPrefs.SetString("Difficulty", difficulties[difficultyIndex]);
    }

    void UpdatePlayersPrefs()
    {
        PlayerPrefs.SetInt("DosJugadores", playersIndex);
    }

    // Métodos públicos para los botones, asignados en el inspector
    public void OnModeButtonClick(int idx)
    {
        modeIndex = idx;
        UpdateModePrefs();
        HighlightAll();
    }

    public void OnDifficultyButtonClick(int idx)
    {
        difficultyIndex = idx;
        UpdateDifficultyPrefs();
        HighlightAll();
    }

    public void OnPlayersButtonClick(int idx)
    {
        playersIndex = idx;
        UpdatePlayersPrefs();
        HighlightAll();
    }

    public void OnAcceptButtonClick()
    {
        // Siempre guardamos los valores actuales antes de cambiar de escena para evitar problemas de sincronización
        PlayerPrefs.SetInt("DosJugadores", playersIndex);
        PlayerPrefs.SetString("GameMode", modeIndex == 0 ? "ReachEndMode" : "CollectMode");
        PlayerPrefs.SetString("Difficulty", difficulties[difficultyIndex]);

        string difficulty = difficulties[difficultyIndex];
        if (difficulty != "Personalizada")
            SceneManager.LoadScene("PlayScene");
        else
            SceneManager.LoadScene("ConfiguracionPersonalizada");
    }

    public void CancelConfig()
    {
        Debug.Log("Configuración cancelada");
        SceneManager.LoadScene("MenuPrincipal");
    }
}
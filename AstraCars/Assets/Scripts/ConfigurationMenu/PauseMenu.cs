using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    private bool isPaused = false;
    private float inputCooldown = 0.2f;
    private float lastInputTime = 0f;

    void Update()
    {
        // No permitir pausar si el juego no ha comenzado
        if (GameManager.Instance != null && GameManager.Instance.waitingForStart)
            return;

        if (Time.unscaledTime - lastInputTime < inputCooldown)
            return;

        bool inputProcessed = false;

        // Pausar/despausar 
        if (InputManager.GetPause())
        {
            if (isPaused) Resume();
            else Pause();
            inputProcessed = true;
        }

        if (isPaused)
        {
            // Reiniciar
            if (InputManager.GetRestart())
            {
                Restart();
                inputProcessed = true;
            }

            // Salir
            if (InputManager.GetExit())
            {
                ExitToMenu();
                inputProcessed = true;
            }
        }

        if (inputProcessed)
            lastInputTime = Time.unscaledTime;
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);

        // Destrimos el GameManager antes de recargar la escena
        var gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
            Destroy(gm.gameObject);

        SceneManager.LoadScene("PlayScene");
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        var gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
            Destroy(gm.gameObject);
        SceneManager.LoadScene("Configuracion1");
    }
}
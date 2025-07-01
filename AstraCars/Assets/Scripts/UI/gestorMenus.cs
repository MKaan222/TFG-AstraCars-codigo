using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gestorMenus : MonoBehaviour
{

    public string anterior, posterior;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton9) || InputManager.GetMenuRight() ||
        InputManager.GetMenuUp())
        {
            cargaEscena(posterior);
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton8) || InputManager.GetMenuLeft() ||
        InputManager.GetMenuDown())
        {
            ExitGame();
        }
    }
    public void cargaEscena(string nivel)
    {
        SceneManager.LoadScene(nivel);
    }

    void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

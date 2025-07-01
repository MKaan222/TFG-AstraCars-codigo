using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CustomConfigNavigation : MonoBehaviour
{

    // Sliders actualmente activos en el menú
    [SerializeField] private Slider[] sliders;


    [SerializeField] private Button cancelButton;

    [SerializeField] private Button middleButton;

    [SerializeField] private Button acceptButton;

    private int selectedIndex = 0; // Índice del slider o botón seleccionado
    private bool onButtons = false; // ¿Está en la zona de botones aceptar/cancelar/intermedio?
    private int buttonIndex = 1;    // 0 = Cancelar, 1 = Intermedio, 2 = Aceptar


    [SerializeField] private Slider[] commonSliders;

    [SerializeField] private Slider[] reachEndSliders;

    [SerializeField] private Slider[] collectSliders;

    //Valores para la selección continua del slider
    private float holdDelay = 0.4f;
    private float holdRepeat = 0.08f;
    private float holdTimer = 0f;
    private bool holdingRight = false;
    private bool holdingLeft = false;

    // Cooldown para navegación entre sliders y botones (Dance Pad)
    private float inputCooldown = 0.2f;
    private float lastInputTime = 0f;
    private float sliderInputCooldown = 0.15f;
    private float lastSliderInputTime = 0f;
    void Start()
    {
        UpdateSlidersArray();
        HighlightAll();
    }

    void Update()
    {
        // Cooldown para evitar desplazamiento rápido (Dance Pad/ joystick)
        if (Time.unscaledTime - lastInputTime < inputCooldown)
            return;

        bool inputProcessed = false;

        if (!onButtons)
        {
            // Navegación entre sliders
            if (InputManager.GetMenuDown())
            {
                selectedIndex++;
                if (selectedIndex >= sliders.Length)
                {
                    onButtons = true;
                    buttonIndex = 1; // Botón intermedio
                }
                HighlightAll();
                inputProcessed = true;
            }
            else if (InputManager.GetMenuUp())
            {
                selectedIndex = Mathf.Max(selectedIndex - 1, 0);
                HighlightAll();
                inputProcessed = true;
            }
            // Cambio de valor de slider con cooldown
            else if (InputManager.GetMenuRight() && Time.unscaledTime - lastSliderInputTime > sliderInputCooldown)
            {
                sliders[selectedIndex].value = Mathf.Min(sliders[selectedIndex].value + 1, sliders[selectedIndex].maxValue);
                SendMessage("OnSliderChanged", SendMessageOptions.DontRequireReceiver);
                HighlightAll();
                holdingRight = true;
                holdTimer = Time.unscaledTime + holdDelay;
                lastSliderInputTime = Time.unscaledTime;
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
            {
                holdingRight = false;
                lastSliderInputTime = 0f;
            }
            else if (holdingRight && (InputManager.GetMenuRight() || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                     && Time.unscaledTime > holdTimer && Time.unscaledTime - lastSliderInputTime > sliderInputCooldown)
            {
                sliders[selectedIndex].value = Mathf.Min(sliders[selectedIndex].value + 1, sliders[selectedIndex].maxValue);
                SendMessage("OnSliderChanged", SendMessageOptions.DontRequireReceiver);
                HighlightAll();
                holdTimer = Time.unscaledTime + holdRepeat;
                lastSliderInputTime = Time.unscaledTime;
            }
            else if (InputManager.GetMenuLeft() && Time.unscaledTime - lastSliderInputTime > sliderInputCooldown)
            {
                sliders[selectedIndex].value = Mathf.Max(sliders[selectedIndex].value - 1, sliders[selectedIndex].minValue);
                SendMessage("OnSliderChanged", SendMessageOptions.DontRequireReceiver);
                HighlightAll();
                holdingLeft = true;
                holdTimer = Time.unscaledTime + holdDelay;
                lastSliderInputTime = Time.unscaledTime;
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
            {
                holdingLeft = false;
                lastSliderInputTime = 0f;
            }
            else if (holdingLeft && (InputManager.GetMenuLeft() || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                     && Time.unscaledTime > holdTimer && Time.unscaledTime - lastSliderInputTime > sliderInputCooldown)
            {
                sliders[selectedIndex].value = Mathf.Max(sliders[selectedIndex].value - 1, sliders[selectedIndex].minValue);
                SendMessage("OnSliderChanged", SendMessageOptions.DontRequireReceiver);
                HighlightAll();
                holdTimer = Time.unscaledTime + holdRepeat;
                lastSliderInputTime = Time.unscaledTime;
            }
        }
        else // En la zona de botones
        {
            if (InputManager.GetMenuRight())
            {
                if (buttonIndex == 1)
                {
                    buttonIndex = 2; // Aceptar
                    HighlightAll();
                    AcceptConfig();
                    inputProcessed = true;
                }
            }
            else if (InputManager.GetMenuLeft())
            {
                if (buttonIndex == 1)
                {
                    buttonIndex = 0; // Cancelar
                    HighlightAll();
                    CancelConfig();
                    inputProcessed = true;
                }
            }
            else if (InputManager.GetMenuUp())
            {
                onButtons = false;
                selectedIndex = sliders.Length - 1;
                HighlightAll();
                inputProcessed = true;
            }
        }

        if (inputProcessed)
            lastInputTime = Time.unscaledTime;
    }

    void HighlightAll()
    {
        // Resalta el slider seleccionado
        for (int i = 0; i < sliders.Length; i++)
        {
            var color = sliders[i].targetGraphic.color;
            sliders[i].targetGraphic.color = (!onButtons && i == selectedIndex) ? Color.yellow : Color.white;
        }
        // Resalta los botones
        SetButtonColor(cancelButton, onButtons && buttonIndex == 0);
        SetButtonColor(middleButton, onButtons && buttonIndex == 1);
        SetButtonColor(acceptButton, onButtons && buttonIndex == 2);
    }

    // Cambia el color del botón según si está seleccionado
    void SetButtonColor(Button btn, bool selected)
    {
        var colors = btn.colors;
        colors.normalColor = selected ? Color.yellow : Color.white;
        btn.colors = colors;
    }


    // Acciones para los botones de aceptar y cancelar
    void AcceptConfig()
    {
        FindFirstObjectByType<CustomConfigMenuManager>().OnAccept();
    }

    void CancelConfig()
    {
        SceneManager.LoadScene("Configuracion1");
    }
    public void OnAcceptButtonClick()
    {
        FindFirstObjectByType<CustomConfigMenuManager>().OnAccept();
    }

    public void OnCancelButtonClick()
    {
        SceneManager.LoadScene("Configuracion1");
    }


    // Actualiza el array de sliders según el modo de juego seleccionado
    void UpdateSlidersArray()
    {
        List<Slider> sliderList = new List<Slider>();

        // Añade los sliders comunes
        sliderList.AddRange(commonSliders);

        // Añade los específicos según el modo
        string mode = PlayerPrefs.GetString("GameMode", "ReachEndMode");
        if (mode == "ReachEndMode")
            sliderList.AddRange(reachEndSliders);
        else
            sliderList.AddRange(collectSliders);

        sliders = sliderList.ToArray();
        selectedIndex = 0;
        onButtons = false;
        HighlightAll();
    }
}
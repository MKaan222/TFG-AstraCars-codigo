using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomConfigMenuManager : MonoBehaviour
{
    public Slider maxSpeedSlider;
    public Text maxSpeedValueText;

    public Slider nPiecesSlider;
    public Text nPiecesValueText;

    public Slider gasolinaSlider;
    public Text gasolinaValueText;

    public Slider enemySpawnSlider;
    public Text enemySpawnValueText;

    public Slider nObstaclesSlider;
    public Text nObstaclesValueText;

    public Slider penalizationSlider;
    public Text penalizationValueText;

    public Slider pushBackSlider;
    public Text pushBackValueText;

    public Slider velocidadGiroSlider;
    public Text velocidadGiroValueText;


    public GameObject reachEndSliders;
    public Slider timeSlider;
    public Text timeValueText;
    public GameObject collectSliders;
    public Slider totalCollectiblesSlider;
    public Text totalCollectiblesValueText;
    public Slider collectiblesMultiplierSlider;
    public Text collectiblesMultiplierValueText;




    void Start()
    {

        string mode = PlayerPrefs.GetString("GameMode", "ReachEndMode");
        reachEndSliders.SetActive(mode == "ReachEndMode");
        collectSliders.SetActive(mode == "CollectMode");
        // Inicializar los textos con el valor inicial de cada slider
        UpdateTexts();
    }

    void UpdateTexts()
    {
        maxSpeedValueText.text = maxSpeedSlider.value.ToString("F0");
        gasolinaValueText.text = gasolinaSlider.value.ToString("F0");
        enemySpawnValueText.text = enemySpawnSlider.value.ToString("F0");
        timeValueText.text = timeSlider.value.ToString("F0");
        nPiecesValueText.text = nPiecesSlider.value.ToString("F0");
        nObstaclesValueText.text = nObstaclesSlider.value.ToString("F0");
        penalizationValueText.text = penalizationSlider.value.ToString("F0");
        pushBackValueText.text = pushBackSlider.value.ToString("F0");
        velocidadGiroValueText.text = velocidadGiroSlider.value.ToString("F0");
        totalCollectiblesValueText.text = totalCollectiblesSlider.value.ToString("F0");
        collectiblesMultiplierValueText.text = collectiblesMultiplierSlider.value.ToString("F0");


    }

    public void OnSliderChanged()
    {
        UpdateTexts();
    }

    public void OnAccept()
    {
        // Guarda los valores en PlayerPrefs
        PlayerPrefs.SetFloat("CustomMaxSpeed", maxSpeedSlider.value);
        PlayerPrefs.SetFloat("CustomGasolina", gasolinaSlider.value);
        PlayerPrefs.SetInt("CustomEnemySpawn", (int)enemySpawnSlider.value);
        PlayerPrefs.SetFloat("CustomTime", timeSlider.value);
        PlayerPrefs.SetInt("CustomNPieces", (int)nPiecesSlider.value);
        PlayerPrefs.SetInt("CustomNObstacles", (int)nObstaclesSlider.value);
        PlayerPrefs.SetFloat("CustomPenalization", penalizationSlider.value);
        PlayerPrefs.SetFloat("CustomPushBack", pushBackSlider.value);
        PlayerPrefs.SetInt("CustomTotalCollectibles", (int)totalCollectiblesSlider.value);
        PlayerPrefs.SetInt("CustomCollectiblesMultiplier", (int)collectiblesMultiplierSlider.value);
        PlayerPrefs.SetFloat("CustomVelocidadGiro", velocidadGiroSlider.value);

        // Marca que se ha elegido personalizada
        PlayerPrefs.SetString("Difficulty", "Personalizada");

        // Carga la escena de juego
        SceneManager.LoadScene("PlayScene");
    }
}

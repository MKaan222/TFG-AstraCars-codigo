using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{

    public PlayerData playerData;
    public Text scoreText;
   
    // Update is called once per frame
    void Update()
    {
        if (playerData != null)
        {
            scoreText.text = "Puntuación: " + playerData.score;
        }
        
        
    }
}

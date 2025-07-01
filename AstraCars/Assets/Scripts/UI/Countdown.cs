using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{

    public float countdownTime = 10f;
    public Text countdownText;
    private bool running = false;



    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            countdownTime -= Time.deltaTime;
            if (countdownTime <= 0)
            {
                countdownTime = 0;
                running = false;
                countdownText.color = Color.red;
            }
            int minutes = Mathf.FloorToInt(countdownTime / 60);
            int seconds = Mathf.FloorToInt(countdownTime % 60);
            countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }




    public void StartCountdown(float time)
    {
        countdownTime = time;
        running = true;
        countdownText.color = Color.white;
    }

    public float GetTimeRemaining()
    {
        return countdownTime;
    }

}

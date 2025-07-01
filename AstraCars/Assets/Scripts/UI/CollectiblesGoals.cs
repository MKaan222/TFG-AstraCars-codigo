using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectiblesGoals : MonoBehaviour
{
    public Text goalText;

    public void UpdateGoalText(int remaining)
    {
        goalText.text = "Objetivo: " + remaining;
    }
}

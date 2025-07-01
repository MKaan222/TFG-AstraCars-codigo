using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReachEndMode : IGameMode
{
    private bool finished;
    private PlayerData winner = null;


    public void Init(GameManager gameManager)
    {
        finished = false;

    }
    public void OnPlayerReachEnd(PlayerData playerData)
    {

        finished = true;
        float timeLeft = GameManager.Instance.GetCountdownTime();
        if (winner == null)
        {
            winner = playerData;
        }
        if (timeLeft > 0)
        {
            GameManager.Instance.OnVictory(winner);
        }


    }

    public bool IsGameOver()
    {
        if (finished)
            return true;
        if (GameManager.Instance.GetCountdownTime() <= 0)
        {
            return true;
        }
        if (GameManager.Instance.players.All(p => p.GetGasolina()  <= 0))
            return true;
        return false;
    }

}

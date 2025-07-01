using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectMode : IGameMode
{
    private bool finished;
    //private Dictionary<PlayerData, int> playerCollectibles = new Dictionary<PlayerData, int>();

    public int collectiblesToWin = 10;
    public float collectibleMultiplier = 3f;
    public int collectiblesPlayer1 = 5;
    public int collectiblesPlayer2 = 5;
    private int playersAtEnd;

    public void Init(GameManager gameManager)
    {
        finished = false;
        playersAtEnd = 0;
        foreach (var player in gameManager.players)
        {
            player.score = 0;
        }
    }
    public void OnPlayerReachEnd(PlayerData playerData)
    {
        playersAtEnd++;
       // Debug.Log(playersAtEnd + " jugadores han llegado al final.");
        int numPlayers = GameManager.Instance.players.Count;
        if (numPlayers == 1)
        {
            // Un solo jugador: comprobar victoria al llegar
            CheckVictory(playerData);
        }
        else if (playersAtEnd >= numPlayers)
        {
            // Dos jugadores: comprobar victoria solo cuando ambos han llegado
            // Puedes elegir qué jugador mostrar en el mensaje, aquí se usa el último
            CheckVictory(playerData);
        }


    }
    
    public bool IsGameOver()
    {
        if(finished)   
            return true;
        if (GameManager.Instance.players.All(p => p.GetGasolina() <= 0))
            return true;
        return false;
    }

    private void CheckVictory(PlayerData playerData)
    {
        int playersTotalCollectibles = GetTotalCollected();
        

        if (playersTotalCollectibles >= collectiblesToWin)
        {
            finished = true;
            GameManager.Instance.OnVictory(playerData);
        }
        else
        {
            finished = true;
            //GameManager.Instance.ShowGameOverScreen();
            //Debug.Log("No se han recogido suficientes coleccionables para ganar.");
        }
    }

    public int GetTotalCollected()
    {
        int total = 0;
        foreach (var player in GameManager.Instance.players)
            total += player.getScore();
        return total;
    }

}

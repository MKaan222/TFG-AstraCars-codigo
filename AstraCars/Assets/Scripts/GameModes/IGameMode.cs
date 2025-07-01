using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameMode
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Init(GameManager gameManager);
    void OnPlayerReachEnd(PlayerData playerData);
    //void OnCollectiblePicked(PlayerData playerData);
    bool IsGameOver();
    




}

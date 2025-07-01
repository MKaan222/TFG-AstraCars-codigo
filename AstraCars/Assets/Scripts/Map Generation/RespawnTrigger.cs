using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    public Transform respawnPoint1;
    public Transform respawnPoint2;
    
    public void OnTriggerEnter(Collider other)
    {
        PlayerData playerData = other.GetComponentInParent<PlayerData>();
        if (playerData != null)
        {
            if (playerData.IsPlayer1())
                playerData.SetLastRespawnPoint(respawnPoint1);
            else
                playerData.SetLastRespawnPoint(respawnPoint2);
        }

    }
}

using System.Collections.Generic;
using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    private HashSet<PlayerData> playersWhoFinished = new HashSet<PlayerData>();

    private void OnTriggerEnter(Collider other)
    {
        PlayerData playerData = other.GetComponentInParent<PlayerData>();
        if (playerData != null && !playersWhoFinished.Contains(playerData))
        {
            playersWhoFinished.Add(playerData);
            GameManager.Instance.PlayerReachedEnd(playerData);

            Rigidbody rb = other.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }
}
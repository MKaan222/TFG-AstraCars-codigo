using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreItem : MonoBehaviour
{
    public int scoreValue = 1; // The score value for this item
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerData playerData = other.GetComponentInParent<PlayerData>();
        if (playerData != null)
        {
            // Solo permite recoger si el tag coincide con el jugador
            if (!GameManager.Instance.dosJugadores ||
                (gameObject.tag == "CollectibleJ1" && playerData == GameManager.Instance.players[0]) ||
                (gameObject.tag == "CollectibleJ2" && playerData == GameManager.Instance.players[1]))
            {
                playerData.AddScore(scoreValue);
                Destroy(gameObject);
                GameManager.Instance.PlayerPickCollectible(playerData);
            }
        }
    }
}

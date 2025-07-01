using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BoostAbility : IAbility
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateAbility(PlayerData player)
    {
        // Implement the boost logic here
        Debug.Log("Boost ability activated for player: " + player.name);
        // Example: Increase player's speed temporarily
        Rigidbody rb = player.GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            rb.AddForce(player.transform.forward * 10000f, ForceMode.Impulse); // Adjust the force as needed
        }
    }
}

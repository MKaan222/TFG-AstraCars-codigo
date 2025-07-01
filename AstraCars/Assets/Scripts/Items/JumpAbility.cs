using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JumpAbility : IAbility
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
        // Implement the jump logic here
        Debug.Log("Jump ability activated for player: " + player.name);
        
        // Example: Apply a force to the player's rigidbody to make them jump
        Rigidbody rb = player.GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            rb.AddForce(Vector3.up * 15000f, ForceMode.Impulse); // Adjust the force as needed
        }
    }

}

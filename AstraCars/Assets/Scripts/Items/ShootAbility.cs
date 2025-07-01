using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Abilities/ShootAbility")]
public class ShootAbility : ScriptableObject, IAbility
{
    public GameObject missilePrefab; // Assign a bullet prefab in the inspector
                                     //public float missileSpeed = 10f; // Speed of the missile

    public void ActivateAbility(PlayerData player)
    {
        // Implement the shooting logic here
        Debug.Log("Shoot ability activated for player: " + player.name);

        Transform shootPoint = player.transform;
        Rigidbody rb = player.GetComponent<Rigidbody>();
        float playerSpeed = rb != null ? rb.velocity.magnitude : 0f;


        float baseDistance = 2f;
        float extraDistance = playerSpeed * 0.2f;
        float spawnDistance = baseDistance + extraDistance;

        Vector3 spawnPosition = shootPoint.position + shootPoint.forward * spawnDistance + Vector3.up * 1f;
        GameObject missile = GameObject.Instantiate(missilePrefab, spawnPosition, shootPoint.rotation);

        // For example, instantiate a bullet prefab and set its direction
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

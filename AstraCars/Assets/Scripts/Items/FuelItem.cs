using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelItem : MonoBehaviour
{
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
            playerData.AddGasolina(50f); 
            Destroy(gameObject);
        }
    }
}

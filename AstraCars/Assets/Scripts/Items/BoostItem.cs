using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostItem : MonoBehaviour
{
    public BoostAbility boostAbility = new BoostAbility(); // Assign the shoot ability in the inspector
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    /*
        private void OnTriggerEnter(Collider other)
        {
            PlayerData playerData = other.GetComponentInParent<PlayerData>();
            if (playerData != null && boostAbility != null)
            {
                playerData.AddAbility(boostAbility);
                Destroy(gameObject);
            }
        }*/
    
}

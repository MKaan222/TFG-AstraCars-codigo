using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItem : MonoBehaviour
{
    public ShootAbility shootAbility;
    private List<IAbility> abilities = new List<IAbility>();

    void Awake()
    {
        // Initialize the abilities list with different abilities
        abilities.Add(shootAbility); // Assuming ShootAbility is a ScriptableObject
        abilities.Add(new BoostAbility());
        abilities.Add(new JumpAbility());
    }
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
        if (playerData != null && abilities != null)
        {
            var ability = abilities[Random.Range(0, abilities.Count)] as IAbility;
            if(ability != null)
            { 
                playerData.AddAbility(ability);
            }
            Destroy(gameObject);
        }
    }*/
}

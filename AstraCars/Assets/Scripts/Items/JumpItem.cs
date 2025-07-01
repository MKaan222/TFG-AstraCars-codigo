using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpItem : MonoBehaviour
{
    public JumpAbility jumpAbility = new JumpAbility(); // Assign the shoot ability in the inspector
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
        if (playerData != null && jumpAbility != null)
        {
            playerData.AddAbility(jumpAbility);
            Destroy(gameObject);
        }
    }
    */
}

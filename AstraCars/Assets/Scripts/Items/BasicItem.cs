using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicItem : MonoBehaviour
{
    public BoostAbility shootAbility = new BoostAbility(); // Assign the shoot ability in the inspector
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /*private void OnTriggerEnter(Collider other)
    {
        PlayerData playerData = other.GetComponentInParent<PlayerData>();
        if (playerData != null && shootAbility != null)
        {
            playerData.AddAbility(shootAbility);
            Destroy(gameObject);
        }
    }*/

}

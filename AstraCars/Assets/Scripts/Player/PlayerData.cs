using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public IAbility currentAbility;
    private float gasolinaActual;
    public float gasolinaMaxima = 100;
    public int shootAmmo = 0;
    public int score = 0;
    public bool hasFinished = false;
    public Transform lastRespawnPoint;
    private bool whichPlayer;

    // Start is called before the first frame update
    void Start()
    {
        gasolinaActual = gasolinaMaxima;

    }



    /*
        public void AddAbility(IAbility ability)
        {
            // Add the ability to the player
            currentAbility = ability;
            if (ability is ShootAbility)
            {
                shootAmmo = 5;
                Debug.Log("Shoot ammo increased: " + shootAmmo);
            }
            // This could be a list of abilities or a single ability slot
            Debug.Log("Ability added: " + ability.GetType().Name);
        }

        public void UseAbility()
        {

            float checkDistance = 1f;
            bool onGround = Physics.Raycast(transform.position, Vector3.down, checkDistance);

            // Use the current ability
            if (currentAbility != null && onGround)
            {
                if (currentAbility is ShootAbility)
                {
                    if (shootAmmo <= 0)
                    {
                        Debug.Log("No ammo left for shooting!");
                        return;
                    }
                    shootAmmo--;
                    Debug.Log("Shoot ammo used: " + shootAmmo);
                }
                currentAbility.ActivateAbility(this);
            }

        }*/

    public void AddGasolina(float amount)
    {
        if (gasolinaActual + amount > gasolinaMaxima)
        {
            gasolinaActual = gasolinaMaxima;
        }
        else
        {
            gasolinaActual += amount;
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        //Debug.Log("Score updated: " + score);
    }

    public int getScore()
    {
        return score;
    }
    public float GetGasolina()
    {
        return gasolinaActual;
    }
    public void SetGasolina(float amount)
    {
        gasolinaActual = amount;
    }
    public void SetLastRespawnPoint(Transform respawnPoint)
    {
        lastRespawnPoint = respawnPoint;
    }

    public void SetPlayerType(bool isPlayer1)
    {
        whichPlayer = isPlayer1;
    }
    public bool IsPlayer1()
    {
        return whichPlayer;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public float pushForce = 10f;
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
        CarController car = other.GetComponentInParent<CarController>();
        PlayerData player = other.GetComponentInParent<PlayerData>();
        if (car != null && player != null)
        {
            Rigidbody rb = car.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 push = -car.transform.forward;
                rb.AddForce(push * pushForce, ForceMode.Impulse);
                player.AddGasolina(-5f); // Reduce gasolina when hitting an obstacle
            }
            Destroy(gameObject); // Destroy the obstacle after collision
        }
    }
    public void SetPushForce(float newPushForce)
    {
        pushForce = newPushForce;
    }
}

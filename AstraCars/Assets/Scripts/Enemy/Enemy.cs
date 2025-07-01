using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public NavMeshAgent enemyAgent;
    public List<Vector3> route;
    private int currentTarget = 0;


    // Start is called before the first frame update
    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        if (route != null && currentTarget < route.Count && enemyAgent.remainingDistance < 0.5f && !enemyAgent.pathPending)
        {
            currentTarget++;
            GoToNextPoint();
        }
    }

    public void SetRoute(List<Vector3> routePoints)
    {
        route = routePoints;
        currentTarget = 0;
        GoToNextPoint();
    }

    void GoToNextPoint()
    {
        if (route != null && currentTarget < route.Count)
        {
            enemyAgent.SetDestination(route[currentTarget]);
        }
        else
        {
            Destroy(gameObject); // Ha terminado la ruta
        }
    }


}

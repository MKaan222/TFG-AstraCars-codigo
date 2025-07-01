using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsHelper : MonoBehaviour
{

    // Clase que sirve para obtener los puntos de spawn de diferentes tipos de objetos en el mapa
    public List<Transform> GetDecorationPoints()
    {
        List<Transform> spawnPoints = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Decoration"))
            {
                spawnPoints.Add(child);
            }
        }
        return spawnPoints;
    }

    public List<Transform> GetCollectiblePoints()
    {
        List<Transform> points = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Collectible"))
            {
                points.Add(child);
            }
        }
        return points;
    }

    public List<Transform> GetObstaclePoints()
    {
        List<Transform> points = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Obstacle"))
            {
                points.Add(child);
            }
        }
        return points;
    }

    public List<Transform> GetEnemyRoutePoints()
    {
        List<Transform> points = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("EnemyTarget"))
            {
                points.Add(child);
            }
        }
        return points;
    }

}

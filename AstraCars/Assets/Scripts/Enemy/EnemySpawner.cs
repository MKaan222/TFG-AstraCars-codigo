using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    public List<Transform> targetPoints;
    private float timer = 0f;

    public MapGenerator mapGenerator;
    public Material enemyMaterial;
    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = GameManager.Instance.mapGenerator;

    }



    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }

    }

    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        // Busco los objetos que quiero que cambien de color en el prefab del enemigo
        Transform body = enemy.transform.Find("body");
        if (body != null)
        {
            Renderer bodyRenderer = body.GetComponent<Renderer>();
            Transform spoiler = body.Find("spoiler");
            Renderer spoilerRenderer = spoiler != null ? spoiler.GetComponent<Renderer>() : null;

            if (bodyRenderer != null && bodyRenderer.materials.Length > 5)
            {
                // Crea una copia del material 
                Material newMat = new Material(bodyRenderer.materials[5]);
                newMat.color = Random.ColorHSV();

                // Cambiar el material en el body
                Material[] bodyMats = bodyRenderer.materials;
                bodyMats[1] = newMat;
                bodyRenderer.materials = bodyMats;

                // Cambiar el material en spoiler 
                if (spoilerRenderer != null && spoilerRenderer.materials.Length > 0)
                {
                    Material[] spoilerMats = spoilerRenderer.materials;
                    spoilerMats[1] = newMat;
                    spoilerRenderer.materials = spoilerMats;
                }
            }
        }

        // Genera una ruta aleatoria para este enemigo
        List<Vector3> route = mapGenerator.GenerateRandomEnemyRoute(mapGenerator.spawnedPieces);

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null && route != null)
        {
            enemyScript.SetRoute(route);
        }
    }

    public void SetTargetPoints(List<Transform> newTargetPoints)
    {
        targetPoints = newTargetPoints;
    }

    public void SetSpawnInterval(float newInterval)
    {
        spawnInterval = newInterval;
    }

}

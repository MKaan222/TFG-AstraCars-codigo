using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [Header("Datos de piezas")]
    [SerializeField] private PiecesData[] piecesData;
    [SerializeField] private PiecesData[] beachPiecesData;
    [SerializeField] private PiecesData firstPiece;
    [SerializeField] private PiecesData beachFirstPiece;
    [SerializeField] private PiecesData lastPiece;
    [SerializeField] private PiecesData beachLastPiece;

    [Header("Decoraciones y obstáculos")]
    [SerializeField] private GameObject[] decorations;
    [SerializeField] private GameObject[] beachDecorations;
    [SerializeField] private GameObject[] fillPieces;
    [SerializeField] private GameObject[] beachFillPieces;
    [SerializeField] private GameObject[] obstacles;
    [SerializeField] private GameObject[] beachObstacles;

    [Header("Coleccionables")]
    [SerializeField] private GameObject collectiblePrefab;

    [Header("Configuración de generación")]
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private Vector3 spawnOrigin;
    public int maxPieces = 10;
    private PiecesData previousPiece;
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
    public List<GameObject> spawnedPieces = new List<GameObject>();
    private int nObstacles = 1;
    private int whichTheme = 0;




    // Start is called before the first frame update
    void Start()
    {
        whichTheme = Random.Range(0, 2);

        if (whichTheme == 1)
        {
            piecesData = beachPiecesData;
            decorations = beachDecorations;
            fillPieces = beachFillPieces;
            obstacles = beachObstacles;
            firstPiece = beachFirstPiece;
            lastPiece = beachLastPiece;
        }

        nObstacles = GameManager.Instance.obstacles;
    }


    // Metodo para iniciar la generación del mapa. 
    // Primero generamos la pieza de inicio (especial), piezas intermedias y la pieza final (especial)
    public void SpawnPieces()
    {

        SpawnFirstPiece();
        for (int i = 1; i < maxPieces - 1; i++)
        {
            SpawnPiece();
        }
        SpawnLastPiece();

        DistributeCollectiblesAndObstacles(spawnedPieces, collectiblePrefab, obstacles);
    }

    void SpawnFirstPiece()
    {

        GameObject piece = firstPiece.levelPieces[Random.Range(0, firstPiece.levelPieces.Length)];
        previousPiece = firstPiece;
        Vector3 pieceWorldPostion = spawnPosition + spawnOrigin;
        GameObject pieceInstance = Instantiate(piece, spawnPosition + spawnOrigin, Quaternion.identity);
        occupiedPositions.Add(pieceWorldPostion);
        spawnedPieces.Add(pieceInstance);

    }

    void SpawnLastPiece()
    {

        // Calcula la posición de la pieza final según la salida de la última pieza
        Vector3 lastPiecePosition = spawnPosition;
        Quaternion rotation = Quaternion.identity;


        // La entrada de la pieza final debe coincidir con previousPiece.exit
        // Basicamente consiste en que, teniendo solo una pieza de final, siempre pueda ponerse a continuacion de la ultima pieza generada
        switch (previousPiece.exit)
        {
            case PiecesData.Direction.North:
                lastPiecePosition += new Vector3(0, 0, previousPiece.piezeSize.y);
                rotation = Quaternion.Euler(0, 0, 0); // Entrada Sur debe mirar hacia el Sur
                break;
            case PiecesData.Direction.South:
                lastPiecePosition += new Vector3(0, 0, -previousPiece.piezeSize.y);
                rotation = Quaternion.Euler(0, 0, 0); // Entrada Sur ya está hacia el Sur
                break;
            case PiecesData.Direction.East:
                lastPiecePosition += new Vector3(previousPiece.piezeSize.x, 0, 0);
                rotation = Quaternion.Euler(0, 90, 0); // Entrada Sur debe mirar hacia el Este
                break;
            case PiecesData.Direction.West:
                lastPiecePosition += new Vector3(-previousPiece.piezeSize.x, 0, 0);
                rotation = Quaternion.Euler(0, -90, 0); // Entrada Sur debe mirar hacia el Oeste
                break;
        }

        GameObject piece = lastPiece.levelPieces[Random.Range(0, lastPiece.levelPieces.Length)];
        GameObject pieceInstance = Instantiate(piece, lastPiecePosition + spawnOrigin, rotation);
        occupiedPositions.Add(lastPiecePosition + spawnOrigin);

        EnemySpawner enemySpawner = pieceInstance.GetComponentInChildren<EnemySpawner>();
        enemySpawner.SetSpawnInterval(GameManager.Instance.enemySpawnTime);
        spawnedPieces.Add(pieceInstance);

    }

    void SpawnPiece()
    {
        PiecesData pieceToSpawn = GetNextPiece();
        GameObject piece = pieceToSpawn.levelPieces[Random.Range(0, pieceToSpawn.levelPieces.Length)];
        previousPiece = pieceToSpawn;
        Vector3 pieceWorldPostion = spawnPosition + spawnOrigin;
        GameObject pieceInstance = Instantiate(piece, spawnPosition + spawnOrigin, piece.transform.rotation);

        occupiedPositions.Add(pieceWorldPostion);
        spawnedPieces.Add(pieceInstance);


        // Llamamos a SpawnPointsHelper para obtener los puntos de spawn relevantes
        SpawnPointsHelper spawnPointsHelper = pieceInstance.GetComponent<SpawnPointsHelper>();

        if (spawnPointsHelper != null)
        {
            if (spawnPointsHelper.GetDecorationPoints() != null)
            {
                foreach (Transform spawnPoint in spawnPointsHelper.GetDecorationPoints())
                {
                    bool placed = false;
                    int maxAttempts = 10;
                    int attempts = 0;

                    while (!placed && attempts < maxAttempts)
                    {
                        GameObject decorationPrefab = decorations[Random.Range(0, decorations.Length)];
                        // Instancia temporalmente para obtener el tamaño del collider
                        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                        GameObject temp = Instantiate(decorationPrefab, spawnPoint.position, randomRotation);
                        Collider col = temp.GetComponent<Collider>();
                        if (col != null)
                        {
                            // Comprueba solapamiento
                            Collider[] overlaps = Physics.OverlapBox(col.bounds.center, col.bounds.extents, temp.transform.rotation, ~0, QueryTriggerInteraction.Ignore);
                            bool hasOverlap = false;
                            foreach (var overlap in overlaps)
                            {
                                if (overlap.gameObject != temp) // Ignora el propio collider
                                {
                                    hasOverlap = true;
                                    break;
                                }
                            }
                            if (!hasOverlap)
                            {
                                // Todas las decoraciones tienen el pivote en la base, asi que con esta config de altura aseguro esten encima del suelo
                                float alturaDeseada = 6.01f;
                                if (col != null)
                                {
                                    float baseY = col.bounds.center.y - col.bounds.extents.y;
                                    Vector3 pos = temp.transform.position;
                                    pos.y += alturaDeseada - baseY;
                                    temp.transform.position = pos;
                                }
                                else
                                {
                                    // Si no hay collider, usa el spawnPoint tal cual
                                    Vector3 pos = temp.transform.position;
                                    pos.y = alturaDeseada;
                                    temp.transform.position = pos;
                                }
                            }
                            else
                            {
                                Destroy(temp); // Elimina la instancia temporal
                            }
                        }
                        else
                        {
                            Destroy(temp); // Sin collider, no se puede comprobar
                        }
                        attempts++;
                    }
                }
            }

        }
    }

    // Devuelve una pieza aleatoria que se pueda colocar a continuacion de la ultima pieza generada con el objetivo de que
    // el nivel general avance siempre hacia el norte (delante)
    PiecesData GetNextPiece()
    {
        List<PiecesData> allowedPieces = new List<PiecesData>();
        PiecesData.Direction nextDirection = PiecesData.Direction.North;


        switch (previousPiece.exit)
        {
            case PiecesData.Direction.North:
                nextDirection = PiecesData.Direction.South;
                spawnPosition = spawnPosition + new Vector3(0, 0, previousPiece.piezeSize.y);
                break;
            case PiecesData.Direction.South:
                nextDirection = PiecesData.Direction.North;
                spawnPosition = spawnPosition + new Vector3(0, 0, -previousPiece.piezeSize.y);
                break;
            case PiecesData.Direction.East:
                nextDirection = PiecesData.Direction.West;
                spawnPosition = spawnPosition + new Vector3(previousPiece.piezeSize.x, 0, 0);
                break;
            case PiecesData.Direction.West:
                nextDirection = PiecesData.Direction.East;
                spawnPosition = spawnPosition + new Vector3(-previousPiece.piezeSize.x, 0, 0);
                break;
            default:
                break;
        }

        foreach (PiecesData pieceData in piecesData)
        {
            if (pieceData.entry == nextDirection)
            {
                allowedPieces.Add(pieceData);
            }
        }
        return allowedPieces[Random.Range(0, allowedPieces.Count)];

    }



    public void FillRoadMap()
    {
        HashSet<Vector3> fillPositions = new HashSet<Vector3>();
        int[] rotationAngles = { 0, 90, 180, 360 };
        float offsetX = 48f;

        // Offsets para laterales y esquinas
        Vector3[] offsets = new Vector3[]
        {
        new Vector3(-offsetX, 0, 0),   // izquierda
        new Vector3(offsetX, 0, 0),    // derecha
        new Vector3(-offsetX, 0, offsetX),   // esquina sup. izq
        new Vector3(offsetX, 0, offsetX),    // esquina sup. der
        new Vector3(-offsetX, 0, -offsetX),  // esquina inf. izq
        new Vector3(offsetX, 0, -offsetX)    // esquina inf. der
        };

        foreach (Vector3 position in occupiedPositions)
        {
            foreach (var offset in offsets)
            {
                Vector3 targetPos = position + offset;
                if (!occupiedPositions.Contains(targetPos) && !fillPositions.Contains(targetPos))
                {
                    GameObject fillPrefab = fillPieces[Random.Range(0, fillPieces.Length)];
                    int randomAngle = rotationAngles[Random.Range(0, rotationAngles.Length)];
                    Quaternion rotation = Quaternion.Euler(0, randomAngle, 0);
                    Instantiate(fillPrefab, targetPos, rotation);
                    fillPositions.Add(targetPos);
                }
            }
        }
    }



    // Genera un punto aleatorio dentro del cuadrado definido por 4 puntos
    Vector3 RandomPointInQuad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = Random.value;
        float v = Random.value;
        Vector3 a = Vector3.Lerp(p0, p1, u);
        Vector3 b = Vector3.Lerp(p3, p2, u);
        return Vector3.Lerp(a, b, v);
    }

    // Genera una ruta aleatoria para los enemigos a partir de los puntos de spawn de cada pieza del mapa. 
    // Cada pieza cuenta con dos puntos para las rutas y se selecciona un punto aleatorio de entre la distancia que hay entre ambos puntos
    public List<Vector3> GenerateRandomEnemyRoute(List<GameObject> roadPieces)
    {
        List<Vector3> route = new List<Vector3>();
        foreach (GameObject piece in roadPieces)
        {
            var helper = piece.GetComponent<SpawnPointsHelper>();
            if (helper == null) continue;
            var points = helper.GetEnemyRoutePoints();
            if (points.Count >= 2)
            {
                float t = Random.Range(0f, 1f);
                Vector3 randomPoint = Vector3.Lerp(points[0].position, points[1].position, t);
                route.Add(randomPoint);
            }
        }
        route.Reverse();
        return route;
    }


    // Metodo unificado para distribuir coleccionables y obstaculos en las piezas del mapa
    public void DistributeCollectiblesAndObstacles(List<GameObject> roadPieces, GameObject collectiblePrefab, GameObject[] obstaclePrefabs)
    {
        // Los coleccionables solo deben de aparecer en el Collect Mode
        var collectMode = GameManager.Instance.gameMode as CollectMode;
        bool isCollectMode = collectMode != null;

        int collectiblesToWin = isCollectMode ? collectMode.collectiblesToWin : 0;
        // Siempre creamos mas coleccionables de los necesarios para evitar coleccionables perdibles criticos
        float collectibleMultiplier = isCollectMode ? collectMode.collectibleMultiplier : 0f;
        int totalCollectibles = isCollectMode ? Mathf.CeilToInt(collectiblesToWin * collectibleMultiplier) : 0;

        int piecesCount = roadPieces.Count;
        float minDistance = 2.0f;

        // Distribuimos los coleccionables entre las piezas
        List<int> pieceIndices = new List<int>();
        for (int i = 0; i < piecesCount; i++) pieceIndices.Add(i);

        List<int> assignedPieces = new List<int>(); // Contiene, paracada coleccionable, el indice de la pieza donde se asigna
        if (isCollectMode)
        {
            if (totalCollectibles <= piecesCount)
            {
                for (int c = 0; c < totalCollectibles; c++)
                {
                    int idx = Random.Range(0, pieceIndices.Count);
                    assignedPieces.Add(pieceIndices[idx]);
                    pieceIndices.RemoveAt(idx);
                }
            }
            else
            {
                int baseCount = totalCollectibles / piecesCount;
                int remainder = totalCollectibles % piecesCount;
                for (int i = 0; i < piecesCount; i++)
                    for (int j = 0; j < baseCount; j++)
                        assignedPieces.Add(i);
                for (int r = 0; r < remainder; r++)
                {
                    int idx = Random.Range(0, piecesCount);
                    assignedPieces.Add(idx);
                }
            }
        }

        // Lista para almacenar posiciones ya usadas para evitar solapamientos
        List<Vector3> usedPositions = new List<Vector3>();

        // Repartimos el numero de coleccionables entre los jugadores
        int currentJ1 = isCollectMode ? totalCollectibles / 2 : 0;
        int currentJ2 = isCollectMode ? totalCollectibles - currentJ1 : 0;

        // Instanciar los recolectables y obstáculos por pieza 
        for (int i = 0; i < piecesCount; i++)
        {
            GameObject piece = roadPieces[i];
            var helper = piece.GetComponent<SpawnPointsHelper>();
            if (helper == null) continue;

            // Añadimos los coleccionables si es el modo correcto
            if (isCollectMode)
            {
                // Buscamos todos los coleccionables que deben ir en esta pieza
                int collectiblesInPiece = assignedPieces.FindAll(idx => idx == i).Count;
                if (collectiblesInPiece <= 0) continue; // Si no hay coleccionables, saltamos a la siguiente pieza
                // Obtenemos los cuatro puntos que corresponden al area de spawn de coleccionables
                var collectiblePoints = helper.GetCollectiblePoints();
                // Si hay suficientes puntos de spawn, generamos los coleccionables. Intentaremos colocar el coleccionable en un punto aleatorio dentro del area de spawn
                // y evitaremos solapamientos con otros coleccionables u obstaculos. Si se llega a X intentos sin encontrar un punto valido, se salta la generacion de ese coleccionable
                if (collectiblePoints.Count >= 4)
                {

                    for (int j = 0; j < collectiblesInPiece; j++)
                    {
                        Vector3 pos;
                        int attempts = 0;
                        do
                        {
                            pos = RandomPointInQuad(
                                collectiblePoints[0].position,
                                collectiblePoints[1].position,
                                collectiblePoints[2].position,
                                collectiblePoints[3].position
                            );
                            attempts++;
                        }
                        while (usedPositions.Exists(p => Vector3.Distance(p, pos) < minDistance) && attempts < 10);
                        usedPositions.Add(pos);
                        pos.y += 0.7f;

                        GameObject collectible = Instantiate(collectiblePrefab, pos, Quaternion.identity);
                        Renderer rend = collectible.GetComponent<Renderer>();
                        if (rend != null)
                        {
                            // Gestionamos el color y la etiqueta del coleccionable dependiendo de los jugadores
                            // Como ya sabemos cuantos coleccionables hay por pieza, podemos realizar la distribucion en caso de que haya varios jugadores
                            // Si solo hay un jugador, directamente se asigna el color y la etiqueta correspondiente
                            if (GameManager.Instance.dosJugadores)
                            {
                                if (currentJ1 > 0 && currentJ2 > 0)
                                {
                                    int currentPlayer = Random.Range(0, 2);
                                    if (currentPlayer == 0)
                                    {
                                        rend.material.color = Color.blue;
                                        collectible.tag = "CollectibleJ1";
                                        currentJ1--;
                                    }
                                    else
                                    {
                                        rend.material.color = Color.red;
                                        collectible.tag = "CollectibleJ2";
                                        currentJ2--;
                                    }
                                }
                                else if (currentJ1 > 0)
                                {
                                    rend.material.color = Color.blue;
                                    collectible.tag = "CollectibleJ1";
                                    currentJ1--;
                                }
                                else if (currentJ2 > 0)
                                {
                                    rend.material.color = Color.red;
                                    collectible.tag = "CollectibleJ2";
                                    currentJ2--;
                                }
                            }
                            else
                            {
                                rend.material.color = Color.yellow;
                                collectible.tag = "Collectible";
                            }
                        }
                    }
                }
            }

            // Ahora generamos los obstaculos independientemente del modo 
            var obstaclePoints = helper.GetObstaclePoints();
            int nObstacles = GameManager.Instance.obstacles;
            // El procedimiento de spawn es similar al de los coleccionables, pero con un maximo de obstaculos que puede haber por pieza
            if (nObstacles > 0 && obstaclePoints.Count == 4)
            {
                int numObstacles = Random.Range(1, nObstacles + 1); // +1 para incluir nObstacles
                for (int o = 0; o < numObstacles; o++)
                {
                    Vector3 pos;
                    int attempts = 0;
                    do
                    {
                        pos = RandomPointInQuad(
                            obstaclePoints[0].position,
                            obstaclePoints[1].position,
                            obstaclePoints[2].position,
                            obstaclePoints[3].position
                        );
                        attempts++;
                    }
                    while (usedPositions.Exists(p => Vector3.Distance(p, pos) < minDistance) && attempts < 10);

                    usedPositions.Add(pos); // Añade la posición del obstáculo a la lista

                    GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                    // Busca los puntos de entrada y salida en la pieza
                    Transform entry = piece.transform.Find("Entry");
                    Transform exit = piece.transform.Find("Exit");
                    // Ajustamos la rotacion del obstaculo en funcion de la rotacion que debe tener la carretera 
                    Quaternion obstacleRotation = Quaternion.identity;
                    if (entry != null && exit != null)
                    {
                        Vector3 forward = (exit.position - entry.position).normalized;
                        obstacleRotation = Quaternion.LookRotation(forward, Vector3.up);
                    }

                    GameObject obstacleInstance = Instantiate(obstaclePrefab, pos, obstacleRotation);

                    // Ajusta la fuerza de repulsión
                    Obstacle obstacle = obstacleInstance.GetComponent<Obstacle>();
                    if (obstacle != null)
                        obstacle.SetPushForce(GameManager.Instance.obstaclePushForce);

                    // Ajuste de altura debido a que el pivote de los obstaculos no esta en la base
                    Collider collider = obstacleInstance.GetComponent<Collider>();
                    if (collider != null)
                    {
                        float baseY = collider.bounds.center.y - collider.bounds.extents.y;
                        float deltaY = obstacleInstance.transform.position.y - baseY;
                        Vector3 adjPos = obstacleInstance.transform.position;
                        adjPos.y = pos.y + deltaY;
                        obstacleInstance.transform.position = adjPos;
                    }
                    else
                    {
                        Renderer rend = obstacleInstance.GetComponent<Renderer>();
                        if (rend != null)
                        {
                            float baseY = rend.bounds.center.y - rend.bounds.extents.y;
                            float deltaY = obstacleInstance.transform.position.y - baseY;
                            Vector3 adjPos = obstacleInstance.transform.position;
                            adjPos.y = pos.y + deltaY;
                            obstacleInstance.transform.position = adjPos;
                        }
                    }
                }
            }
        }
    }


}

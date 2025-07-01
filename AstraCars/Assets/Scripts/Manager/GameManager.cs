using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    public List<PlayerData> players = new List<PlayerData>();
    public GameObject barraGasolina;
    public Transform barrasParent;

    public GameObject barraScore;
    public GameObject barraGoalPrefab;
    // Texto que muestra el objetivo de coleccionables
    private CollectiblesGoals collectiblesGoals;
    public MapGenerator mapGenerator;


    public IGameMode gameMode;

    public bool dosJugadores = false;
    public GameObject carPrefab;
    public GameObject carPrefab2;
    public GameObject cameraPrefab;

    public GameObject countdownPrefab; // Prefab para el texto de cuenta atrás
    private Countdown countdownInstance;
    public float countdownTime = 10f; // Tiempo de cuenta atrás en segundos

    public NavMeshBaker navMeshSurface;

    public int enemySpawnTime = 5;
    public int obstacles = 10;
    public float obstaclePenalty = 5f;
    public float obstaclePushForce = 5000f;
    public float playerMaxSpeed = 10f; // Velocidad máxima del jugador
    public float playerVelocidadGiro = 20f;

    private bool gameOver = false;
    public GameObject gameOverPanel;
    private Animator gameOverAnimator;

    private bool victory = false;
    public GameObject winPanel;
    public GameObject winPanel1;
    public GameObject winPanel2;
    private Animator winAnimator;
    public bool waitingForStart = true;
    public GameObject startPanel;

    private PlayerData winner = null;





    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }


    void Start()
    {

        waitingForStart = true;
        if (startPanel != null)
            startPanel.SetActive(true);
        Time.timeScale = 0f;


        dosJugadores = PlayerPrefs.GetInt("DosJugadores", 0) == 1;
        string mode = PlayerPrefs.GetString("GameMode", "CollectMode");
        if (mode == "CollectMode")
        {
            gameMode = new CollectMode();
        }
        else if (mode == "ReachEndMode")
        {
            gameMode = new ReachEndMode();
        }



        CreatePlayers();
        gameMode.Init(this);

        string difficulty = PlayerPrefs.GetString("Difficulty", "Normal");
        if (gameMode is ReachEndMode)
            CreateCountdownText();
        SetConfiguration(gameMode, difficulty);
        if (gameMode is CollectMode)
        {
            CreateBarraScore();
            CreateGoalText();
        }


        CreateBarraGasolina();
        mapGenerator.SpawnPieces();
        mapGenerator.FillRoadMap();

        var navMeshBaker = FindFirstObjectByType<NavMeshBaker>();
        if (navMeshBaker != null)
            navMeshBaker.BakeNavMesh();
        else
            //Debug.LogWarning("NavMeshBaker no encontrado en la escena de juego.");


        if (gameOverPanel != null)
            gameOverAnimator = gameOverPanel.GetComponent<Animator>();

        if (winPanel != null)
            winAnimator = winPanel.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (waitingForStart)
        {
            foreach (var player in players)
            {
                if (InputManager.GetRespawn(player))
                {
                    waitingForStart = false;
                    if (startPanel != null)
                        startPanel.SetActive(false);
                    Time.timeScale = 1f; // Reanudar el juego
                    break;
                }
            }
            return; // No ejecutar lógica de juego hasta que se pulse la tecla
        }


        if (gameOver || victory) return;
        foreach (PlayerData player in players)
        {
            if (player.hasFinished) continue;
            if (player.GetGasolina() > 0)
            {
                player.SetGasolina( player.GetGasolina() - Time.deltaTime * 1f);
            }

        }

        if (!gameOver && gameMode.IsGameOver())
        {
            gameOver = true;
            ShowGameOverScreen();

        }


    }

    void CreatePlayers()
    {
        GameObject coche1 = Instantiate(carPrefab, new Vector3(-2, 9, 0), Quaternion.identity);
        PlayerData player1 = coche1.GetComponent<PlayerData>();
        player1.SetPlayerType(true);
        players.Add(player1);

        CarController carController1 = coche1.GetComponent<CarController>();
        if (carController1 != null)
        {
            carController1.maxSpeed = playerMaxSpeed;
            carController1.maxSteeringAngle = playerVelocidadGiro;
        }

        // Guardar posición y rotación local del prefab
        Vector3 cameraLocalPos = cameraPrefab.transform.localPosition;
        Quaternion cameraLocalRot = cameraPrefab.transform.localRotation;

        // Instanciar la cámara y hacerla hija del coche
        GameObject camera1 = Instantiate(cameraPrefab);
        camera1.transform.SetParent(coche1.transform, false); // false mantiene localPosition/localRotation
        camera1.transform.localPosition = cameraLocalPos;
        camera1.transform.localRotation = cameraLocalRot;

        Camera camComponent1 = camera1.GetComponent<Camera>();
        if (dosJugadores)
            camComponent1.rect = new Rect(0, 0.5f, 1, 0.5f); // Parte superior
        else
            camComponent1.rect = new Rect(0, 0, 1, 1);

        if (dosJugadores)
        {
            GameObject coche2 = Instantiate(carPrefab2, new Vector3(2, 9, 0), Quaternion.identity);
            PlayerData player2 = coche2.GetComponent<PlayerData>();
            player2.SetPlayerType(false);
            players.Add(player2);

            CarController carController2 = coche2.GetComponent<CarController>();
            if (carController2 != null)
            {

                carController2.maxSpeed = playerMaxSpeed;
                carController2.maxSteeringAngle = playerVelocidadGiro;
            }

            GameObject camera2 = Instantiate(cameraPrefab);
            camera2.transform.SetParent(coche2.transform, false);
            camera2.transform.localPosition = cameraLocalPos;
            camera2.transform.localRotation = cameraLocalRot;

            Camera camComponent2 = camera2.GetComponent<Camera>();
            camComponent2.rect = new Rect(0, 0, 1, 0.5f); // Parte inferior
            camera2.GetComponent<AudioListener>().enabled = false;
        }
    }

    void CreateBarraGasolina()
    {
        for (int i = 0; i < players.Count; i++)
        {
            GameObject barraGO = Instantiate(barraGasolina, barrasParent);
            BarraGasolina barra = barraGO.GetComponent<BarraGasolina>();
            barra.playerData = players[i];

            RectTransform rt = barraGO.GetComponent<RectTransform>();
            if (rt != null)
            {
                if (i == 0)
                {
                    rt.anchorMin = new Vector2(0, 1);
                    rt.anchorMax = new Vector2(0, 1);
                    rt.pivot = new Vector2(0, 1);
                    rt.anchoredPosition = new Vector2(20, -100);
                }
                else if (i == 1)
                {
                    rt.anchorMin = new Vector2(1, 1);
                    rt.anchorMax = new Vector2(1, 1);
                    rt.pivot = new Vector2(1, 1);
                    rt.anchoredPosition = new Vector2(-20, -100);
                }
            }
        }
    }
    public void PlayerReachedEnd(PlayerData playerData)
    {
        playerData.hasFinished = true;
        gameMode.OnPlayerReachEnd(playerData);
    }
    public void PlayerPickCollectible(PlayerData playerData)
    {
        // Actualiza el texto del objetivo si existe y el modo es CollectMode
        if (gameMode is CollectMode collectMode && collectiblesGoals != null)
        {
            int remaining = collectMode.collectiblesToWin - collectMode.GetTotalCollected();
            collectiblesGoals.UpdateGoalText(Mathf.Max(remaining, 0));
        }
    }

    void CreateGoalText()
    {
        if (gameMode is CollectMode collectMode)
        {
            GameObject goalTextGO = Instantiate(barraGoalPrefab, barrasParent);
            RectTransform rt = goalTextGO.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);
                rt.anchoredPosition = new Vector2(768, 0);
            }
            collectiblesGoals = goalTextGO.GetComponent<CollectiblesGoals>();
            if (collectiblesGoals != null)
            {
                collectiblesGoals.UpdateGoalText(collectMode.collectiblesToWin);
            }
        }
    }

    void CreateCountdownText()
    {
        GameObject countDowwnGO = Instantiate(countdownPrefab, barrasParent);
        RectTransform rt = countDowwnGO.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(768, 0);
        }
        countdownInstance = countDowwnGO.GetComponent<Countdown>();
        if (countdownInstance != null)
        {
            countdownInstance.StartCountdown(countdownTime);
        }

    }

    void CreateBarraScore()
    {
        for (int i = 0; i < players.Count; i++)
        {
            GameObject barraScoreGO = Instantiate(barraScore, barrasParent);
            ScoreText scoreScript = barraScoreGO.GetComponent<ScoreText>();
            scoreScript.playerData = players[i];

            RectTransform scoreRT = barraScoreGO.GetComponent<RectTransform>();
            if (scoreRT != null)
            {
                if (i == 0) // Jugador 1 esquina superior izquierda
                {
                    scoreRT.anchorMin = new Vector2(0, 1);
                    scoreRT.anchorMax = new Vector2(0, 1);
                    scoreRT.pivot = new Vector2(0, 1);
                    scoreRT.anchoredPosition = new Vector2(20, -14);
                }
                else if (i == 1) // Jugador 2 esquina superior derecha
                {
                    scoreRT.anchorMin = new Vector2(1, 1);
                    scoreRT.anchorMax = new Vector2(1, 1);
                    scoreRT.pivot = new Vector2(1, 1);
                    scoreRT.anchoredPosition = new Vector2(-5, -14);
                }
            }
        }
    }

    public float GetCountdownTime()
    {
        return countdownInstance != null ? countdownInstance.GetTimeRemaining() : 0f;
    }

    private void SetConfiguration(IGameMode gameMode, string difficulty)
    {
        //TODO: Mirar si añadir la velocidad de giro
        switch (difficulty)
        {
            case "Facil":
                if (gameMode is CollectMode collectModeFacil)
                {
                    collectModeFacil.collectiblesToWin = 10;
                    collectModeFacil.collectibleMultiplier = 5f;
                }
                else if (gameMode is ReachEndMode)
                {
                    countdownInstance.StartCountdown(360f);
                }
                SetCommonConfig(5f, 300f, 20, 20, 2, 5f, 0f, 6f);
                break;

            case "Normal":
                if (gameMode is CollectMode collectModeNormal)
                {
                    collectModeNormal.collectiblesToWin = 20;
                    collectModeNormal.collectibleMultiplier = 4f;
                }
                else if (gameMode is ReachEndMode reachEndMode)
                {
                    countdownInstance.StartCountdown(300f);
                }
                SetCommonConfig(10f, 250f, 15, 25, 4, 10f, 5000f, 8f);


                break;
            case "Dificil":
                if (gameMode is CollectMode collectModeDificil)
                {
                    collectModeDificil.collectiblesToWin = 30;
                    collectModeDificil.collectibleMultiplier = 3f;
                }
                else if (gameMode is ReachEndMode reachEndMode)
                {
                    countdownInstance.StartCountdown(240f);
                }
                SetCommonConfig(15f, 200f, 10, 30, 7, 15f, 8000f, 15f);
                break;

            case "Personalizada":
                if (gameMode is CollectMode collectPersonalizada)
                {
                    collectPersonalizada.collectiblesToWin = PlayerPrefs.GetInt("CustomTotalCollectibles", 20);
                    collectPersonalizada.collectibleMultiplier = PlayerPrefs.GetInt("CustomCollectiblesMultiplier", 2);
                }
                else if (gameMode is ReachEndMode reachEndMode)
                {
                    countdownInstance.StartCountdown(PlayerPrefs.GetFloat("CustomTime", 180f));
                }
                foreach (var player in players)
                {
                    CarController car = player.GetComponent<CarController>();
                    if (car != null)
                        car.maxSpeed = PlayerPrefs.GetFloat("CustomMaxSpeed", 10f);
                    player.gasolinaMaxima = PlayerPrefs.GetFloat("CustomGasolina", 250f);
                    player.SetGasolina(player.gasolinaMaxima);
                }
                enemySpawnTime = PlayerPrefs.GetInt("CustomEnemySpawn", 15); // Tiempo de spawn de enemigos
                mapGenerator.maxPieces = PlayerPrefs.GetInt("CustomNPieces", 25);
                obstacles = PlayerPrefs.GetInt("CustomNObstacles", 7);
                obstaclePenalty = PlayerPrefs.GetFloat("CustomPenalization", 10f);
                obstaclePushForce = PlayerPrefs.GetFloat("CustomPushBack", 5f);
                obstaclePushForce *= 1000f; // Multiplicamos por 1000 para que sea mas sencillo para el usr de ajustar en config
                playerVelocidadGiro = PlayerPrefs.GetFloat("CustomVelocidadGiro", 4f);
                //playerVelocidadGiro *= 2;
                break;
        }

    }

    private void SetCommonConfig(float maxSpeed, float gasolinaMax, int enemySpawn, int maxPieces, int obstacles, float penalty, float pushForce, float steerAngle)
    {
        foreach (var player in players)
        {
            CarController car = player.GetComponent<CarController>();
            if (car != null)
            {
                car.maxSpeed = maxSpeed;
                car.maxSteeringAngle = steerAngle;
            }

            player.gasolinaMaxima = gasolinaMax;
            player.SetGasolina(player.gasolinaMaxima);

        }
        enemySpawnTime = enemySpawn;
        mapGenerator.maxPieces = maxPieces;
        this.obstacles = obstacles;
        obstaclePenalty = penalty;
        obstaclePushForce = pushForce;
    }

    void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverAnimator != null)
            {
                gameOverAnimator.SetBool("Show", true);
            }
            StartCoroutine(LoadGameOverSceneAfterDelay(3f));
        }
    }

    private IEnumerator LoadGameOverSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
        SceneManager.LoadScene("GameOver");
    }

    void ShowVictoryScreen()
    {
        if (gameMode is CollectMode collectMode)
        {
            if (winPanel != null)
            {
                winPanel.SetActive(true);
                var animator = winPanel.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("Show", true);
                }
                StartCoroutine(LoadVictorySceneAfterDelay(3f));
            }
        }
        else if (gameMode is ReachEndMode)
        {
            if (!dosJugadores)
            {
                if (winPanel != null)
                {
                    winPanel.SetActive(true);
                    var animator = winPanel.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.SetBool("Show", true);
                    }
                    StartCoroutine(LoadVictorySceneAfterDelay(3f));
                }
            }
            else
            {
                if (winner.IsPlayer1())
                {
                    if (winPanel1 != null)
                    {
                        winPanel1.SetActive(true);
                        var animator = winPanel1.GetComponent<Animator>();
                        if (animator != null)
                        {
                            animator.SetBool("Show", true);
                        }
                        StartCoroutine(LoadVictorySceneAfterDelay(3f));
                    }
                }
                else if (!winner.IsPlayer1())
                {
                    if (winPanel2 != null)
                    {
                        winPanel2.SetActive(true);
                        var animator = winPanel2.GetComponent<Animator>();
                        if (animator != null)
                        {
                            animator.SetBool("Show", true);
                        }
                        StartCoroutine(LoadVictorySceneAfterDelay(3f));
                    }
                }
            }
        }
    }

    private IEnumerator LoadVictorySceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
        SceneManager.LoadScene("Victory");
    }

    public void OnVictory(PlayerData winner)
    {
        if (!victory)
        {
            this.winner = winner;
            victory = true;
            ShowVictoryScreen();
        }
    }

}

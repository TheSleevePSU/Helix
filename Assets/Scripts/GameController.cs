﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameController : MonoBehaviour
{
    public bool debugGuiDisplay = false;
    public bool showHelpScreen = false;
    public bool debugCheats = false;

    int breakpoint = 0;
    private bool levelReady = true;
    public float secondsPerTurn = 1f;
    public GameObject playerPrefab;
    public int level = 1;

    public Canvas gameInfoCanvas;
    public Text guiTextLevel;
    public Text guiTextEnemies;
    public Text guiTextHP;

    private LevelGenerator levelGenerator;

    /// <summary>
    /// Singleton instance
    /// </summary>
    public static GameController instance;

    /// <summary>
    /// Master list of active objects generated by player (update during playerTurn state)
    /// </summary>
    public List<ActiveObject> activeObjects;

    /// <summary>
    /// List of objects that are currently moving in the playerTurn state. Must be empty before state ends.
    /// </summary>
    public List<ActiveObject> currentlyActiveObjects;

    /// <summary>
    /// List of all enemies in the scene
    /// </summary>
    public List<Enemy> enemies;

    /// <summary>
    /// List of enemies that are currently executing turns in the enemyTurn state. Must be empty before state ends.
    /// </summary>
    public List<Enemy> currentlyActiveEnemies;

    /// <summary>
    /// Permanent reference to player instance
    /// </summary>
    public Player player;

    /// <summary>
    /// Reference to Player currently executing turns in the playerTurn state. Must be empty before state ends.
    /// </summary>
    public List<Player> currentlyActivePlayer;

    /// <summary>
    /// List of place savers used in Enemy move planning
    /// </summary>
    public List<PlaceSaver> placeSavers;

    public enum GameState
    {
        playerInput,
        playerTurn,
        enemyTurn,
        mainMenu,
        gameInactive
    };
    public GameState gameState = GameState.mainMenu;
    public GameState prePauseGameState = GameState.playerInput;
    public Canvas canvasMainMenu;

    // Use this for initialization
    void Start()
    {
    }

    /// <summary>
    /// Awake is used to initialize any variables or game state before the game starts. 
    /// Awake is called only once during the lifetime of the script instance. 
    /// Awake is called after all objects are initialized so you can safely speak to other objects or query them.
    /// </summary>
    void Awake()
    {
        if (GameController.instance != null) DestroyImmediate(this);
        instance = this;
        activeObjects = new List<ActiveObject>();
        currentlyActiveObjects = new List<ActiveObject>();
        enemies = new List<Enemy>();
        currentlyActiveEnemies = new List<Enemy>();
        currentlyActivePlayer = new List<Player>();
        placeSavers = new List<PlaceSaver>();
        levelGenerator = GetComponent<LevelGenerator>();
        gameState = GameState.mainMenu;
        canvasMainMenu = FindObjectOfType<Canvas>();
        player = FindObjectOfType<Player>();
        player.GetComponent<SpriteRenderer>().enabled = false;
        level = 1;
    }

    public void ClearAllLists()
    {
        activeObjects.Clear();
        currentlyActiveObjects.Clear();
        enemies.Clear();
        currentlyActiveEnemies.Clear();
        currentlyActivePlayer.Clear();
        placeSavers.Clear();
    }

    public void ClearLevel()
    {
        foreach (Enemy e in enemies)
        {
            if (e != null)
                Destroy(e.gameObject);
        }
        foreach (ActiveObject ao in activeObjects)
        {
            if (ao != null)
                Destroy(ao.gameObject);
        }
        foreach (PlaceSaver ps in placeSavers)
        {
            if (ps != null)
                Destroy(ps.gameObject);
        }
        ClearAllLists();
    }

    // Update is called once per frame
    void Update()
    {
        if (debugCheats && Input.GetKeyDown(KeyCode.L))
        {
            ClearLevel();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        breakpoint++; //Breakpoint for debugging

        if (player.isAlive)
        {
            switch (gameState)
            {
                case GameState.playerInput:
                    if (enemies.Count == 0)
                    {
                        LevelOver();
                        break;
                    }
                    else if (player != null)
                        if (player.isReadyForTurn)
                            PlayerTurnStart();
                    break;
                case GameState.playerTurn:
                    if (currentlyActivePlayer.Count == 0)
                    {
                        if (currentlyActiveObjects.Count == 0)
                        {
                            PlayerTurnEnd();
                        }
                    }
                    break;
                case GameState.enemyTurn:
                    if (currentlyActiveEnemies.Count == 0)
                    {
                        if (currentlyActiveObjects.Count == 0)
                        {
                            EnemyTurnEnd();
                            gameState = GameState.playerInput;
                        }
                    }
                    break;
                case GameState.mainMenu:
                    break;
                case GameState.gameInactive:
                    if (levelReady) gameState = GameState.playerTurn;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Perform setup for playerTurn state
    /// </summary>
    void PlayerTurnStart()
    {
        player.isReadyForTurn = false;
        currentlyActiveObjects = new List<ActiveObject>(activeObjects);
        currentlyActivePlayer = new List<Player>();
        currentlyActivePlayer.Add(player);
        foreach (ActiveObject ao in currentlyActiveObjects)
        {
            if (ao != null)
                ao.PrepareForTurn();
        }
        gameState = GameState.playerTurn;
        player.ClearPreviewObjects();
    }

    /// <summary>
    /// Perform cleanup after playerTurn state
    /// </summary>
    void PlayerTurnEnd()
    {
        if (!player.isAlive)
        {
            GameOver();
        }
        //else if (enemies.Count == 0 || player.hasReachedExit)
        //{
        //    LevelOver();
        //}
        //else
        //{
            EnemyTurnStart();
        //}
    }

    /// <summary>
    /// Perform setup for enemyTurn state. Call AI planning methods on all enemies.
    /// </summary>
    void EnemyTurnStart()
    {
        currentlyActiveEnemies = new List<Enemy>(enemies);
        currentlyActiveObjects = new List<ActiveObject>(activeObjects);
        foreach (ActiveObject ao in currentlyActiveObjects) ao.PrepareForTurn();
        foreach (Enemy e in enemies)
        {
            if (e != null)
            {
                e.GetDistanceToPlayer();
                e.CheckVisionToPlayer();
                e.SetState();
                e.AiPlanAction();
            }
        }
        gameState = GameState.enemyTurn;
    }

    /// <summary>
    /// Perform cleanup at end of enemyTurn state.
    /// </summary>
    void EnemyTurnEnd()
    {
        foreach (PlaceSaver ps in placeSavers)
        {
            Destroy(ps.gameObject);
        }
        placeSavers.Clear();
        foreach (Enemy e in enemies)
        {
            e.EndTurn();
        }
        player.GeneratePreviewObjects();
    }

    /// <summary>
    /// When levels are procedurally generated, call this function to find and register all relevant objects in the scene
    /// </summary>
    void RegisterAllObjectsInGeneratedScene()
    {
        enemies.Clear();
        Enemy enemyComponent;
        Enemy[] foundEnemies = FindObjectsOfType(typeof(Enemy)) as Enemy[];
        foreach (Enemy e in foundEnemies)
        {
            enemyComponent = e.GetComponent<Enemy>();
            if (enemyComponent != null) enemies.Add(enemyComponent);
        }

        activeObjects.Clear();
        ActiveObject activeObjectComponent;
        ActiveObject[] foundActiveObjects = FindObjectsOfType(typeof(ActiveObject)) as ActiveObject[];
        foreach (ActiveObject ao in foundActiveObjects)
        {
            activeObjectComponent = ao.GetComponent<ActiveObject>();
            if (activeObjectComponent != null) activeObjects.Add(activeObjectComponent);
        }

        player = FindObjectOfType(typeof(Player)) as Player;
    }

    public void RegisterActiveObject(ActiveObject ao)
    {
        if (!activeObjects.Contains(ao)) activeObjects.Add(ao);
    }

    public void DeRegisterActiveObject(ActiveObject ao)
    {
        if (activeObjects.Contains(ao)) activeObjects.Remove(ao);
        if (currentlyActiveObjects.Contains(ao)) currentlyActiveObjects.Remove(ao);
    }

    public void RegisterEnemy(Enemy e)
    {
        if (!enemies.Contains(e)) enemies.Add(e);
    }

    public void DeRegisterEnemy(Enemy e)
    {
        if (enemies.Contains(e)) enemies.Remove(e);
        if (currentlyActiveEnemies.Contains(e)) currentlyActiveEnemies.Remove(e);
    }

    public void RegisterPlayer(Player p)
    {
        if (player == null) player = p;
    }

    public void RegisterPlaceSaver(PlaceSaver ps)
    {
        if (!placeSavers.Contains(ps)) placeSavers.Add(ps);
    }

    public void EnemyTurnComplete(Enemy e)
    {
        if (currentlyActiveEnemies.Contains(e)) currentlyActiveEnemies.Remove(e);
    }

    public void PlayerTurnComplete(Player p)
    {
        if (currentlyActivePlayer.Contains(p)) currentlyActivePlayer.Remove(p);
    }

    public void ActiveObjectTurnComplete(ActiveObject ao)
    {
        if (currentlyActiveObjects.Contains(ao)) currentlyActiveObjects.Remove(ao);
    }

    public void GameOver()
    {
        SceneManager.LoadScene("LevelGenerator");
        gameState = GameState.mainMenu;
        canvasMainMenu.GetComponent<Canvas>().enabled = true;
        player.Initialize();
    }

    void LevelOver()
    {
        level++;

        if (level > 5)
        {
            SceneManager.LoadScene("WinScreen");
            Destroy(this);
        }
        else
        {
            gameState = GameState.gameInactive;
            currentlyActiveEnemies.Clear();
            currentlyActiveObjects.Clear();
            currentlyActivePlayer.Clear();
            activeObjects.Clear();
            enemies.Clear();

            levelGenerator.CreateLevel();
            RegisterAllObjectsInGeneratedScene();
            FindObjectOfType<Grid>().CreateGrid();
            player.GetComponent<SpriteRenderer>().enabled = true;
            player.hitPoints = player.hitPointsMax;
            gameState = GameState.playerInput;
        }
    }

    public void Pause()
    {
        if (gameState == GameState.mainMenu)
        {
            gameState = prePauseGameState;
        }
        else
        {
            prePauseGameState = gameState;
            gameState = GameState.mainMenu;
        }
    }

    public void NewGame()
    {
        Debug.Log("New Game");

        level = 1;
        levelGenerator.CreateLevel();
        RegisterAllObjectsInGeneratedScene();
        FindObjectOfType<Grid>().CreateGrid();
        player.GetComponent<SpriteRenderer>().enabled = true;

        gameState = GameState.playerInput;
    }

    void OnGUI()
    {
        if (debugGuiDisplay)
        {
            GUI.Label(new Rect(0, 0, 200, 100), "gameState " + gameState.ToString());
        }
        if (showHelpScreen)
        {
            //Todo - Display help screen on GUI canvas
        }
        if (gameState == GameState.mainMenu)
        {

        }
        guiTextLevel.text = " Level: " + level.ToString();
        guiTextEnemies.text = "Enemies: " + enemies.Count;
        guiTextHP.text = "HP: " + Player.instance.hitPoints.ToString();
    }
}

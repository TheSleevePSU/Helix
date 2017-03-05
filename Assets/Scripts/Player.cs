using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Player : MonoBehaviour
{
    private float speed;
    public bool isReadyForTurn;
    public bool isAlive;
    public bool hasReachedExit;

    public int hitPointsMax = 4;
    public int hitPoints = 4;

    public int actionPreviews = 5;

    public ActiveObject testProjectile;
    public ActiveObject facingAttackActiveObject;
    public GameObject teleportEffect;

    public static Player instance;

    public Sprite corpseSprite;
    public GameObject corpsePrefab;

    public AudioClip audioReceiveDamage;
    public AudioClip audioMoveStep;
    public AudioClip audioFacingAttack;
    private AudioSource myAudioSource;

    private Collider2D myCollider;
    private SpriteRenderer mySpriteRenderer;

    public Texture previewBoxTexture;

    public enum Facing
    {
        up,
        down,
        left,
        right
    };
    public Facing facing;

    /// <summary>
    /// List of actions to be performed on this turn
    /// </summary>
    private List<Action> actionsToExecute;

    /// <summary>
    /// Action selected; will be executed if player presses execute action key
    /// </summary>
    private List<Action> selectedAction;
    private List<List<Action>> actionQueue;

    private Move facingMove;
    private Spawn facingAttack;
    private List<GameObject> previewObjects;
    public GameObject facingMovePreview;
    public GameObject facingAttackPreview;
    public GameObject actionMovePreview;
    public GameObject actionSpawnPreview;
    public GameObject actionMoveInvalidPreview;
    public GameObject actionSpawnInvalidPreview;

    /// <summary>
    /// Action currently executing during playerTurn state - either a move or a spawn
    /// </summary>
    private Action currentAction;
    private Move currentMove;
    private Vector3 currentMoveTarget;
    private Spawn currentSpawn;

    public void Initialize()
    {
        instance = this;
        isAlive = true;
        isReadyForTurn = false;
        hasReachedExit = false;
        GameController.instance.RegisterPlayer(this);
        actionsToExecute = new List<Action>();
        selectedAction = new List<Action>();
        actionQueue = new List<List<Action>>();
        previewObjects = new List<GameObject>();
        facingMove = new Move(Vector3.up, false, false, false);
        facingAttack = new Spawn(facingAttackActiveObject, Vector3.up, Vector3.zero);
        myCollider = GetComponent<Collider2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myAudioSource = GetComponent<AudioSource>();

        for (int i = 0; i < actionPreviews; i++)
        {
            actionQueue.Add(ActionGenerator.instance.GetAction());
        }

        selectedAction = actionQueue[0];
    }

    void Start()
    {
        Initialize();
    }

    void ChangeFacing(Facing f)
    {
        if (facing != f)
        {
            facing = f;
            facingMove = new Move(Vector3.up, false, false, false);
            facingAttack = new Spawn(facingAttackActiveObject, Vector3.up, Vector3.zero);
            GeneratePreviewObjects();
        }
        else
        {
            if (facingMove.CanPerform(transform.position + RotateByFacing(Vector3.up, facing)))
            {
                myAudioSource.PlayOneShot(audioMoveStep);
                actionsToExecute.Add(facingMove);
                isReadyForTurn = true;
            }
            else if (facingAttack.CanPerform(transform.position + RotateByFacing(Vector3.up, facing)))
            {
                myAudioSource.PlayOneShot(audioFacingAttack);
                actionsToExecute.Add(facingAttack);
                isReadyForTurn = true;
            }
        }
    }

    void Update()
    {
        switch (GameController.instance.gameState)
        {
            case GameController.GameState.playerInput:
                if (isAlive)
                {
                    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        ChangeFacing(Facing.up);
                    }
                    else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        ChangeFacing(Facing.left);
                    }
                    else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        ChangeFacing(Facing.down);
                    }
                    else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        ChangeFacing(Facing.right);
                    }
                    else if (Input.GetKeyDown(KeyCode.Q))
                    {
                        actionsToExecute.Add(new Move(Vector3.zero, true, true, false));
                        isReadyForTurn = true;
                    }
                    else if (Input.GetKeyDown(KeyCode.E))
                    {
                        actionsToExecute = new List<Action>(selectedAction);
                        if (actionsToExecute.Count > 0) isReadyForTurn = true;
                        RemoveActionAtTopOfQueue();
                    }
                }
                break;
            case GameController.GameState.playerTurn:
                if (isAlive)
                {
                    if (actionsToExecute.Count > 0)
                    {
                        if (currentAction == null) GetAction();
                        if (currentMove != null)
                        {
                            transform.position = Vector3.MoveTowards(transform.position, currentMoveTarget, speed * Time.deltaTime);
                            if (transform.position == currentMoveTarget)
                            {
                                if (currentMove.teleport)
                                {
                                    myCollider.enabled = true;
                                    mySpriteRenderer.enabled = true;
                                }
                                actionsToExecute.RemoveAt(0);
                                currentMove = null;
                                currentAction = null;
                            }
                        }
                        if (currentSpawn != null)
                        {
                            ActiveObject ao = Instantiate(currentSpawn.spawnObject, transform.position + RotateByFacing(currentSpawn.spawnLocation, facing), Quaternion.identity) as ActiveObject;
                            ao.trajectory = RotateByFacing(currentSpawn.trajectory, facing);
                            ao.faction = ActiveObject.Faction.player;
                            ao.transform.rotation = Quaternion.LookRotation(Vector3.forward, ao.trajectory);
                            actionsToExecute.RemoveAt(0);
                            currentSpawn = null;
                            currentAction = null;
                        }
                    }
                    else
                    {
                        GameController.instance.SendMessage("PlayerTurnComplete", this);
                    }
                }
                break;
            case GameController.GameState.enemyTurn:
                break;
            case GameController.GameState.mainMenu:
                break;
            case GameController.GameState.gameInactive:
                break;
            default:
                break;
        }
    }

    void RemoveActionAtTopOfQueue()
    {
        actionQueue.RemoveAt(0);
        actionQueue.Add(ActionGenerator.instance.GetAction());
        selectedAction = actionQueue[0];
    }

    void RotateSpriteByFacing(GameObject go, Facing f)
    {
        go.transform.rotation = Quaternion.LookRotation(Vector3.forward, RotateByFacing(Vector3.up, f));
    }

    public void GeneratePreviewObjects()
    {
        GameObject go;
        Vector3 pos = transform.position;

        ClearPreviewObjects();
        if (facingMove != null)
        {
            if (facingMove.CanPerform(pos + RotateByFacing(Vector3.up, facing)))
            {
                go = (GameObject)Instantiate(facingMovePreview, pos + RotateByFacing(Vector3.up, facing), Quaternion.identity);
                RotateSpriteByFacing(go, facing);
                previewObjects.Add(go);
            }
            else if (facingAttack != null)
            {
                if (facingAttack.CanPerform(pos + RotateByFacing(Vector3.up, facing)))
                {
                    go = (GameObject)Instantiate(facingAttackPreview, pos + RotateByFacing(Vector3.up, facing), Quaternion.identity);
                    RotateSpriteByFacing(go, facing);
                    previewObjects.Add(go);
                }
            }
        }
        if (selectedAction != null)
        {
            foreach (Action a in selectedAction)
            {
                if (a is Move)
                {
                    Move m = a as Move;
                    if (m.CanPerform(pos + RotateByFacing(m.position, facing)))
                    {
                        go = (GameObject)Instantiate(actionMovePreview, pos + RotateByFacing(m.position, facing), Quaternion.identity);
                        previewObjects.Add(go);
                    }
                    else
                    {
                        go = (GameObject)Instantiate(actionMoveInvalidPreview, pos + RotateByFacing(m.position, facing), Quaternion.identity);
                        previewObjects.Add(go);
                    }
                }
                if (a is Spawn)
                {
                    Spawn s = a as Spawn;
                    if (s.CanPerform(transform.position + RotateByFacing(s.spawnLocation, facing)))
                    {
                        go = (GameObject)Instantiate(actionSpawnPreview, pos + RotateByFacing(s.spawnLocation, facing) + RotateByFacing(s.trajectory, facing), Quaternion.identity);
                        previewObjects.Add(go);
                    }
                    else
                    {
                        go = (GameObject)Instantiate(actionSpawnInvalidPreview, pos + RotateByFacing(s.spawnLocation, facing) + RotateByFacing(s.trajectory, facing), Quaternion.identity);
                        previewObjects.Add(go);
                    }
                }
            }
        }
    }

    public void ClearPreviewObjects()
    {
        foreach (GameObject go in previewObjects)
        {
            Destroy(go.gameObject);
        }
        previewObjects.Clear();
    }

    Vector3 RotateByFacing(Vector3 input, Facing f)
    {
        float outX = input.x;
        float outY = input.y;
        float outZ = input.z;
        switch (facing)
        {
            case Facing.up:
                outX = input.x;
                outY = input.y;
                break;
            case Facing.right:
                outX = input.y;
                outY = -input.x;
                break;
            case Facing.down:
                outX = -input.x;
                outY = -input.y;
                break;
            case Facing.left:
                outX = -input.y;
                outY = input.x;
                break;
        }
        return new Vector3(outX, outY, outZ);
    }

    void GetAction()
    {
        if (actionsToExecute[0] is Move)
        {
            currentMove = actionsToExecute[0] as Move;
            if (!currentMove.CanPerform(transform.position + RotateByFacing(currentMove.position, facing)))
                currentMove = new Move(Vector3.zero, true, true, false);
            currentAction = currentMove;
            speed = currentMove.position.magnitude / GameController.instance.secondsPerTurn;
            currentMoveTarget = transform.position + RotateByFacing(currentMove.position, facing);
            if (currentMove.teleport == true)
            {

                Instantiate(teleportEffect, transform.position, Quaternion.identity);
                Instantiate(teleportEffect, currentMoveTarget, Quaternion.identity);
                myCollider.enabled = false;
                mySpriteRenderer.enabled = false;
            }
        }
        else if (actionsToExecute[0] is Spawn)
        {
            currentSpawn = actionsToExecute[0] as Spawn;
            currentAction = currentSpawn;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(gameObject.name + " was triggered by " + other.name);
    }

    public void HitByEnemyActiveObject()
    {
        Debug.Log(gameObject.name + " was hit by enemy active object");
        myAudioSource.PlayOneShot(audioReceiveDamage);
        hitPoints--;
        if (hitPoints <= 0)
        {
            isAlive = false;
            GetComponent<SpriteRenderer>().enabled = false;
            GameObject c = Instantiate(corpsePrefab, transform.position, Quaternion.identity) as GameObject;
            SpriteRenderer sr = c.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = corpseSprite;
            GameController.instance.SendMessage("PlayerTurnComplete", this);
            GameController.instance.GameOver();
        }
    }

    void OnGUI()
    {
        if (GameController.instance.gameState != GameController.GameState.mainMenu)
        {
            //Vector3 position = Camera.main.WorldToScreenPoint(transform.position);
            //position.y = Screen.height - position.y;
            //GUI.color = Color.green;
            //GUI.Label(new Rect(position, new Vector2(100, 100)), hitPoints.ToString());

            GUI.color = Color.white;

            int apStartX = 5;
            int apStartY = 5;
            int apSizeX = 7 * 10;
            int apSizeY = 4 * 10;
            int apBorderX = 20;


            for (int a = 0; a < actionQueue.Count; a++)
            {
                Texture t = actionQueue[a][0].previewSprite.texture;

                GUI.DrawTexture(new Rect(apStartX + (a * (apSizeX + apBorderX)), apStartY, apSizeX, apSizeY), t);
            }
        }
    }
}


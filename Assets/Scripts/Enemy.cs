using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    private float speed;
    public bool isAlive;
    public float sightDistance;
    public LayerMask sightBlockMask;
    public float attackRange;
    public int coolDownTurns;
    private int coolDownCounter;
    public int aimTurns;
    private int aimCounter;
    public bool useMelee;

    public Sprite corpseSprite;
    public GameObject corpsePrefab;
    private ThoughtBubble thoughtBubble;

    private List<Action> actionsToExecute;

    private Action currentAction;
    private Move currentMove;
    private Vector3 currentMoveTarget;

    public ActiveObject attackProjectile;
    public float attackProjectileSpeed;

    public PlaceSaver placeSaver;

    private Spawn currentSpawn;

    bool canSeePlayer = false;
    float distanceToPlayer = 0f;

    public enum AiState
    {
        patrol,
        hunt,
        aim,
        attack,
        cooldown
    };
    public AiState aiState;

    // Use this for initialization
    void Start()
    {
        isAlive = true;
        GameController.instance.RegisterEnemy(this);
        actionsToExecute = new List<Action>();
        thoughtBubble = GetComponentInChildren<ThoughtBubble>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameController.instance.gameState)
        {
            case GameController.GameState.playerInput:
                break;
            case GameController.GameState.playerTurn:
                break;
            case GameController.GameState.enemyTurn:
                if (actionsToExecute.Count > 0)
                {
                    if (currentAction == null) GetAction();
                    if (currentMove != null)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, currentMoveTarget, speed * Time.deltaTime);
                        if (transform.position == currentMoveTarget)
                        {
                            actionsToExecute.RemoveAt(0);
                            currentMove = null;
                            currentAction = null;
                        }
                    }
                    if (currentSpawn != null)
                    {
                        Vector3 spawnPoint = new Vector3();
                        spawnPoint = transform.position;
                        if (useMelee)
                            if ((Player.instance.transform.position - transform.position).magnitude <= 1f)
                                spawnPoint = Player.instance.transform.position;
                        ActiveObject ao = Instantiate(attackProjectile, spawnPoint, Quaternion.identity) as ActiveObject;
                        ao.trajectory = (Player.instance.transform.position - spawnPoint).normalized * attackProjectileSpeed;
                        ao.faction = ActiveObject.Faction.enemy;
                        ao.transform.rotation = Quaternion.LookRotation(Vector3.forward, ao.trajectory);
                        coolDownCounter = coolDownTurns;
                        aiState = AiState.cooldown;
                        actionsToExecute.RemoveAt(0);
                        currentSpawn = null;
                        currentAction = null;
                    }
                }
                else
                {
                    GameController.instance.SendMessage("EnemyTurnComplete", this);
                }
                break;
            case GameController.GameState.mainMenu:
                break;
            case GameController.GameState.gameInactive:
                break;
            default:
                break;
        }
    }

    void GetAction()
    {
        if (actionsToExecute[0] is Move)
        {
            currentMove = actionsToExecute[0] as Move;
            currentAction = currentMove;
            speed = currentMove.position.magnitude / GameController.instance.secondsPerTurn;
            currentMoveTarget = transform.position + currentMove.position;
        }
        else if (actionsToExecute[0] is Spawn)
        {
            currentSpawn = actionsToExecute[0] as Spawn;
            currentAction = currentSpawn;
        }
    }

    public void AiPlanAction()
    {
        switch (aiState)
        {
            case AiState.patrol:
                PickRandomMove();
                break;
            case AiState.hunt:
                PathfindToPlayer();
                break;
            case AiState.aim:
                break;
            case AiState.attack:
                actionsToExecute.Add(new Spawn(attackProjectile));
                break;
            default:
                break;
        }
    }

    void PathfindToPlayer()
    {
        Grid.instance.pathfinding.FindPath(transform.position, Player.instance.transform.position);
        List<Node> pathToPlayer = Grid.instance.path;
        if (pathToPlayer != null)
        {
            Move m = new Move(pathToPlayer[0].worldPosition - transform.position, false, false, false);
            if (m.CanPerform(transform.position + m.position))
            {
                PlaceSaver ps = (PlaceSaver)Instantiate(placeSaver, transform.position + m.position, Quaternion.identity);
                GameController.instance.RegisterPlaceSaver(ps);
                actionsToExecute.Add(m);
            }
            else
            {
                LazyPathfindToPlayer();
            }
        }
    }

    void LazyPathfindToPlayer()
    {
        List<Action> possibleActions = new List<Action>();
        if (Player.instance.transform.position.x - transform.position.x > 0)
            possibleActions.Add(new Move(Vector3.right, false, false, false));
        if (Player.instance.transform.position.x - transform.position.x < 0)
            possibleActions.Add(new Move(Vector3.left, false, false, false));
        if (Player.instance.transform.position.y - transform.position.y > 0)
            possibleActions.Add(new Move(Vector3.up, false, false, false));
        if (Player.instance.transform.position.y - transform.position.y < 0)
            possibleActions.Add(new Move(Vector3.down, false, false, false));
        
        possibleActions.Shuffle();

        foreach (Action a in possibleActions)
        {
            if (a is Move)
            {
                Move m = a as Move;
                if (m.CanPerform(transform.position + m.position))
                {
                    PlaceSaver ps = (PlaceSaver)Instantiate(placeSaver, transform.position + m.position, Quaternion.identity);
                    GameController.instance.RegisterPlaceSaver(ps);
                    actionsToExecute.Add(m);
                    break;
                }
            }
        }
    }

    void PickRandomMove()
    {
        List<Action> possibleActions = new List<Action>();
        possibleActions.Add(new Move(Vector3.up, false, false, false));
        possibleActions.Add(new Move(Vector3.down, false, false, false));
        possibleActions.Add(new Move(Vector3.left, false, false, false));
        possibleActions.Add(new Move(Vector3.right, false, false, false));
        possibleActions.Shuffle();

        foreach (Action a in possibleActions)
        {
            if (a is Move)
            {
                Move m = a as Move;
                if (m.CanPerform(transform.position + m.position))
                {
                    PlaceSaver ps = (PlaceSaver)Instantiate(placeSaver, transform.position + m.position, Quaternion.identity);
                    GameController.instance.RegisterPlaceSaver(ps);
                    actionsToExecute.Add(m);
                    break;
                }
            }
        }
    }

    public void SetState()
    {
        switch (aiState)
        {
            case AiState.patrol:
                if (canSeePlayer)
                {
                    aiState = AiState.hunt;
                }
                if ((distanceToPlayer <= attackRange) && canSeePlayer)
                {
                    if (aimTurns > 0)
                    {
                        aiState = AiState.aim;
                        aimCounter = aimTurns;
                    }
                    else aiState = AiState.attack;
                }
                break;
            case AiState.hunt:
                if (canSeePlayer)
                {
                    aiState = AiState.hunt;
                }
                if ((distanceToPlayer <= attackRange) && canSeePlayer)
                {
                    if (aimTurns > 0)
                    {
                        aiState = AiState.aim;
                        aimCounter = aimTurns;
                    }
                    else aiState = AiState.attack;
                }
                break;
            case AiState.attack:
                break;
            case AiState.aim:
                break;
            case AiState.cooldown:
                if (coolDownCounter <= 0)
                    aiState = AiState.hunt;
                break;
        }
        switch (aiState)
        {
            case AiState.patrol:
                thoughtBubble.SetSprite(thoughtBubble.patrol);
                break;
            case AiState.hunt:
                thoughtBubble.SetSprite(thoughtBubble.hunt);
                break;
            case AiState.attack:
                thoughtBubble.SetSprite(thoughtBubble.attack);
                break;
            case AiState.aim:
                thoughtBubble.SetSprite(thoughtBubble.aim);
                break;
            case AiState.cooldown:
                thoughtBubble.SetSprite(thoughtBubble.cooldown);
                break;
        }
    }

    public void EndTurn()
    {
        switch (aiState)
        {
            case AiState.patrol:
                break;
            case AiState.hunt:
                break;
            case AiState.aim:
                aimCounter--;
                if (aimCounter == 0) aiState = AiState.attack;
                break;
            case AiState.attack:
                break;
            case AiState.cooldown:
                coolDownCounter--;
                if (coolDownCounter <= 0) aiState = AiState.hunt;
                break;
        }
    }

    public void GetDistanceToPlayer()
    {
        distanceToPlayer = (Player.instance.transform.position - transform.position).magnitude;
    }


    public void CheckVisionToPlayer()
    {
        //Vector3 directionVector = Player.instance.transform.position - transform.position;
        if (distanceToPlayer <= sightDistance)
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, Player.instance.transform.position, sightBlockMask);
            if (hit.collider != null)
            {
                canSeePlayer = false;
            }
            else
            {
                canSeePlayer = true;
            }
        }
        else
        {
            canSeePlayer = false;
        }
    }

    public void HitByPlayerActiveObject(object other)
    {
        Debug.Log(gameObject.name + " was hit by player object");
        GameController.instance.DeRegisterEnemy(this);
        GameObject c = Instantiate(corpsePrefab, transform.position, Quaternion.identity) as GameObject;
        SpriteRenderer sr = c.GetComponent<SpriteRenderer>();
        if (sr != null) sr.sprite = corpseSprite;
        Destroy(this.gameObject);
    }

    void OnGUI()
    {
        //Vector3 position = Camera.main.WorldToScreenPoint(transform.position);
        //position.y = Screen.height - position.y;
        //GUI.color = Color.red;
        //GUI.Label(new Rect(position, new Vector2(100, 100)), canSeePlayer.ToString());
    }
}
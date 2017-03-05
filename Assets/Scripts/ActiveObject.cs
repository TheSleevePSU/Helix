using UnityEngine;
using System.Collections;

public class ActiveObject : MonoBehaviour
{
    private float speed;
    public enum Faction
    {
        player,
        enemy,
        none
    };
    public Faction faction;

    public Vector3 trajectory;
    public Vector3 targetThisTurn;

    public ActiveObject(Faction _faction = Faction.none, Vector3 _trajectory = default(Vector3))
    {
        faction = _faction;
        trajectory = _trajectory;
    }

    // Use this for initialization
    void Start()
    {
        GameController.instance.RegisterActiveObject(this);
        targetThisTurn = transform.position + trajectory;
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameController.instance.gameState)
        {
            case GameController.GameState.playerInput:
                break;
            case GameController.GameState.playerTurn:
            case GameController.GameState.enemyTurn:
                transform.position = Vector3.MoveTowards(transform.position, targetThisTurn, speed * Time.deltaTime);
                if (transform.position == targetThisTurn)
                {
                    GameController.instance.SendMessage("ActiveObjectTurnComplete", this);
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

    public void PrepareForTurn()
    {
        if (targetThisTurn != null)
        {
            targetThisTurn = transform.position + trajectory;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, targetThisTurn - transform.position);
            speed = (targetThisTurn - transform.position).magnitude / GameController.instance.secondsPerTurn;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (faction == Faction.enemy || faction == Faction.none)
        {
            Player p = other.gameObject.GetComponent<Player>();
            if (p != null)
            {
                p.SendMessage("HitByEnemyActiveObject");
                GameController.instance.DeRegisterActiveObject(this);
                Destroy(this.gameObject);
            }
        }
        if (faction == Faction.player || faction == Faction.none)
        {
            Enemy e = other.gameObject.GetComponent<Enemy>();
            if (e != null)
            {
                e.SendMessage("HitByPlayerActiveObject", this);
                GameController.instance.DeRegisterActiveObject(this);
                Destroy(this.gameObject);
            }
        }
        Tile t = other.gameObject.GetComponent<Tile>();
        if (t != null)
        {
            if (t.blocksProjectiles)
            {
                GameController.instance.DeRegisterActiveObject(this);
                Destroy(this.gameObject);
            }
        }
        Barrel b = other.gameObject.GetComponent<Barrel>();
        if (b != null)
        {
            b.SendMessage("HitByActiveObject", this);
            GameController.instance.DeRegisterActiveObject(this);
            Destroy(this.gameObject);
        }
    }
}

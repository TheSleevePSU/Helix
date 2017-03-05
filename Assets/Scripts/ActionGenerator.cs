using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionGenerator : MonoBehaviour
{
    public ActiveObject defaultProjectile;
    public static ActionGenerator instance;
    public List<List<Action>> actions;

    public Sprite move01;
    public Sprite move02;
    public Sprite move03;
    public Sprite move04;
    public Sprite move05;
    public Sprite move06;
    public Sprite move07;
    public Sprite move08;
    public Sprite move09;
    public Sprite move10;
    public Sprite move11;
    public Sprite move12;
    public Sprite move13;
    public Sprite move14;
    public Sprite move15;
    public Sprite move16;
    public Sprite move17;
    public Sprite move18;
    public Sprite move19;
    public Sprite move20;
    public Sprite move21;
    public Sprite move22;
    public Sprite move23;
    public Sprite move24;

    void Awake()
    {
        instance = this;
        actions = new List<List<Action>>();
        InitializeLists();
    }

    void InitializeLists()
    {
        List<Action> tempAction = new List<Action>();

        //Move01
        tempAction.Add(new Move(new Vector3(-1f, 1f, 0), false, false, true, move01));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move02
        tempAction.Add(new Move(new Vector3(1f, 1f, 0), false, false, true, move02));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move03
        tempAction.Add(new Move(new Vector3(-2f, 2f, 0), false, false, true, move03));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move04
        tempAction.Add(new Move(new Vector3(-1f, 2f, 0), false, false, true, move04));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move05
        tempAction.Add(new Move(new Vector3(0f, 2f, 0), false, false, true, move05));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move06
        tempAction.Add(new Move(new Vector3(1f, 2f, 0), false, false, true, move06));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move07
        tempAction.Add(new Move(new Vector3(2f, 2f, 0), false, false, true, move07));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move08
        tempAction.Add(new Move(new Vector3(-3f, 3f, 0), false, false, true, move08));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move09
        tempAction.Add(new Move(new Vector3(-2f, 3f, 0), false, false, true, move09));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move10
        tempAction.Add(new Move(new Vector3(-1f, 3f, 0), false, false, true, move10));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move11
        tempAction.Add(new Move(new Vector3(0f, 3f, 0), false, false, true, move11));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move12
        tempAction.Add(new Move(new Vector3(1f, 3f, 0), false, false, true, move12));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move13
        tempAction.Add(new Move(new Vector3(2f, 3f, 0), false, false, true, move13));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move14
        tempAction.Add(new Move(new Vector3(3f, 3f, 0), false, false, true, move14));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        ///***----------------------------------------------------------------------

        //Move15
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(-1f, 1f), move15));
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3( 1f, 1f), move15));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move16
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(-1f, 2f), move16));
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(1f, 2f), move16));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move17
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(0, 2f), move17));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move18
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(-2f, 2f), move18));
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(0, 2f), move18));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move19
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(-1f, 2f), move19));
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(2f, 2f), move19));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move20
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(-2f, 2f), move20));
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(2f, 2f), move20));
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(0, 2f), move20));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move21
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(-2f, 3f), move21));
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(0f, 3f), move21));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move22
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(-1f, 3f), move22));
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(1f, 3f), move22));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move23
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(-1f, 1f), move23));
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(1f, 1f), move23));
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(0f, 3f), move23));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        //Move24
        tempAction.Add(new Move(new Vector3(0f, 2f, 0), false, false, true, move24));
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(-1f, 2f), move24));
        tempAction.Add(new Spawn(defaultProjectile, Vector3.zero, new Vector3(1f, 2f), move24));
        actions.Add(new List<Action>(tempAction));
        tempAction.Clear();

        actions.Shuffle();
    }

    public List<Action> GetAction()
    {
        List<Action> action = new List<Action>(actions[0]);
        actions.Add(actions[0]);
        actions.RemoveAt(0);
        return action;
    }
}


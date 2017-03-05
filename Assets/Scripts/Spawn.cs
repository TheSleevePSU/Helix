using UnityEngine;
using System.Collections;
using System;

public class Spawn : Action
{
    public ActiveObject spawnObject;
    public Vector3 spawnLocation;
    public Vector3 trajectory;

    public Spawn(ActiveObject _spawnObject = null, Vector3 _spawnLocation = default(Vector3), Vector3 _trajectory = default(Vector3), Sprite _previewSprite = default(Sprite))
    {
        spawnObject = _spawnObject;
        spawnLocation = _spawnLocation;
        trajectory = _trajectory;
        previewSprite = _previewSprite;
    }

    public override bool CanPerform(Vector3 worldPosition)
    {
        return true;
    }
}

